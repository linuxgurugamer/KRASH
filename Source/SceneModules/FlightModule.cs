
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
//using System.Threading;
using KSP.UI.Screens;
using KSP.UI.Dialogs;
#if !RP_1_131
using ClickThroughFix;
#endif
namespace KRASH
{
    /*
     * This class contains all of the code for manipulating the GUI, and interacting with the game
     * for KRASH in GameScenes.FLIGHT.
     */
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    class FlightModule : MonoBehaviour
    {
        private const string TAG = "KRASH.FlightModule";
        public static AltimeterSliderButtons _Recovery_button = null;


        private void CallbackLevelWasLoaded(Scene scene, LoadSceneMode mode)
        {
            if (HighLogic.LoadedScene == GameScenes.FLIGHT)
            {

                Log.Info("CallbackLevelWasLoaded");
                //	KRASHShelter.instance.simPauseMenuInstance.AddToPostDrawQueue ();
            }

        }

        public void Awake()
        {
            Log.Info("FlightModule.Awake");

        }
        Camera nearCamera, farCamera, scaledSpaceCamera;
        Camera[] cams;

        // Entry Point
        void Start()
        {

            Log.Info("FlightModule.Start");
            if (KRASHShelter.instance.simPauseMenuInstance == null)
                Log.Info("KRASH.simPauseMenuInstance == null");

            GameObject t = GameObject.Find("SimulationPauseMenu");
            KRASHShelter.instance.simPauseMenuInstance = t.GetComponent<SimulationPauseMenu>();

            //		KRASHShelter.instance.simPauseMenuInstance = new SimulationPauseMenu();
            // KSP isn't calling Start for the simpausemenu, so we do it here
            //		KRASHShelter.instance.simPauseMenuInstance.Start ();
           // GameEvents.onLevelWasLoaded.Add(CallbackLevelWasLoaded);

            //	}
            //			GameEvents.onLevelWasLoaded.Add (CallbackLevelWasLoaded);
            // DontDestroyOnLoad (this);

            
        }

        void OnEnable()
        {
            //Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
            SceneManager.sceneLoaded += CallbackLevelWasLoaded;
        }

        void OnDisable()
        {
            //Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
            SceneManager.sceneLoaded -= CallbackLevelWasLoaded;
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

        int pauseCnt = 0;
        int closeCnt = 0;

        bool wireFrameAdded = false;


        void Update()
        {
            // We don't want to do anything if we aren't simming
            if (KRASHShelter.persistent.shelterSimulationActive)
            {
#if true
                if (HighLogic.CurrentGame.Parameters.CustomParams<KRASH_Settings>().wireframes)
                {
                    if (!wireFrameAdded)
                    {
                        wireFrameAdded = true;
                        Log.Info("Adding wireframe");
                        cams = Camera.allCameras;

                        for (int i = 0; i < cams.Length; i++)
                        {
                            if (cams[i].name == "Camera 00")
                            {
                                nearCamera = cams[i];
                            }
                            if (cams[i].name == "Camera 01")
                            {
                                //cams [i].renderingPath=RenderingPath.DeferredShading;
                                farCamera = cams[i];
                                //cams [i].enabled=false;

                            }
                            if (cams[i].name == "Camera ScaledSpace")
                            {
                                //cams [i].renderingPath=RenderingPath.DeferredShading;
                                scaledSpaceCamera = cams[i];
                            }
                        }
                        nearCamera.gameObject.AddComponent(typeof(WireFrame));
                        // farCamera.gameObject.AddComponent(typeof(WireFrame));
                        // scaledSpaceCamera.gameObject.AddComponent(typeof(WireFrame));
#if false
                        // Camera.main.gameObject.AddComponent(typeof(Wireframe));
                      
                    var v = FlightGlobals.ActiveVessel;

                    foreach (var p in v.parts)
                    {
                        
                        var mf = p.FindModelComponents<MeshFilter>();
                        var smr = p.FindModelComponents<SkinnedMeshRenderer>();

                        //  if (p.partTransform.GetComponent<MeshFilter>() || p.partTransform.GetComponent<SkinnedMeshRenderer>())
                        if ((mf != null && mf.Count > 0) || (smr != null && smr.Count > 0))
                        {
                            Log.Info("Wireframe added to part: " + p.partInfo.name);
                            // Add a WireFrame object to it.
                           // WireFrame added = p.gameObject.AddComponent<WireFrame>();

                          

                        }

                    }
#endif
                    }
#endif
                }
                // Don't allow any of the recovery buttons to be used
                //	AltimeterSliderButtons _Recovery_button = (AltimeterSliderButtons)GameObject.FindObjectOfType (typeof(AltimeterSliderButtons));
                if (_Recovery_button == null)
                {
                    _Recovery_button = (AltimeterSliderButtons)GameObject.FindObjectOfType(typeof(AltimeterSliderButtons));
                }
                if (_Recovery_button != null && _Recovery_button.slidingTab.enabled)
                {
                    _Recovery_button.hoverArea = new XSelectable();
                    _Recovery_button.slidingTab.enabled = false;
                    _Recovery_button.spaceCenterButton.enabled = false;
                    _Recovery_button.vesselRecoveryButton.enabled = false;
                    Log.Info("Recovery locked");
                }


                if (KRASHShelter.instance.simPauseMenuInstance == null)
                {
                    Log.Info("FlightModule.Update KRASH.simPauseMenuInstance == null");
                    return;
                }
                if (KRASHShelter.instance == null)
                {
                    Log.Info("FlightModule.Update KRASH.instance == null");
                    return;
                }
                //Log.Info ("jbb PauseMenu.isOpen: " + PauseMenu.isOpen.ToString ());
                //Log.Info ("KRASHShelter.instance.simPauseMenuInstance.isOpen: " + KRASHShelter.instance.simPauseMenuInstance.isOpen.ToString ());

                bool b;
                try
                {
                    b = FlightResultsDialog.isDisplaying;
                }
                catch (Exception)
                {
                    Log.Info("FlightResultsDialog.isDisplaying exception");
                    b = false;
                }
                if (b)
                {
                    FlightResultsDialog.showExitControls = false;
                    FlightResultsDialog.allowClosingDialog = true;
                    FlightResultsDialog.Display("Simulation ended!");
                }


                // Hide the vanilla pause menu.
                Log.Info("Checking PauseMenu");
                Log.Info("PauseMenu.isOpen: " + PauseMenu.isOpen.ToString());
                if (PauseMenu.isOpen)
                {
                    PauseMenu.Close();
                    pauseCnt = 1;
                }

                // Check for pause keypress
                //pauseCnt++;
                if (/* GameSettings.PAUSE.GetKeyDown() && */ pauseCnt != 0 && closeCnt == 0)
                {
                    Log.Info("GetKeyDown    menu.isOpen: " + KRASHShelter.instance.simPauseMenuInstance.isOpen);
                    if (GameSettings.PAUSE.GetKeyDown())
                    {
                        pauseCnt = 0;
                        Log.Info("GameSettings.PAUSE.GetKeyDown");
                    }
                    switch (KRASHShelter.instance.simPauseMenuInstance.isOpen)
                    {
                        case false:
                            if (pauseCnt == 1)
                            {
                                KRASHShelter.instance.simPauseMenuInstance.Display();
                                pauseCnt = 2;
                            }
                            break;
                        case true:
                            if (pauseCnt == 0)
                            {
                                Log.Info("menu.Close");
                                KRASHShelter.instance.simPauseMenuInstance.Close();
                                Log.Info("After close simPauseMenuInstance, PauseMenu.isOpen: " + PauseMenu.isOpen.ToString());
                                pauseCnt = 0;
                                closeCnt = 1;
                            }
                            break;
                    }
                }
                else
                {
                    // The wierd issue changed from 1.1.3 to 1.2, now, it pauses for 7-10 tics after unpausing
                    if (closeCnt > 0 && closeCnt++ < 20)
                    {
                        FlightDriver.SetPause(false);
                        PauseMenu.Close();
                        pauseCnt = 0;
                    }
                    else
                        closeCnt = 0;
                    // This is to get around a wierd issue where the game unpauses
                    // for about 7-10 tics after PauseMenu.Close() is called.
                    // if (KRASHShelter.instance.simPauseMenuInstance.isOpen && pauseCnt < 20 /*&& FlightDriver.Pause */)
                    //     FlightDriver.SetPause(true);
                }
            }
        }

        /*
         * When this module is destroyed, make sure to clean up our menu. 
         * It probably won't leak, but I'd rather not tempt fate.
         */
        public void OnDestroy()
        {
            Log.Info("FlightModule.OnDestroy");
            //if (KRASHShelter.instance.simPauseMenuInstance != null)
            //    KRASHShelter.instance.simPauseMenuInstance.OnDestroy ();
            //       if (Camera.main.gameObject.GetComponent(typeof(Wireframe)))
            //         Component.Destroy(Camera.main.gameObject.GetComponent(typeof(Wireframe)));
            var v = FlightGlobals.ActiveVessel;
            if (wireFrameAdded)
            {
#if false
                foreach (var p in v.parts)
                {
                    // Try to remove any WireFrame components found.
                    try
                    {
                        Destroy(p.partTransform.gameObject.GetComponent<WireFrame>());
                    }
                    catch { }
                }
#endif
                for (int i = 0; i < cams.Length; i++)
                {
                    if (cams[i].name == "Camera 00")
                    {
                        nearCamera = cams[i];
                        Destroy(nearCamera.gameObject.GetComponent<WireFrame>());
                        break;
                    }
                 
                }
                
            }
            KRASHShelter.instance.simPauseMenuInstance = null;
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
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class SimulationPauseMenu : MonoBehaviour
    {
        public static SimulationPauseMenu instance;
        private const float SPACER = 5.0f;
        private const string TAG = "KRASH.FlightModule.SimulationPauseMenu";

        private Rect _windowRect;
        //		private bool _display = false;
        private Color _backgroundColor;
        //		private GUISkin _guiSkin;
        private PopupDialog _activePopup;
        private MiniSettings _miniSettings;

        private bool simTermination = false;
        private static string simTerminationMsg = "";

        public bool isOpen = false;


        public void Awake()
        {
            Log.Info("SimulationPauseMenu.Awake");
            instance = this;
        }


        public void TerminateSimNoDialog()
        {
            StartCoroutine(WaitForFlightResultsDialog());
        }

        //        public void test(string s)
        //        {
        //            Log.Info("SimulationPauseMenu.test s: " + s);
        //        }

        double lastUpdate = 0.0F;

        void CalcStartingSimCost()
        {
            if (HighLogic.CurrentGame.Mode != Game.Modes.CAREER)
                return;
            Log.Info("SimulationPauseMenu.CalcStartingSimCost 1");
            if (KRASHShelter.instance == null || KRASHShelter.instance.cfg == null || FlightGlobals.ActiveVessel == null)
            {
                if (FlightGlobals.ActiveVessel == null)
                    Log.Info("FlightGlobals.ActiveVessel is null");
                else
                    Log.Info("SimulationPauseMenu.CalcStartingSimCost cfg == null");
                return;
            }
            if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER)
            {
                // float dryMass, fuelMass;

                //KRASHShelter.simCost = KRASHShelter.instance.cfg.flatSetupCost +
                //	EditorLogic.fetch.ship.parts.Count * KRASHShelter.instance.cfg.perPartSetupCost +
                //	(dryMass + fuelMass) * KRASHShelter.instance.cfg.perTonSetupCost +
                //	KRASHShelter.shipCost * KRASHShelter.instance.cfg.percentSetupCost;

                KRASHShelter.simSetupCost = KRASHShelter.instance.cfg.flatSetupCost +
                    FlightGlobals.ActiveVessel.parts.Count * KRASHShelter.instance.cfg.perPartSetupCost +
                    (KRASHShelter.dryMass + KRASHShelter.fuelMass) * KRASHShelter.instance.cfg.perTonSetupCost +
                    KRASHShelter.shipCost * KRASHShelter.instance.cfg.percentSetupCost;
                Log.Info("simSetupCost: " + KRASHShelter.simSetupCost.ToString());

                lastUpdate = Planetarium.GetUniversalTime();

                simX = KRASHShelter.instance.cfg.horizontalPos;
                simY = KRASHShelter.instance.cfg.verticalPos;
                Log.Info("simX: " + simX.ToString() + "   simY: " + simY.ToString());
                Log.Info("cnt: " + KRASHShelter.instance.cfg.cnt.ToString());
            }
        }

        public void Start()
        {
            Log.Info("SimulationPauseMenu Start");

            //KRASH.simPauseMenuInstance = this;

            PauseMenu originalMenu = GameObject.FindObjectOfType(typeof(PauseMenu)) as PauseMenu;


            CalcStartingSimCost();

            _backgroundColor = originalMenu.backgroundColor;
            //			_guiSkin = originalMenu.guiSkin;

            //AddToPostDrawQueue ();
            _windowRect = new Rect((float)(Screen.width / 2.0 - 125.0), (float)(Screen.height / 2.0 - 70.0), 250f, 130f);

            //DontDestroyOnLoad (this);  
            Log.Info("SimulationPauseMenu Start complete");
        }

        private void OnGUI()
        {
            DrawGUI();
        }

        //~SimulationPauseMenu ()
        public void OnDestroy()
        {
            Log.Info("SimulationPauseMenu OnDestroy");
            //Destroy ();
        }

        public void Display()
        {
            isOpen = true;
            //	_display = true;
            InputLockManager.SetControlLock(ControlTypes.PAUSE, "KRASHSimPauseMenu");
            Log.Info("Display:  FlightDriver.SetPause (true)");
            FlightDriver.SetPause(true);
            Log.Info("FlightDriver.Pause: " + FlightDriver.Pause.ToString());

        }

        public void Close()
        {
            if (_activePopup != null)
            {
                _activePopup.Dismiss();
                _activePopup = null;
                Unhide();
            }
            else if (_miniSettings != null)
            {
                // WTF SQUAD?
                _miniSettings.GetType().GetMethod("Dismiss", BindingFlags.NonPublic | BindingFlags.Instance);
            }
            else
            {

                isOpen = false;
                //_display = false;
                InputLockManager.RemoveControlLock("KRASHSimPauseMenu");
                Log.Info("Close:  FlightDriver.SetPause (false)");
                FlightDriver.SetPause(false);
                Log.Info("FlightDriver.Pause: " + FlightDriver.Pause.ToString());

            }
        }
#if false
		public void aDestroy ()
		{
			Log.Info ("SimulationPauseMenu Destroy");
		//	Log.Info ("RemoveFromPostDrawQueue DrawGUI");
		//	RenderingManager.RemoveFromPostDrawQueue (3, new Callback (DrawGUI));
		}
#endif
        // Screw you, MiniSettings
        private void Hide()
        {
            Log.Info("Hide");
            isOpen = false;
            //_display = false;
        }

        private void Unhide()
        {
            Log.Info("Unhide");
            isOpen = true;
            //_display = true;
        }

        private const int LEFT = 10;
        private const int TOP = 400;
        private const int WIDTH = 80;
        private const int HEIGHT = 50;
        //private Rect gametimePos = new Rect(10, 200, 80, 50);
        private Rect simInfoPos = new Rect(LEFT, TOP, WIDTH, HEIGHT);
        [Persistent]
        private GUIStyle simLabelStyle;
        [Persistent]
        int simSize = 10;
        [Persistent]
        //		float simX = KRASH.cfg.horizontalPos;
        float simX = 10;
        [Persistent]
        float simY = 50;
        //		float simY = KRASH.cfg.verticalPos;

        const string simTitle = "Sim Costs:";

        void DrawOutline(Rect r, string t, int strength, GUIStyle style, Color outColor, Color inColor)
        {
            Color backup = style.normal.textColor;
            style.normal.textColor = outColor;
            for (int i = -strength; i <= strength; i++)
            {
                GUI.Label(new Rect(r.x - strength, r.y + i, r.width, r.height), t, style);
                GUI.Label(new Rect(r.x + strength, r.y + i, r.width, r.height), t, style);
            }
            for (int i = -strength + 1; i <= strength - 1; i++)
            {
                GUI.Label(new Rect(r.x + i, r.y - strength, r.width, r.height), t, style);
                GUI.Label(new Rect(r.x + i, r.y + strength, r.width, r.height), t, style);
            }
            style.normal.textColor = inColor;
            GUI.Label(r, t, style);
            style.normal.textColor = backup;
        }


        void CalcAndDisplaySimCost()
        {
            if (!simStarted)
                return;
            //	Log.Info ("lastUpdate: " + lastUpdate.ToString () + "     Planetarium.GetUniversalTime: " + Planetarium.GetUniversalTime ().ToString ());
            if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER)
            {
                if (FlightGlobals.fetch.activeVessel == null)
                    return;

                float m = 1.0F;
                if (Planetarium.GetUniversalTime() - lastUpdate >= 1)
                {
                    // Determine atmospheric multiplier, if any
                    if (FlightGlobals.fetch.activeVessel.mainBody.atmosphere)
                    {
                        double alt = FlightGlobals.fetch.activeVessel.RevealAltitude();
                        if (alt <= FlightGlobals.fetch.activeVessel.mainBody.atmosphereDepth)
                        {
                            m = KRASHShelter.instance.cfg.AtmoMultipler;
                            if (m < 1.0F)
                                m = 1.0F;
                        }
                    }


                    lastUpdate = Planetarium.GetUniversalTime();
                    float mass = FlightGlobals.fetch.activeVessel.GetTotalMass();

                    List<Part> parts = FlightGlobals.fetch.activeVessel.GetActiveParts();
                    int cnt = parts.Count();

                    if (!HighLogic.CurrentGame.Parameters.CustomParams<KRASH_Settings>().noChargeDuringTimewarp ||
                        TimeWarp.CurrentRate == 1 ||
						TimeWarp.WarpMode == TimeWarp.Modes.LOW)
                    {
                        KRASHShelter.simCost = KRASHShelter.simCost +
                            (KRASHShelter.instance.cfg.flatPerMinCost +
                            cnt * KRASHShelter.instance.cfg.perPartPerMinCost +
                            mass * KRASHShelter.instance.cfg.perTonPerMinCost +
                            KRASHShelter.shipCost * KRASHShelter.instance.cfg.percentPerMinCost) / 60 * m;
                    }
                }
                Log.Info("Funding.Instance.Funds: " + Funding.Instance.Funds.ToString());
                if (Funding.Instance.Funds < KRASHShelter.simCost + KRASHShelter.simSetupCost)
                {
                    if (!KRASHShelter.instance.cfg.ContinueIfNoCash && !KRASHShelter.persistent.shelterSimulationActive && !HighLogic.CurrentGame.Parameters.CustomParams<GameParameters.AdvancedParams>().AllowNegativeCurrency)
                    {
                        KRASHShelter.simCost = Funding.Instance.Funds - KRASHShelter.simSetupCost;
                        Log.Info("Funding.Instance.Funds: " + Funding.Instance.Funds.ToString());
                        Log.Info("KRASHShelter.simCost: " + KRASHShelter.simCost.ToString());
                        Log.Info("KRASHShelter.simSetupCost: " + KRASHShelter.simSetupCost.ToString());
                        DisplayTerminationMessage("Simulation terminated due to lack of funds");
                    }
                }
                if (KRASHShelter.simCost + KRASHShelter.simSetupCost >= KRASHShelter.LimitSimCost && KRASHShelter.LimitSimCost > 0)
                {
                    KRASHShelter.simCost = KRASHShelter.LimitSimCost - KRASHShelter.simSetupCost;
                    DisplayTerminationMessage("Simulation terminated due to cost limit reached");
                }
                // Following code is for displaying the costs during flight
                if (simLabelStyle == null)
                {
                    simLabelStyle = new GUIStyle(GUI.skin.label);
                    simX = Mathf.Clamp(simX, 0, Screen.width);
                    simY = Mathf.Clamp(simY, 0, Screen.height);
                    simLabelStyle.fontSize = simSize;
                }

                // Check for atmospheric flight
                if (FlightGlobals.fetch.activeVessel.mainBody.atmosphere &&
                    FlightGlobals.fetch.activeVessel.RevealAltitude() <= FlightGlobals.fetch.activeVessel.mainBody.atmosphereDepth &&
                    !FlightGlobals.fetch.activeVessel.mainBody.isHomeWorld)
                {
                    foreach (PreSimStatus bodyStatus in KRASHShelter.preSimStatus)
                    {
                        if (bodyStatus.flightsGlobalIndex == FlightGlobals.fetch.activeVessel.mainBody.flightGlobalsIndex)
                        {
                            if (!bodyStatus.scienceFromAtmo)
                            {
                                Log.Info("vessel.RevealAltitude: " + FlightGlobals.fetch.activeVessel.RevealAltitude().ToString());
                                Log.Info("vessel.altitdue: " + FlightGlobals.fetch.activeVessel.altitude.ToString());
                                Log.Info("AtmosphereDepth: " + FlightGlobals.fetch.activeVessel.mainBody.atmosphereDepth.ToString());
                                DisplayTerminationMessage("Simulation terminated due to lack of science data from " +
                                FlightGlobals.fetch.activeVessel.mainBody.name + " atmosphere");
                            }
                        }
                    }
                }
                if (KRASHShelter.uiVisiblity.isVisible())
                {
                    simLabelStyle.fontSize = 20;
                    Vector2 size,
                    sizeTitle = simLabelStyle.CalcSize(new GUIContent(simTitle));
                    //if (config.displayGameTime && config.logGameTime) {
                    simInfoPos.Set(simX, simY, 200, sizeTitle.y);
                    DrawOutline(simInfoPos, simTitle, 1, simLabelStyle, Color.black, Color.yellow);

                    string costs = Math.Round(KRASHShelter.simCost + KRASHShelter.simSetupCost, 1).ToString();

                    //Log.Info("current thread id: " + Thread.CurrentThread.GetHashCode().ToString());
                    Log.Info("KRASHShelter.simCost: " + KRASHShelter.simCost.ToString());
                    Log.Info("KRASHShelter.simSetupCost: " + KRASHShelter.simSetupCost.ToString());

                    size = simLabelStyle.CalcSize(new GUIContent(costs));
                    simInfoPos.Set(simX + sizeTitle.x + 5, simY, 200, size.y);
                    DrawOutline(simInfoPos, costs, 1, simLabelStyle, Color.black, Color.white);
                    simInfoPos.Set(simInfoPos.xMin, simInfoPos.yMin + size.y, 200, size.y);
                    //}
                }
            }
        }

        // The following callback is called for several different scenerios.

        private void CallbackWillDestroy(Vessel evt)
        {
            Log.Info("CallbackWillDestroy");
            if (evt == FlightGlobals.ActiveVessel)
            {
                StartCoroutine(WaitForFlightResultsDialog(true));
                DisplayTerminationMessage("Simulation terminated due to vessel destruction");
            }

        }

        private void CallbackSOIChanged(GameEvents.HostedFromToAction<Vessel, CelestialBody> action)
        {
            if (KRASHShelter.instance.cfg.TerminateAtSoiWithoutData)
            {
                if (action.to != Sun.Instance.sun && !action.to.isHomeWorld)
                {
                    foreach (PreSimStatus bodyStatus in KRASHShelter.preSimStatus)
                    {
                        if (bodyStatus.flightsGlobalIndex == action.to.flightGlobalsIndex)
                        {
                            // if body hasn't been reached or no science from it, terminate the sim
                            if (!(action.to.isHomeWorld || bodyStatus.scienceFromOrbit))
                            {
                                DisplayTerminationMessage("Simulation terminated due to entering unknown SOI");
                            }
                        }
                    }
                }
            }
        }

        private void CallbackOnLand(Vessel vsl, CelestialBody destbody)
        {
            Log.Info("CallbackOnland");
            if (KRASHShelter.instance.cfg.TerminateAtLandWithoutData && !destbody.isHomeWorld)
            {
                foreach (PreSimStatus bodyStatus in KRASHShelter.preSimStatus)
                {
                    if (bodyStatus.flightsGlobalIndex == destbody.flightGlobalsIndex)
                    {
                        if (!bodyStatus.isReached || !bodyStatus.landed)
                        {
                            DisplayTerminationMessage("Simulation terminated due to landing on unknown body");
                        }
                    }
                }
            }
        }

        private const int PHYSICSWAIT = 1;

        bool hyper = false;
        bool doLanding = false;
        public bool simStarted = false;
        int physicsCnt = 0;
        //bool pausedtest = false;

        public bool SimStarted()
        {
            return simStarted;
        }
        //	int ticCntr = 0;
        private void DrawGUI()
        {
            //	ticCntr++;

            if (KRASHShelter.persistent.shelterSimulationActive)
            {
                if (!hyper && HighLogic.LoadedScene == GameScenes.FLIGHT)
                {
                    //foreach (Part p in FlightGlobals.fetch.activeVessel.Parts) {
                    //	p.
                    //}
                    if (LaunchGUI.simType == LaunchGUI.SimType.ORBITING)
                    {
                        if (!FlightGlobals.fetch.activeVessel.HoldPhysics && physicsCnt++ > PHYSICSWAIT)
                        {
                            Hyperedit.OrbitEditor.Simple(FlightGlobals.fetch.activeVessel.orbitDriver, LaunchGUI.newAltitude, LaunchGUI.selectedBody);
                            hyper = true;
                            physicsCnt = 0;
                        }
                    }

                    if (LaunchGUI.simType == LaunchGUI.SimType.LANDED &&
                        ((!doLanding && !FlightGlobals.fetch.activeVessel.HoldPhysics) ||
                        (!hyper && doLanding && !FlightGlobals.fetch.activeVessel.HoldPhysics)))
                    {
                        if (!doLanding)
                        {
                            if (physicsCnt++ > PHYSICSWAIT)
                            {
                                Log.Info("Setting initial orbit");
#if false
								KRASHShelter.originalUp = FlightGlobals.getUpAxis();
								KRASHShelter.originalHeading = Quaternion.LookRotation (FlightGlobals.fetch.activeVessel.GetWorldPos3D ());

								Log.Info ("DrawGUI KRASHShelter.originalUp: " + KRASHShelter.originalUp.ToString ());
#endif
                                Hyperedit.OrbitEditor.Simple(FlightGlobals.fetch.activeVessel.orbitDriver, 100000, LaunchGUI.selectedBody);
                                hyper = false;
                                physicsCnt = 0;
                                doLanding = true;
                            }
                        }
                        else if (!FlightGlobals.fetch.activeVessel.HoldPhysics && physicsCnt++ > PHYSICSWAIT * 1)
                        {
                            Action<double, double, CelestialBody> onManualEdit = (latVal, lonVal, body) =>
                            {
                                if (LaunchGUI.newLatitude == 0.0f)
                                    LaunchGUI.newLatitude = 0.001;
                                if (LaunchGUI.newLongitude == 0.0f)
                                    LaunchGUI.newLongitude = 0.001;
                                latVal = LaunchGUI.newLatitude;
                                lonVal = LaunchGUI.newLongitude;
                                body = LaunchGUI.selectedBody;
                                Log.Info("latVal: " + latVal.ToString() + "    lonVal: " + lonVal.ToString());
                            };
                            Log.Info("Setting landing");
                            if (LaunchGUI.newLatitude == 0.0f)
                                LaunchGUI.newLatitude = 0.001;
                            if (LaunchGUI.newLongitude == 0.0f)
                                LaunchGUI.newLongitude = 0.001;
                            Log.Info("LaunchGUI.newLatitude: " + LaunchGUI.newLatitude.ToString() + "   LaunchGUI.newLongitude: " + LaunchGUI.newLongitude.ToString());
                            Hyperedit.DoLander.ToggleLanding(LaunchGUI.newLatitude, LaunchGUI.newLongitude, LaunchGUI.newAltitude, LaunchGUI.selectedBody, onManualEdit);
                            hyper = true;
                        }
                    }
                }

                if (!simStarted && LaunchGUI.selectedBody.flightGlobalsIndex == Planetarium.fetch.CurrentMainBody.flightGlobalsIndex)
                {
                    simStarted = true;
                    APIManager.ApiInstance.SimStartEvent.Fire((Vessel)FlightGlobals.ActiveVessel, 0.0f);
                    if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER)
                    {
                        GameEvents.onVesselWillDestroy.Add(this.CallbackWillDestroy);
                        GameEvents.onVesselSOIChanged.Add(this.CallbackSOIChanged);
                        GameEvents.VesselSituation.onLand.Add(this.CallbackOnLand);
                    }
                }


                //				if (simStarted && !pausedtest){
                //					pausedtest = true;
                //					PauseMenu.Close ();
                //					FlightDriver.SetPause (true);
                //				}
                CalcAndDisplaySimCost();
                Log.Info("isOpen: " + isOpen.ToString() + "  FlightResultsDialog.isDisplaying: " + FlightResultsDialog.isDisplaying.ToString());
                if (isOpen && !FlightResultsDialog.isDisplaying)
                {
                    //					Log.Info ("Displaying draw window");
                    //					GUI.skin = _guiSkin;
                    GUI.backgroundColor = _backgroundColor;
#if RP_1_131
                    _windowRect = GUILayout.Window(this.GetInstanceID() + 1, _windowRect, new GUI.WindowFunction(draw), "Game Paused", new GUILayoutOption[0]);
#else
                    _windowRect = ClickThruBlocker.GUILayoutWindow(this.GetInstanceID() + 1, _windowRect, new GUI.WindowFunction(draw), "Game Paused", new GUILayoutOption[0]);
#endif
                }
            }
        }

        private void draw(int id)
        {
            if (!simTermination)
            {
                if (GUILayout.Button("Resume Simulation"/*, _guiSkin.button */))
                {
                    Log.Info("Close 1");
                    Close();
                }

                GUILayout.Space(SPACER);
            }
            if (GUILayout.Button("<color=orange>Terminate Simulation</color>"/*, _guiSkin.button */))
            {
                string revertTarget;

                switch (KRASHShelter.lastScene)
                {
                    case GameScenes.EDITOR:
                        switch (KRASHShelter.lastEditor)
                        {
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
                string s = "Revert to " + revertTarget + " (" + KSPUtil.PrintTimeLong(((int)(Planetarium.GetUniversalTime() - FlightDriver.PostInitState.UniversalTime)));
                DialogGUIBase[] options = new DialogGUIBase[2];

                options[0] = new DialogGUIButton(s, () =>
                {
                    StartCoroutine(WaitForFlightResultsDialog());
                });
                options[1] = new DialogGUIButton("Cancel", () =>
                {
                    Log.Info("Close 2");

                    Close();
                });
                var multidialog = new MultiOptionDialog("krash2", "Terminating will set the game back to an earlier state. Are you sure you want to continue?", "Terminating Simulation",
                                      HighLogic.UISkin, 450, options);

                _activePopup = PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), multidialog, false, HighLogic.UISkin, true);
                Hide();
            }


            if (FlightDriver.CanRevertToPostInit)
            {

                if (GUILayout.Button("<color=orange>Restart Simulation</color>"/*, _guiSkin.button*/))
                {

                    DialogGUIBase[] options = new DialogGUIBase[2];
                    string s = "Revert to Launch(" + KSPUtil.PrintTimeLong(((int)(Planetarium.GetUniversalTime() - FlightDriver.PostInitState.UniversalTime))) + " ago";
                    options[0] = new DialogGUIButton(s, () =>
                    {
                        Log.Info("Close 3");

                        Close();
                        Log.Info("Close 4");

                        Close();
                        FlightDriver.RevertToLaunch();
                        // The RevertTolaunch reloads all the objects, so we destroy them here to avoid conflicts
                        KRASHShelter.instance.DestroyModules();
                    });
                    options[1] = new DialogGUIButton("Cancel", () =>
                    {
                        Log.Info("Close 5");

                        Close();
                    });


                    var multidialog = new MultiOptionDialog("krash3", "Reverting will set the game back to an earlier state. Are you sure you want to continue?", "Reverting Simulation",
                                          HighLogic.UISkin, 450, options);




                    _activePopup = PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), multidialog, false, HighLogic.UISkin, true);

                    //						_activePopup = PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                    //							new MultiOptionDialog(null, new Callback (drawRevertWarning), "Reverting Simulation", HighLogic.Skin, new DialogOption[0]), false, HighLogic.Skin);
                    APIManager.ApiInstance.SimRestartEvent.Fire((Vessel)FlightGlobals.ActiveVessel, KRASHShelter.simCost + KRASHShelter.simSetupCost);
                    Hide();
                }
            }

