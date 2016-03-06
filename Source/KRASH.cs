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

//using KRASH.SceneModules;
//using HoloDeck.AppLauncherButton;

namespace KRASH
{
	// We want to load in all relevant game scenes, and be applied to all games.
	[KSPScenario (ScenarioCreationOptions.AddToAllGames, new GameScenes[] {
		GameScenes.SPACECENTER,
		GameScenes.EDITOR,
		GameScenes.FLIGHT,
		GameScenes.TRACKSTATION
	})]
	class KRASH : ScenarioModule
	{
		// Tag for marking debug logs with
		//private const string TAG = "HoloDeck.HoloDeck";

		// This class is a singleton, as much as Unity will allow.
		// There is probably a better way to do this in a Unity-like way,
		// But I don't know it.
		public static KRASH instance;
		public static SimulationPauseMenu simPauseMenuInstance;

		public static Configuration cfg;
		private static bool componentsLoaded = false;

		// This is a flag for marking a save as 'dirty'. Any flag with this flag
		// that enters SPACECENTER, EDITOR, or TRACKSTATION will be immediately reset
		[KSPField (isPersistant = true)]
		public bool SimulationActive = false;

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
		// This is our entry point
		void Start ()
		{            
			// update the singleton;
			instance = this;
			APIManager api = APIManager.ApiInstance;

			//testWrapper ();

			#if true
			if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER)
			{
				if (KRASHShelter.simCost > 0) {
					Funding.Instance.AddFunds (-1.0F * KRASHShelter.simCost, TransactionReasons.Any);
					Log.Info ("simCost found, Funds: " + Funding.Instance.Funds.ToString ());
					KRASHShelter.simCost = 0;
				}
			}
			#endif

			Log.Info ("Loading configs");
			cfg = new Configuration ();

			cfg.LoadConfiguration (HighLogic.CurrentGame.Parameters.preset.ToString());

			// Reload to pre-sim if we are in the wrong scene.
			if (HighLogic.LoadedScene != GameScenes.FLIGHT) {
				Log.Info ("Not GameScenes.FLIGHT, calling Deactivate");
				instance.Deactivate (GameScenes.SPACECENTER);
			}
            
			// Deploy scene-specific modules, for GUI hijacking and similar logic
			switch (HighLogic.LoadedScene) {
			case GameScenes.FLIGHT: 
				if (!componentsLoaded && KRASH.instance.SimulationActive) 
				{
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
			SimulationNotification (SimulationActive);
		}

		// This is called when the script is destroyed.
		// This is honestly probably not necessary, but better safe than sorry
		void OnDestroy ()
		{
			SimulationNotification (false); 
			instance = null;
			cfg = null;
		}

		// Activates the Simulation. Returns the success of the activation.
		public  bool Activate ()
		{
			Log.Info ("Activate");
			// for recording save status, not sure what this string actually is tbh
			string save = null;

			// Make sure the instance actually exists. I can't imagine this ever failing, but NREs are bad.
			if (instance != null) {   
				// We create the pre-sim save.
				save = GamePersistence.SaveGame ("KRASHRevert", HighLogic.SaveFolder, SaveMode.OVERWRITE);

				// Mark the existing save as dirty.
				KRASH.instance.SimulationActive = true;

				// Record the scene we are coming from
				KRASHShelter.lastScene = HighLogic.LoadedScene;

				if (KRASHShelter.lastScene == GameScenes.EDITOR) {
//					HoloDeck.OnLeavingEditor (EditorDriver.editorFacility, EditorLogic.fetch.launchSiteName);
					KRASH.instance.OnLeavingEditor (EditorDriver.editorFacility, LaunchGUI.selectedSite);
				}
					
				// Start the tell-tale
				KRASH.instance.SimulationNotification (true);
			}
			HighLogic.CurrentGame.Parameters.Flight.CanQuickSave = false;
			HighLogic.CurrentGame.Parameters.Flight.CanQuickLoad = false;
			Log.Info ("Activate returning: " + (save != null ? true : false).ToString ());
			return save != null ? true : false;
		}

		public void DestroyModules()
		{
			Log.Info ("DestroyModules  unloading components");
			componentsLoaded = false;
			SimulationPauseMenu[] simulationPauseMenus = GameObject.FindObjectsOfType (typeof(SimulationPauseMenu)) as SimulationPauseMenu[];
			FlightModule[] flightModules = GameObject.FindObjectsOfType (typeof(FlightModule)) as FlightModule[];
//			EditorModule[] editorModules = GameObject.FindObjectsOfType (typeof(EditorModule)) as EditorModule[];


			foreach (SimulationPauseMenu simulationPauseMenu in simulationPauseMenus) {
				DestroyImmediate (simulationPauseMenu);
			}

			foreach (FlightModule flightModule in flightModules) {
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
			if (KRASH.instance.SimulationActive && System.IO.File.Exists (KSPUtil.ApplicationRootPath + "saves/" + HighLogic.SaveFolder + "/" + "KRASHRevert.sfs")) {
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
			KRASH.instance.SimulationActive = false;
		}

		// This method should be called before activating the simulation directly from an editor, and allows
		// QOL improvements (like returning to that editor correctly, and automatically clearing the launch site)
		// Either of these values can be null, if you want to do that for some reason
		public  void OnLeavingEditor (EditorFacility facility,  LaunchGUI.LaunchSite launchSite)
		{
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
		private void SimulationNotification (bool state)
		{
			if (HighLogic.LoadedScene == GameScenes.FLIGHT) {
				switch (state) {
				case true:
					SetSimActiveNotification ();
					InvokeRepeating ("DoSimulationNotification", 0.1f, 2.0f);
					break;

				case false:
					CancelInvoke ("DoSimulationNotification");
					break;
				}
			}
		}

		string simNotification; 

		public void SetSimActiveNotification()
		{
			simNotification = "Simulation Active";
		}
		public void SetSimNotification(string str)
		{
			simNotification = str;
		}
		private void DoSimulationNotification ()
		{
			ScreenMessages.PostScreenMessage (simNotification, 1.0f, ScreenMessageStyle.UPPER_CENTER);
		}
	}
}
