/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2015 Alexander Taylor
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */﻿

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using UnityEngine;


namespace KRASH
{

	// We want to load in all relevant game scenes, and be applied to all games.
	//	[KSPScenario (ScenarioCreationOptions.AddToAllGames, new GameScenes[] {
	//		GameScenes.SPACECENTER
	//		/*,
	//		GameScenes.EDITOR,
	//		GameScenes.FLIGHT,
	//		GameScenes.TRACKSTATION */
	//	})]

	[KSPAddon (KSPAddon.Startup.SpaceCentre, true)]
	public class KRASH : MonoBehaviour
	{

		// This class is a singleton, as much as Unity will allow.
		// There is probably a better way to do this in a Unity-like way,
		// But I don't know it.
		//public static KRASH instance;

		// This is a flag for marking a save as 'dirty'. Any flag with this flag
		// that enters SPACECENTER, EDITOR, or TRACKSTATION will be immediately reset
		//[KSPField (isPersistant = true)]
		//public bool SimulationActive = false;

		// This stores which of the cost settings is being used by this save
		//[KSPField (isPersistant = true)]
		//public string selectedCostsCfg = "";

		public  SimulationPauseMenu simPauseMenuInstance;
		public Configuration cfg = null;
		private static bool componentsLoaded = false;


		#if false
		public void simStart(Vessel v, double f)
		{
			Log.Info ("simStart");
		}
		public void testWrapper()
		{
			bool b = KRASHWrapper.SetSetupCosts (500.0f, 501.0f, 502.0f);
			b = KRASHWrapper.addToCosts (12345.0f);
			double d = KRASHWrapper.getCurrentSimCosts ();
			//KRASHWrapper.AddSimStartEvent (simStart);

		}
		#endif
		void Awake ()
		{
			Log.Info ("KRASH.Awake");
		}

		// We need to start AFTER the persistent data is loaded in KRASHPersistent
		// So the following code takes care of that.
		bool inited = false;

		void Update ()
		{
			

			if (!inited && KRASHPersistent.inited) {
				Start ();
				inited = true;
			}
			if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER) {
				Log.Info ("Current funds: " + Funding.Instance.Funds.ToString ());

				if (KRASHShelter.simCost != 0 && KRASHShelter.startingFunds == Funding.Instance.Funds && !KRASHShelter.persistent.shelterSimulationActive) {
					Funding.Instance.AddFunds (-1.0F * KRASHShelter.simCost, TransactionReasons.Any);
					KRASHShelter.startingFunds = 0;
					KRASHShelter.simCost = 0;
				}
			}
		}

		void Start ()
		{      
			Log.Info ("KRASH.Start");
			DontDestroyOnLoad (this);      
			// update the singleton;
			if (KRASHShelter.instance != null) {
				Log.Info ("KRASH.Start: KRASHShelter.instance ! null");
				return;
			}
			if (KRASHPersistent.inited == false)
				return;
			KRASHShelter.instance = this;
			APIManager api = APIManager.ApiInstance;

			//testWrapper ();

			if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER) {
				if (KRASHShelter.simCost > 0) {
					Funding.Instance.AddFunds (-1.0F * KRASHShelter.simCost, TransactionReasons.Any);
					Log.Info ("simCost found, Funds: " + Funding.Instance.Funds.ToString ());
					KRASHShelter.simCost = 0;
				}
			}

			Log.Info ("Loading configs");
			if (cfg == null)
				cfg = new Configuration ();
			
			if (KRASHShelter.persistent.selectedCostsCfg == "")
				KRASHShelter.persistent.selectedCostsCfg = "* " + HighLogic.CurrentGame.Parameters.preset.ToString ();
			if (cfg.LoadConfiguration (KRASHShelter.persistent.selectedCostsCfg) == false)
				cfg.LoadConfiguration ("* " + HighLogic.CurrentGame.Parameters.preset.ToString ());

			// Reload to pre-sim if we are in the wrong scene.
			if (HighLogic.LoadedScene != GameScenes.FLIGHT) {
				Log.Info ("Not GameScenes.FLIGHT, calling Deactivate");
				KRASHShelter.instance.Deactivate (GameScenes.SPACECENTER);
			}
            
			// Deploy scene-specific modules, for GUI hijacking and similar logic
			switch (HighLogic.LoadedScene) {
			case GameScenes.FLIGHT: 
				if (!componentsLoaded && KRASHShelter.persistent.shelterSimulationActive) {
					componentsLoaded = true;
					Log.Info ("Adding components");
					gameObject.AddComponent<FlightModule> ();
					gameObject.AddComponent<SimulationPauseMenu> ();
				}
				break;
			case GameScenes.EDITOR:
//				gameObject.AddComponent<EditorModule> ();
				break;
			}

