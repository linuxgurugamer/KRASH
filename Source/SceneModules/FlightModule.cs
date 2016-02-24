
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;

using UnityEngine;

namespace KRASH
{
	/*
     * This class contains all of the code for manipulating the GUI, and interacting with the game
     * for KRASH in GameScenes.FLIGHT.
     */
 
	class FlightModule : MonoBehaviour
	{
		private const string TAG = "KRASH.FlightModule";

		private SimulationPauseMenu menu = null;

		FlightModule ()
		{
			Log.Info ("FlightModule instantiated");
		}

		// Entry Point
		void Start ()
		{
			Log.Info ("FlightModule.Start");
			if (!menu)
				menu = this.GetComponent<SimulationPauseMenu> ();

		}

		void OnLevelWasLoaded (int i)
		{
			Log.Info ("OnLevelWasLoaded  in FlightModule.cs");
			if (KRASH.instance.SimulationActive && HighLogic.LoadedScene == GameScenes.FLIGHT) {
//				OrbitEditor.Simple (FlightGlobals.fetch.activeVessel.orbitDriver, 100000, LaunchGUI.selectedBody);
			}
		}


		/*
         * The basic idea here is that if we are Simming, destroy the vanilla pause menu as soon 
         * as it appears, and replace it with out own. 
         * 
         * If the pause menu is open, close it.
         * 
         * If the pause key is being pressed, check if our menu is active.
         * if our menu is not active, make it active.
         * if our menu is not active, tell it to reduce by one ui layer.
         */
//		int lastFrame = 0;
//		int lastUcnt = 0;

		void Update ()
		{
			//		Log.Info ("PauseMenu.isOpen: " + PauseMenu.isOpen.ToString ());
			// We don't want to do anything if we aren't simming
			if (KRASH.instance.SimulationActive) {
				if (FlightResultsDialog.isDisplaying) {
					FlightResultsDialog.showExitControls = false;
					FlightResultsDialog.Display ("Simulation ended in craft destruction!");
				}
				// Hide the vanilla pause menu.
				if (PauseMenu.isOpen) {
					PauseMenu.Close ();
				}

				// Check for pause keypress
				if (GameSettings.PAUSE.GetKeyDown ()) {
					Log.Info ("PAUSE.GetKeyDown");
					switch (menu.isOpen) {
					case false:
						Log.Info ("menu.Display");
						menu.Display ();
						break;
					case true:
						Log.Info ("menu.Close");
						menu.Close ();
						break;
					}
				}
			}
		}

		/*
         * When this module is destroyed, make sure to clean up our menu. 
         * It probably won't leak, but I'd rather not tempt fate.
         */
		public void OnDestroy ()
		{
			Log.Info ("FlightModule.Destroy");
			menu.Destroy ();
		}
	}

	/*
     * This menu is very similar to the squad PauseMenu.
     * 
     * isOpen - returns true if this menu is in an active state
     * 
     * Display() - makes this menu active
     * Close() - closes one ui layer. 
     */
	class SimulationPauseMenu : MonoBehaviour
	{
		private const float SPACER = 5.0f;
		private const string TAG = "KRASH.FlightModule.SimulationPauseMenu";

		private Rect _windowRect;
		private bool _display;
		private Color _backgroundColor;
		private GUISkin _guiSkin;
		private PopupDialog _activePopup;
		private MiniSettings _miniSettings;
//		private static int cnt = 0;
//		public int curCnt;
//		public static int ucnt = 0;

		public bool isOpen = false;

		public void Awake ()
		{
			Log.Info ("SimulationPauseMenu instantiated");
		}

		private void Start ()
		{
			Log.Info ("SimulationPauseMenu.Start");
			PauseMenu originalMenu = GameObject.FindObjectOfType (typeof(PauseMenu)) as PauseMenu;

			_backgroundColor = originalMenu.backgroundColor;
			_guiSkin = originalMenu.guiSkin;

			RenderingManager.AddToPostDrawQueue (3, new Callback (DrawGUI));
			_windowRect = new Rect ((float)(Screen.width / 2.0 - 125.0), (float)(Screen.height / 2.0 - 70.0), 250f, 130f);
		}


