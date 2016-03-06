
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

		}

		// Entry Point
		void Start ()
		{
			if (!menu)
				menu = this.GetComponent<SimulationPauseMenu> ();
		}

		#if false
		void OnLevelWasLoaded (int i)
		{
			//Log.Info ("OnLevelWasLoaded  in FlightModule.cs");
			if (KRASH.instance.SimulationActive && HighLogic.LoadedScene == GameScenes.FLIGHT) {
//				OrbitEditor.Simple (FlightGlobals.fetch.activeVessel.orbitDriver, 100000, LaunchGUI.selectedBody);
			}
		}
		#endif

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
					switch (menu.isOpen) {
					case false:
						menu.Display ();
						break;
					case true:
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

		private bool simTermination = false;
		private static string simTerminationMsg = "";

		public bool isOpen = false;

		public void Awake ()
		{

		}

		double lastUpdate = 0.0F;

		void CalcStartingSimCost ()
		{
			if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER) {
				float dryMass, fuelMass;
				EditorLogic.fetch.ship.GetShipMass (out dryMass, out fuelMass);

				KRASHShelter.simCost = KRASH.cfg.flatSetupCost +
					EditorLogic.fetch.ship.parts.Count * KRASH.cfg.perPartSetupCost +
					(dryMass + fuelMass) * KRASH.cfg.perTonSetupCost;

				lastUpdate = Planetarium.GetUniversalTime ();

			}
		}

		private void Start ()
		{
			KRASH.simPauseMenuInstance = this;
			PauseMenu originalMenu = GameObject.FindObjectOfType (typeof(PauseMenu)) as PauseMenu;

			CalcStartingSimCost ();

			_backgroundColor = originalMenu.backgroundColor;
			_guiSkin = originalMenu.guiSkin;

			RenderingManager.AddToPostDrawQueue (3, new Callback (DrawGUI));
			_windowRect = new Rect ((float)(Screen.width / 2.0 - 125.0), (float)(Screen.height / 2.0 - 70.0), 250f, 130f);
		}


		//~SimulationPauseMenu ()
		void OnDestroy ()
		{
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
				_activePopup.Dismiss ();
				_activePopup = null;
				Unhide ();
			} else if (_miniSettings != null) {
				// WTF SQUAD?
				_miniSettings.GetType ().GetMethod ("Dismiss", BindingFlags.NonPublic | BindingFlags.Instance);
			} else {
				isOpen = false;
				_display = false;
				InputLockManager.RemoveControlLock ("KRASHSimPauseMenu");
				FlightDriver.SetPause (false);
			}            
		}

		public void Destroy ()
		{
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

		private const int LEFT = 10;
		private const int TOP = 400;
		private const int WIDTH = 80;
		private const int HEIGHT = 50;
		//private Rect gametimePos = new Rect(10, 200, 80, 50);
		private Rect simInfoPos = new Rect (LEFT, TOP, WIDTH, HEIGHT);
		[Persistent]
		private GUIStyle simLabelStyle;
		[Persistent]
		int simSize = 10;
		[Persistent]
		float simX = 10;
		[Persistent]
		float simY = 200;

		const string simTitle = "Sim Costs:";

		void DrawOutline (Rect r, string t, int strength, GUIStyle style, Color outColor, Color inColor)
		{
			Color backup = style.normal.textColor;
			style.normal.textColor = outColor;
			for (int i = -strength; i <= strength; i++) {
				GUI.Label (new Rect (r.x - strength, r.y + i, r.width, r.height), t, style);
				GUI.Label (new Rect (r.x + strength, r.y + i, r.width, r.height), t, style);
			}
			for (int i = -strength + 1; i <= strength - 1; i++) {
				GUI.Label (new Rect (r.x + i, r.y - strength, r.width, r.height), t, style);
				GUI.Label (new Rect (r.x + i, r.y + strength, r.width, r.height), t, style);
			}
			style.normal.textColor = inColor;
			GUI.Label (r, t, style);
			style.normal.textColor = backup;
		}



		void CalcAndDisplaySimCost ()
		{
			if (!simStarted)
				return;
		//	Log.Info ("lastUpdate: " + lastUpdate.ToString () + "     Planetarium.GetUniversalTime: " + Planetarium.GetUniversalTime ().ToString ());
			if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER) {
				float m = 1.0F;
				if (Planetarium.GetUniversalTime () - lastUpdate >= 1) {
					// Determine atmospheric multiplier, if any
					if (FlightGlobals.fetch.activeVessel.mainBody.atmosphere) {
						double alt = FlightGlobals.fetch.activeVessel.RevealAltitude ();
						if (alt <= FlightGlobals.fetch.activeVessel.mainBody.atmosphereDepth) {
							m = KRASH.cfg.AtmoMultipler;
							if (m < 1.0F)
								m = 1.0F;
						}
					}

					lastUpdate = Planetarium.GetUniversalTime ();
					float mass = FlightGlobals.fetch.activeVessel.GetTotalMass ();

					List<Part> parts = FlightGlobals.fetch.activeVessel.GetActiveParts ();
					int cnt = parts.Count ();

					KRASHShelter.simCost = KRASHShelter.simCost +
					(KRASH.cfg.flatPerMinCost +
					cnt * KRASH.cfg.perPartPerMinCost +
					mass * KRASH.cfg.perTonPerMinCost) / 60 * m;

					// Log.Info ("CalcAndDisplaySimCost: " + KRASH.instance.simCost.ToString ());
				}

				if (Funding.Instance.Funds < KRASHShelter.simCost) {
					// First make sure that funds won't go negative
					KRASHShelter.simCost = Funding.Instance.Funds;
					if (!KRASH.cfg.ContinueIfNoCash) {
						DisplayTerminationMessage ("Simulation terminated due to lack of funds");
					}
				} 
				if (KRASHShelter.simCost >= KRASHShelter.LimitSimCost && KRASHShelter.LimitSimCost > 0 ) {
					KRASHShelter.simCost = KRASHShelter.LimitSimCost;
					DisplayTerminationMessage ("Simulation terminated due to cost limit reached");
				}
				// Following code is for displaying the costs during flight
				if (simLabelStyle == null) {
					simLabelStyle = new GUIStyle (GUI.skin.label);
					simX = Mathf.Clamp (simX, 0, Screen.width);
					simY = Mathf.Clamp (simY, 0, Screen.height);
					simLabelStyle.fontSize = simSize;
				}

				// Check for atmospheric flight
				if (FlightGlobals.fetch.activeVessel.mainBody.atmosphere &&
				    FlightGlobals.fetch.activeVessel.RevealAltitude () <= FlightGlobals.fetch.activeVessel.mainBody.atmosphereDepth &&
				    !FlightGlobals.fetch.activeVessel.mainBody.isHomeWorld) {
					foreach (PreSimStatus bodyStatus in KRASHShelter.preSimStatus) {
						if (bodyStatus.flightsGlobalIndex == FlightGlobals.fetch.activeVessel.mainBody.flightGlobalsIndex) {
							if (!bodyStatus.scienceFromAtmo) {
								Log.Info ("vessel.RevealAltitude: " + FlightGlobals.fetch.activeVessel.RevealAltitude ().ToString());
								Log.Info ("vessel.altitdue: " + FlightGlobals.fetch.activeVessel.altitude.ToString ());
								Log.Info ("AtmosphereDepth: " + FlightGlobals.fetch.activeVessel.mainBody.atmosphereDepth.ToString ());
								DisplayTerminationMessage ("Simulation terminated due to lack of science data from " + 
									FlightGlobals.fetch.activeVessel.mainBody.name + " atmosphere");
							}
						}
					}
				}


				Vector2 size, 
				sizeTitle = simLabelStyle.CalcSize (new GUIContent (simTitle));
				//if (config.displayGameTime && config.logGameTime) {
				simInfoPos.Set (simX, simY, 200, sizeTitle.y);
				DrawOutline (simInfoPos, simTitle, 1, simLabelStyle, Color.black, Color.yellow);

				string costs = Math.Floor (KRASHShelter.simCost).ToString ();

				size = simLabelStyle.CalcSize (new GUIContent (costs));
				simInfoPos.Set (simX + sizeTitle.x + 5, simY, 200, size.y);
				DrawOutline (simInfoPos, costs, 1, simLabelStyle, Color.black, Color.white);
				simInfoPos.Set (simInfoPos.xMin, simInfoPos.yMin + size.y, 200, size.y);
				//}
			}
		}

		// The following callback is called for several different scenerios.

		private void CallbackWillDestroy (Vessel evt)
		{
			Log.Info ("CallbackWillDestroy");
			DisplayTerminationMessage ("Simulation terminated due to vessel destruction");
						
		}

		private void CallbackSOIChanged (GameEvents.HostedFromToAction<Vessel, CelestialBody >  action)
		{
			if (KRASH.cfg.TerminateAtSoiWithoutData) {
				if (action.to != Sun.Instance.sun && !action.to.isHomeWorld) {
					foreach (PreSimStatus bodyStatus in KRASHShelter.preSimStatus) {
						if (bodyStatus.flightsGlobalIndex == action.to.flightGlobalsIndex) {
							// if body hasn't been reached or no science from it, terminate the sim
							if (!(action.to.isHomeWorld || bodyStatus.scienceFromOrbit)) {
								DisplayTerminationMessage ("Simulation terminated due to entering unknown SOI");
							}
						}
					}
				}
			}
		}

		private void CallbackOnLand (Vessel vsl, CelestialBody destbody)
		{
			Log.Info ("CallbackOnland");
			if (KRASH.cfg.TerminateAtLandWithoutData && !destbody.isHomeWorld) {
				foreach (PreSimStatus bodyStatus in KRASHShelter.preSimStatus) {
						if (bodyStatus.flightsGlobalIndex == destbody.flightGlobalsIndex) {
							if (!bodyStatus.isReached || !bodyStatus.landed) {
								DisplayTerminationMessage ("Simulation terminated due to landing on unknown body");
						}
					}
				}
			}
		}

		private const int PHYSICSWAIT = 1;

		bool hyper = false;
		bool doLanding = false;
		bool simStarted = false;
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
							physicsCnt = 0;
						}
					}

					if (LaunchGUI.simType == LaunchGUI.SimType.LANDED &&
					    ((!doLanding && !FlightGlobals.fetch.activeVessel.HoldPhysics) ||
					    (!hyper && doLanding && !FlightGlobals.fetch.activeVessel.HoldPhysics))) {
						if (!doLanding) {
							if (physicsCnt++ > PHYSICSWAIT) {
								Log.Info ("Setting initial orbit");
								Hyperedit.OrbitEditor.Simple (FlightGlobals.fetch.activeVessel.orbitDriver, 100000, LaunchGUI.selectedBody);
								hyper = false;
								physicsCnt = 0;
								doLanding = true;
							}
						} else if (!FlightGlobals.fetch.activeVessel.HoldPhysics && physicsCnt++ > PHYSICSWAIT) {
							Action<double, double, CelestialBody> onManualEdit = (latVal, lonVal, body) => {
								latVal = LaunchGUI.newLatitude;
								lonVal = LaunchGUI.newLongitude;
								body = LaunchGUI.selectedBody;
							};

							Hyperedit.DoLander.ToggleLanding (LaunchGUI.newLatitude, LaunchGUI.newLongitude, LaunchGUI.newAltitude, LaunchGUI.selectedBody, onManualEdit);
							hyper = true;
						}
					}
				}

				if ( !simStarted && LaunchGUI.selectedBody.flightGlobalsIndex == Planetarium.fetch.CurrentMainBody.flightGlobalsIndex) {
					simStarted = true;
					APIManager.ApiInstance.SimStartEvent.Fire ((Vessel)FlightGlobals.ActiveVessel, 0.0f);
					if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER) {
						GameEvents.onVesselWillDestroy.Add (this.CallbackWillDestroy);
						GameEvents.onVesselSOIChanged.Add (this.CallbackSOIChanged);
						GameEvents.VesselSituation.onLand.Add (this.CallbackOnLand);
					}
				}
				CalcAndDisplaySimCost ();

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
			if (!simTermination) {
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
						APIManager.ApiInstance.SimRestartEvent.Fire ((Vessel)FlightGlobals.ActiveVessel, KRASHShelter.simCost);
						Hide ();
					}
				}

				GUILayout.Space (SPACER);

				if (GUILayout.Button ("Settings", _guiSkin.button)) {
					Hide ();
					_miniSettings = MiniSettings.Create (Unhide);
				}
			} else {
				

				GUILayout.Label (simTerminationMsg);
				if (GUILayout.Button ("<color=orange>Terminate Simulation</color>", _guiSkin.button)) {
					Hide ();
					StartCoroutine (WaitForFlightResultsDialog ());

//					_activePopup = PopupDialog.SpawnPopupDialog (new MultiOptionDialog (null, new Callback (drawTerminationWarning), "Terminating Simulation", HighLogic.Skin, new DialogOption[0]), false, HighLogic.Skin);

				}
			}
		}

		private void drawRevertWarning ()
		{
			GUILayout.Label ("Reverting will set the game back to an earlier state. Are you sure you want to continue?");
			if (GUILayout.Button ("Revert to Launch (" + KSPUtil.PrintTime ((int)(Planetarium.GetUniversalTime () - FlightDriver.PostInitState.UniversalTime), 3, false) + " ago)")) {
				Close ();
				Close ();
				FlightDriver.RevertToLaunch ();
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
				//	GameEvents.onVesselSOIChanged.Remove (this.CallbackSOIChanged);
				StartCoroutine (WaitForFlightResultsDialog ());
			}
            
			if (GUILayout.Button ("Cancel")) {
				Close ();
			}
		}

		public void   DisplayTerminationMessage (string msg)
		{
			if (!simTermination) {
				simTermination = true;
				simTerminationMsg = msg;

				Display ();
			}
		}

		public IEnumerator WaitForFlightResultsDialog ()
		{ 
			if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER) {
				GameEvents.onVesselSOIChanged.Remove (this.CallbackSOIChanged);
				GameEvents.onVesselWillDestroy.Remove (this.CallbackWillDestroy);
				GameEvents.VesselSituation.onLand.Remove (this.CallbackOnLand);
			}
			HighLogic.CurrentGame.Parameters.Flight.CanQuickSave = true;
			HighLogic.CurrentGame.Parameters.Flight.CanQuickLoad = true;
			FlightResultsDialog.showExitControls = false;

			FlightResultsDialog.Display ("Simulation results!");

			while (FlightResultsDialog.isDisplaying)
				yield return 0;
			Close ();
			Close ();
			APIManager.ApiInstance.SimTerminationEvent.Fire ((Vessel)FlightGlobals.ActiveVessel, KRASHShelter.simCost);
			KRASH.instance.Deactivate (KRASHShelter.lastScene);
			Destroy ();

			yield return 0;
		}
	}
}