			// If the sim is active, display the tell-tale
			SimulationNotification (KRASHShelter.persistent.shelterSimulationActive);
		}

		// This is called when the script is destroyed.
		// This is honestly probably not necessary, but better safe than sorry
		void OnDestroy ()
		{
			Log.Info ("OnDestroy");
			SimulationNotification (false); 
			return;
			//KRASHShelter.instance = null;
			//cfg = null;
		}

		// Activates the Simulation. Returns the success of the activation.
		public  bool Activate ()
		{
			Log.Info ("Activate");
			// for recording save status, not sure what this string actually is tbh
			string save = null;

			// Make sure the instance actually exists. I can't imagine this ever failing, but NREs are bad.
//			if (KRASHShelter.instance != null) {   
			// We create the pre-sim save.
			save = GamePersistence.SaveGame ("KRASHRevert", HighLogic.SaveFolder, SaveMode.OVERWRITE);

			// Mark the existing save as dirty.
			KRASHShelter.persistent.shelterSimulationActive = true;

			//GameObject t = GameObject.Find ("KRASHPersistent");
			//KRASHPersistent k = t.GetComponent<KRASHPersistent> ();
			//if (k != KRASHShelter.persistent) {
			//	Log.Info ("k != KRASHShelter.persistent");
			//}
			//KRASHShelter.persistent.shelterSimulationActive = true;


			// Record the scene we are coming from
			KRASHShelter.lastScene = HighLogic.LoadedScene;

			if (KRASHShelter.lastScene == GameScenes.EDITOR) {
//					HoloDeck.OnLeavingEditor (EditorDriver.editorFacility, EditorLogic.fetch.launchSiteName);
				KRASHShelter.instance.OnLeavingEditor (EditorDriver.editorFacility, LaunchGUI.selectedSite);
			}
			if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER) {
				KRASHShelter.simCost = 0;
				KRASHShelter.startingFunds = Funding.Instance.Funds;
			}

			// Start the tell-tale
			KRASHShelter.instance.SimulationNotification (true);