		//~SimulationPauseMenu ()
		void OnDestroy()
		{
			Log.Info ("~SimulationPauseMenu");
			Destroy ();
		}

		public void Display ()
		{
			isOpen = true;
			_display = true;
			InputLockManager.SetControlLock (ControlTypes.PAUSE, "KRASHSimPauseMenu");
			FlightDriver.SetPause (true);
		}

		public void Close ()
		{
			if (_activePopup != null) {
				Log.Info ("Close 1");
				_activePopup.Dismiss ();
				_activePopup = null;
				Unhide ();
			} else if (_miniSettings != null) {
				// WTF SQUAD?
				Log.Info ("Close 2");
				_miniSettings.GetType ().GetMethod ("Dismiss", BindingFlags.NonPublic | BindingFlags.Instance);
			} else {
				Log.Info ("Close 3");
				isOpen = false;
				_display = false;
				InputLockManager.RemoveControlLock ("KRASHSimPauseMenu");
				FlightDriver.SetPause (false);
			}            
		}

		public void Destroy ()
		{
			Log.Info ("SimulationPauseMenu.Destroy");
			RenderingManager.RemoveFromPostDrawQueue (3, new Callback (DrawGUI));
		}

		// Screw you, MiniSettings
		private void Hide ()
		{
			_display = false;
		}

		private void Unhide ()
		{
			_display = true;
		}

		private const int PHYSICSWAIT = 1;

		bool hyper = false;
		bool doLanding = false;
		int physicsCnt = 0;

		private void DrawGUI ()
		{
			if (KRASH.instance.SimulationActive) {
				if (!hyper && HighLogic.LoadedScene == GameScenes.FLIGHT) {
					//foreach (Part p in FlightGlobals.fetch.activeVessel.Parts) {
					//	p.
					//}
					if (LaunchGUI.simType == LaunchGUI.SimType.ORBITING) {
						if (!FlightGlobals.fetch.activeVessel.HoldPhysics && physicsCnt++ > PHYSICSWAIT) {
							Hyperedit.OrbitEditor.Simple (FlightGlobals.fetch.activeVessel.orbitDriver, LaunchGUI.newAltitude, LaunchGUI.selectedBody);
							hyper = true;
						}
					}

					if (LaunchGUI.simType == LaunchGUI.SimType.LANDED &&
					    ((!doLanding && !FlightGlobals.fetch.activeVessel.HoldPhysics) ||
					    (!hyper && doLanding && !FlightGlobals.fetch.activeVessel.HoldPhysics))) {
						Log.Info ("LANDED");
						if (!doLanding) {
							if (physicsCnt++ > PHYSICSWAIT) {
								Log.Info ("Setting initial orbit");
								Hyperedit.OrbitEditor.Simple (FlightGlobals.fetch.activeVessel.orbitDriver, 100000, LaunchGUI.selectedBody);
								hyper = false;
								physicsCnt = 0;
								doLanding = true;
							}
						} else if (!FlightGlobals.fetch.activeVessel.HoldPhysics && physicsCnt++ > PHYSICSWAIT) {
							Log.Info ("Setting landing");
							Action<double, double, CelestialBody> onManualEdit = (latVal, lonVal, body) => {
								latVal = LaunchGUI.newLatitude;
								lonVal = LaunchGUI.newLongitude;
								body = LaunchGUI.selectedBody;
							};
//							var pqs = LaunchGUI.selectedBody.pqsController;
//							var alt = pqs.GetSurfaceHeight (
//								          QuaternionD.AngleAxis (LaunchGUI.newLongitude, Vector3d.down) *
//								          QuaternionD.AngleAxis (LaunchGUI.newAltitude, Vector3d.forward) * Vector3d.right) -
//							          pqs.radius;
//							alt = Math.Max (alt, 0); // Underwater!
							Log.Info("body: " + LaunchGUI.selectedBody.name);
							Log.Info ("newAltitude: " + LaunchGUI.newAltitude.ToString ());
//							Log.Info ("alt: " + alt.ToString ());
							Hyperedit.DoLander.ToggleLanding (LaunchGUI.newLatitude, LaunchGUI.newLongitude, LaunchGUI.newAltitude, LaunchGUI.selectedBody, onManualEdit);
							hyper = true;
						}
					}
				}
			//	Log.Info ("_display: " + _display.ToString ());
				if (_display) {
					GUI.skin = _guiSkin;
					GUI.backgroundColor = _backgroundColor;
					_windowRect = GUILayout.Window (0, _windowRect, new GUI.WindowFunction (draw), "Game Paused", new GUILayoutOption[0]);
				}
			}
		}