            GUILayout.Space(SPACER);

            if (GUILayout.Button("Settings"/*, _guiSkin.button*/))
            {
                Hide();
                _miniSettings = MiniSettings.Create(Unhide);
            }
#if false
			} else {
				

				GUILayout.Label (simTerminationMsg);
				if (GUILayout.Button ("<color=orange>Terminate Simulation</color>"/*, _guiSkin.button*/)) {
					Hide ();
					StartCoroutine (WaitForFlightResultsDialog ());

//					_activePopup = PopupDialog.SpawnPopupDialog (new MultiOptionDialog (null, new Callback (drawTerminationWarning), "Terminating Simulation", HighLogic.Skin, new DialogOption[0]), false, HighLogic.Skin);

				}
			}
#endif
            GUI.DragWindow();
        }


        public void DisplayTerminationMessage(string msg)
        {
            if (!simTermination)
            {
                simTermination = true;
                simTerminationMsg = msg;
                //                KRASHShelter.persistent.SetSuspendUpdate(false);
                Display();
            }
        }


        bool returnedAfterDialog = false;

        public IEnumerator WaitForFlightResultsDialog(bool returnAfterDialog = false)
        {
            Log.Info("IEnumerator WaitForFlightResultsDialog");

            if (!returnedAfterDialog)
            {
                FlightResultsDialog.showExitControls = false;
                FlightResultsDialog.allowClosingDialog = true;

                FlightResultsDialog.Display("Simulation results!");

                bool b = true;

                while (b)
                {
                    try
                    {
                        b = FlightResultsDialog.isDisplaying;
                    }
                    catch (Exception)
                    {
                        Log.Info("FlightResultsDialog.isDisplaying exception");
                        b = false;
                    }
                    if (b)
                        yield return 0;
                }
            }
            returnedAfterDialog = returnAfterDialog;
            if (returnAfterDialog)
                yield break;

            if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER)
            {
                GameEvents.onVesselSOIChanged.Remove(this.CallbackSOIChanged);
                GameEvents.onVesselWillDestroy.Remove(this.CallbackWillDestroy);
                GameEvents.VesselSituation.onLand.Remove(this.CallbackOnLand);
            }
            Log.Info("FlightModule.WaitForFlightResultsDialog, before SimulationNotification");
            KRASHShelter.instance.SimulationNotification(false);

            HighLogic.CurrentGame.Parameters.Flight.CanQuickSave = true;
            HighLogic.CurrentGame.Parameters.Flight.CanQuickLoad = true;

            Close();
            Close();

            APIManager.ApiInstance.SimTerminationEvent.Fire((Vessel)FlightGlobals.ActiveVessel, KRASHShelter.simCost + KRASHShelter.simSetupCost);

            KRASHShelter.instance.Deactivate(KRASHShelter.lastScene);

            //OnDestroy ();

            yield return 0;
        }
    }
}