//			} else
//				Log.Info ("Activate:  KRASHShelter.instance == null");
			HighLogic.CurrentGame.Parameters.Flight.CanQuickSave = false;
			HighLogic.CurrentGame.Parameters.Flight.CanQuickLoad = false;
			Log.Info ("Activate returning: " + (save != null ? true : false).ToString ());
			return save != null ? true : false;
		}

		public void DestroyModules ()
		{
			Log.Info ("DestroyModules  unloading components");
			componentsLoaded = false;
			SimulationPauseMenu[] simulationPauseMenus = GameObject.FindObjectsOfType (typeof(SimulationPauseMenu)) as SimulationPauseMenu[];
			FlightModule[] flightModules = GameObject.FindObjectsOfType (typeof(FlightModule)) as FlightModule[];
//			EditorModule[] editorModules = GameObject.FindObjectsOfType (typeof(EditorModule)) as EditorModule[];


			foreach (SimulationPauseMenu simulationPauseMenu in simulationPauseMenus) {
				if (simulationPauseMenu != null)
					DestroyImmediate (simulationPauseMenu);
			}

			foreach (FlightModule flightModule in flightModules) {
				if (flightModule != null)
					DestroyImmediate (flightModule);
			}

//			foreach (EditorModule editorModule in editorModules) {
//				DestroyImmediate (editorModule);
//			}
		}
		// Deactivates the simulation. Success is destructive to the plugin state,
		// so... no return value
		public  void  Deactivate (GameScenes targetScene)
		{
			// This method only does something if the sim is active.
			if (KRASHShelter.persistent.shelterSimulationActive && System.IO.File.Exists (KSPUtil.ApplicationRootPath + "saves/" + HighLogic.SaveFolder + "/" + "KRASHRevert.sfs")) {
				// Weird bug can be intorduced by how KSP keeps around KSPAddons until it decides to destroy
				// them. We need to preempt this so extraneous behavior isn't observed

				DestroyModules ();

				// Ok, here is where this is tricky. We can't just directly load the save, we need to 
				// load the save into a new Game object, re-save that object into the default persistence,
				// and then change the scene. Weird shit, right? This is actually how the vanilla quickload
				// works!

				Game newGame = GamePersistence.LoadGame ("KRASHRevert", HighLogic.SaveFolder, true, false);
				GamePersistence.SaveGame (newGame, "persistent", HighLogic.SaveFolder, SaveMode.OVERWRITE);
				System.IO.File.Delete (KSPUtil.ApplicationRootPath + "saves/" + HighLogic.SaveFolder + "/" + "KRASHRevert.sfs");



				newGame.startScene = targetScene;
				// This has to be before... newGame.Start()
				if (targetScene == GameScenes.EDITOR) {
					newGame.editorFacility = KRASHShelter.lastEditor;
				}

				newGame.Start ();
				//HoloDeck.instance.SimulationActive = false;

				#if false
				if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER) {
					if (KRASHShelter.simCost > 0) {
						// Need to subtract the funds here, after the save has been loaded
						Log.Info ("simCost: " + KRASHShelter.simCost.ToString ());
						Log.Info ("Current funds before simCost: " + Funding.Instance.Funds.ToString ());
						Funding.Instance.AddFunds (-1 * KRASHShelter.simCost, TransactionReasons.RnDs);
						Log.Info ("Current funds after simCost: " + Funding.Instance.Funds.ToString ());
						KRASHShelter.simCost = 0;
					}
				}
				#endif

				// ... And this has to be after. <3 KSP
				if (targetScene == GameScenes.EDITOR) {
					EditorDriver.StartupBehaviour = EditorDriver.StartupBehaviours.LOAD_FROM_CACHE;
					ShipConstruction.ShipConfig = KRASHShelter.lastShip;
				}
			}
			//Log.Info ("Total sim cost: " + KRASHShelter.simCost.ToString ());
			//Funding.Instance.AddFunds(-1.0F * KRASHShelter.simCost, TransactionReasons.Any);
			//Log.Info ("Funds: " + Funding.Instance.Funds.ToString ());
			//KRASHShelter.simCost = 0;

			//KRASHShelter.instance.SimulationActive = false;
			//GameObject t = GameObject.Find ("KRASHPersistent");
			//KRASHPersistent k = t.GetComponent<KRASHPersistent> ();
			//if (k != KRASHShelter.persistent) {
			//	Log.Info ("k != KRASHShelter.persistent");
			//}
			KRASHShelter.persistent.shelterSimulationActive = false;

			//KRASHPersistent.shelterSimulationActive = false;
		}

		// This method should be called before activating the simulation directly from an editor, and allows
		// QOL improvements (like returning to that editor correctly, and automatically clearing the launch site)
		// Either of these values can be null, if you want to do that for some reason
		public  void OnLeavingEditor (EditorFacility facility, LaunchGUI.LaunchSite launchSite)
		{
			Log.Info ("OnLeavingEditor");
			// clear the launchpad.
			if (launchSite != null) {
				List<ProtoVessel> junkAtLaunchSite = ShipConstruction.FindVesselsLandedAt (HighLogic.CurrentGame.flightState, launchSite.name);

				foreach (ProtoVessel pv in junkAtLaunchSite) {
					HighLogic.CurrentGame.flightState.protoVessels.Remove (pv);
				}
			}

			if (facility != EditorFacility.None) {
				KRASHShelter.lastEditor = facility;
			}

			KRASHShelter.lastShip = ShipConstruction.ShipConfig;
			LaunchGUI.LaunchGuiInstance.setLaunchSite (launchSite);
		}

		// This is in here instead of GUI, because this isn't an 'implementation detail'
		// If some other mod uses sim mode for some reason, I still want this displayed.
		bool simNotificationActive = false;
		public void SimulationNotification (bool state)
		{
			
			//if (HighLogic.LoadedScene == GameScenes.FLIGHT) 
			if (state != simNotificationActive)
			{
				Log.Info ("SimulationNotification     HighLogic.LoadedScene: " + HighLogic.LoadedScene.ToString());
				simNotificationActive = state;
				switch (state) {
				case true:
					SetSimActiveNotification ();
					InvokeRepeating ("DoSimulationNotification", 1.0f, 2.0f);
					break;

				case false:
					CancelInvoke ("DoSimulationNotification");
					break;
				}
			}
		}

		static string simNotification;

		public void SetSimActiveNotification ()
		{
			simNotification = "Simulation Active";
			Log.Info ("SetSimActiveNotification   simNotification: " + simNotification);
		}

		public void SetSimNotification (string str)
		{
			simNotification = str;
			Log.Info ("SetSimNotification   simNotification: " + simNotification);
		}

		private void DoSimulationNotification ()
		{
			Log.Info("DoSimulationNotification   simNotification: " + simNotification);
			ScreenMessages.PostScreenMessage (simNotification, 1.0f, ScreenMessageStyle.UPPER_CENTER);
		}
	}
}