		private void draw (int id)
		{
			if (GUILayout.Button ("Resume Simulation", _guiSkin.button)) {
				Close ();
			}

			GUILayout.Space (SPACER);

			if (GUILayout.Button ("<color=orange>Terminate Simulation</color>", _guiSkin.button)) {
				_activePopup = PopupDialog.SpawnPopupDialog (new MultiOptionDialog (null, new Callback (drawTerminationWarning), "Terminating Simulation", HighLogic.Skin, new DialogOption[0]), false, HighLogic.Skin);
				Hide ();
			}

			if (FlightDriver.CanRevertToPostInit) {
				if (GUILayout.Button ("<color=orange>Restart Simulation</color>", _guiSkin.button)) {
					_activePopup = PopupDialog.SpawnPopupDialog (new MultiOptionDialog (null, new Callback (drawRevertWarning), "Reverting Simulation", HighLogic.Skin, new DialogOption[0]), false, HighLogic.Skin);
					Hide ();
				}
			}

			GUILayout.Space (SPACER);

			if (GUILayout.Button ("Settings", _guiSkin.button)) {
				Hide ();
				_miniSettings = MiniSettings.Create (Unhide);
			}
		}

		private void drawRevertWarning ()
		{
			GUILayout.Label ("Reverting will set the game back to an earlier state. Are you sure you want to continue?");
			if (GUILayout.Button ("Revert to Launch (" + KSPUtil.PrintTime ((int)(Planetarium.GetUniversalTime () - FlightDriver.PostInitState.UniversalTime), 3, false) + " ago)")) {
				Close ();
				Close ();
				Log.Info ("FlightDriver.RevertToLaunch");
				FlightDriver.RevertToLaunch ();
				Log.Info ("Destroying parent");
				// The RevertTolaunch reloads all the objects, so we destroy them here to avoid conflicts
				KRASH.instance.DestroyModules ();
			}
			if (GUILayout.Button ("Cancel")) {
				Close ();
			}
		}

		private void drawTerminationWarning ()
		{
			string revertTarget;

			GUILayout.Label ("Terminating will set the game back to an earlier state. Are you sure you want to continue?");

			switch (KRASHShelter.lastScene) {
			case GameScenes.EDITOR:
				switch (KRASHShelter.lastEditor) {
				case EditorFacility.SPH:
					revertTarget = "Spaceplane Hangar";
					break;
				case EditorFacility.VAB:
					revertTarget = "Vehicle Assembly Building";
					break;
				// This should never happen. If it does, just go to the SC
				default:
					revertTarget = "Space Center";
					KRASHShelter.lastScene = GameScenes.SPACECENTER;
					break;
				}
				break;
			case GameScenes.SPACECENTER:
				revertTarget = "Space Center";
				break;
			default:
				revertTarget = "Pre-Simulation";
				break;
			}

			if (GUILayout.Button ("Revert to " + revertTarget + " (" + KSPUtil.PrintTime ((int)(Planetarium.GetUniversalTime () - FlightDriver.PostInitState.UniversalTime), 3, false) + " ago)")) {
				StartCoroutine (WaitForFlightResultsDialog ());
//                Close();
//                Close();
//				FlightResultsDialog.showExitControls = false;
//				FlightResultsDialog.Display ("Simulation results!");

//				HyperEditInterface.hideHyperEdit ();
//                KRASH.Deactivate(KRASHShelter.lastScene);
			}
            
			if (GUILayout.Button ("Cancel")) {
				Close ();
			}
		}

		public IEnumerator WaitForFlightResultsDialog ()
		{
			FlightResultsDialog.showExitControls = false;
			FlightResultsDialog.Display ("Simulation results!");

			while (FlightResultsDialog.isDisplaying)
				yield return 0;

			Close ();
			Close ();

			KRASH.instance.Deactivate (KRASHShelter.lastScene);
			Destroy ();
		}
			
	}
}
