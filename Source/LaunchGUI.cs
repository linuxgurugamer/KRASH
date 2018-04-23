using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using KSP.UI;
using KSP.UI.Screens;
using Upgradeables;
using ClickThroughFix;
using ToolbarControl_NS;

namespace KRASH
{
    //	[KSPAddon (KSPAddon.Startup.EditorAny, false)]
    [KSPAddon(KSPAddon.Startup.SpaceCentre, true)]
    public class LaunchGUI : MonoBehaviour
    {
        // Following code copied from KerbalKonstructs
        public enum SiteType
        {
            VAB,
            SPH,
            Any
        }

        enum SelectionType
        {
            launchsites,
            celestialbodies
        }

        public const String TITLE = "KRASH";
        //private const int WIDTH = 725;
        //private const int HEIGHT = 425;
        //private Rect bounds = new Rect (Screen.width / 2 - WIDTH / 2, Screen.height / 2 - HEIGHT / 2, WIDTH, HEIGHT);
        private bool visible = false;
        //private static ApplicationLauncherButton button = null;
       

        bool limitMaxCosts = false;
        public static LaunchGUI LaunchGuiInstance = null;

        const string texPathDefault = "KRASH/Textures/KRASH";

        public static LaunchSite selectedSite = null;
        static SelectionType selectType = SelectionType.launchsites;
        static public CelestialBody selectedBody; // = FlightGlobals.Bodies.Where (cb => cb.isHomeWorld).FirstOrDefault ();
        List<CelestialBody> bodiesList;

        //const string texPathOn = "KRASH/Textures/AppLauncherIcon-On";
        //const string texPathOff = "KRASH/Textures/AppLauncherIcon-Off";


        LaunchGUI()
        {
            Log.Info("LaunchGUI instantiated");
        }

        public void Start()
        {
            Log.Info("LaunchGUI.Start");
            AddToolbarButtons();
#if false
            if (LaunchGUI.button == null)
            {
                OnGuiAppLauncherReady();
            }
#endif
            DontDestroyOnLoad(this);

        }

        public void Awake()
        {
            Log.Info("LaunchGUI.Awake");
            if (LaunchGUI.LaunchGuiInstance == null)
            {
#if false
                GameEvents.onGUIApplicationLauncherReady.Add(this.OnGuiAppLauncherReady);
#endif
                AddToolbarButtons();
                LaunchGuiInstance = this;
            }

        }

        private void OnDestroy()
        {
            Log.Info("LaunchGUI.OnDestroy");
            //			GameEvents.onGUIApplicationLauncherReady.Remove (this.OnGuiAppLauncherReady);
            //			if (this.button != null) {
            //				ApplicationLauncher.Instance.RemoveModApplication (this.button);
            //			}
        }

        private void ButtonState(bool state)
        {
            //			Log.Info ("ApplicationLauncher on" + state.ToString ());

        }
#if false
        private void OnGuiAppLauncherReady()
        {
            if (LaunchGUI.button == null)
            {
                try
                {
                    LaunchGUI.button = ApplicationLauncher.Instance.AddModApplication(
                        GUIButtonToggle, GUIButtonToggle,
                        null, null,
                        null, null,
                        ApplicationLauncher.AppScenes.SPACECENTER | ApplicationLauncher.AppScenes.VAB | ApplicationLauncher.AppScenes.SPH, //visibleInScenes
                        GameDatabase.Instance.GetTexture(texPathDefault, false) //texture
                    );
                }
                catch (Exception ex)
                {
                    Log.Error("Error adding ApplicationLauncher button: " + ex.Message);
                }
            }
        }
#endif

        static internal ToolbarControl toolbarControl = null;
        void AddToolbarButtons()
        {
            if (toolbarControl == null)
            {
                toolbarControl = gameObject.AddComponent<ToolbarControl>();
                toolbarControl.AddToAllToolbars(GUIButtonToggle, GUIButtonToggle,
                    ApplicationLauncher.AppScenes.SPACECENTER | ApplicationLauncher.AppScenes.VAB | ApplicationLauncher.AppScenes.SPH,
                    "KRASH_NS",
                    "krashButton",
                    "KRASH/Textures/KRASH",
                    "KRASH/Textures/KRASH_24",
                    "KRASH"
                );
                toolbarControl.UseBlizzy(HighLogic.CurrentGame.Parameters.CustomParams<KRASH_Settings>().useBlizzy);
            }
        }


        //
        // The following function (only) is from KerbalEngineer
        // and is covered by the GPL V3 (license included with mod)
        //
        private void Update()
        {
            try
            {
                if (LaunchGUI.toolbarControl == null)
                {
                    return;
                }

                if (HighLogic.LoadedScene == GameScenes.SPACECENTER || EditorLogic.RootPart != null)
                {
#if false
                    if (LaunchGUI.button.enabled == false)
                    {
                        Log.Info("Enabling button");
                        LaunchGUI.button.Enable();
                        //		LaunchGUI.button.SetTrue();
                        LaunchGUI.button.enabled = true;
                    }
#endif
                    if (toolbarControl.Enabled == false)
                    {
                        toolbarControl.Enabled = true;
                    }
                }
                else if (toolbarControl.Enabled)
                {
                    Log.Info("Disabling button");
#if false
                    LaunchGUI.button.Disable();
                    //		LaunchGUI.button.SetFalse();
                    LaunchGUI.button.enabled = false;
#endif
                    toolbarControl.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Log.Info("BuildToolbar->Update: " + ex);
            }
        }
        //
        // End of function from KER
        //


        public bool Visible()
        {
            return this.visible;
        }

        public void SetVisible(bool visible)
        {

            this.visible = visible;
        }

        //bool isEditorLocked = false;
        private void EditorLock(bool state, string from="")
        {
            return;
            // This isn't needed since the ClickThroughBlocker is taking care of everything
#if false
            //Log.Info("EditorLock: state: " + state.ToString());
            if (state)
            {
                Log.Info("EditorLock, " + from + "  locking all");
                InputLockManager.SetControlLock(ControlTypes.All, "KRASHEditorLock");
                if (HighLogic.LoadedScene != GameScenes.SPACECENTER)
                    EditorLogic.fetch.Lock(true, true, true, "KRASH_Editor");
                //isEditorLocked = true;
            }
            else
            {
                Log.Info("EditorLock, " + from + " Unlocking");
                InputLockManager.SetControlLock(ControlTypes.None, "KRASHEditorLock");
                if (HighLogic.LoadedScene != GameScenes.SPACECENTER)
                    EditorLogic.fetch.Unlock("KRASH_Editor");
            }
#endif
        }

        public void GUIButtonToggle()
        {
            //if (!configDisplayActive || HighLogic.LoadedScene != GameScenes.SPACECENTER)
                GUIToggle();
        }

        bool configDisplayActive = false;
        public void GUIToggle()
        {
            if (HighLogic.LoadedScene == GameScenes.EDITOR)
                KRASHShelter.persistent.SetSuspendUpdate(false);
            if (HighLogic.LoadedScene != GameScenes.SPACECENTER && EditorLogic.RootPart == null && !Input.GetMouseButtonUp(1))
            {
                return;
            }
            // Check for right mouse click
            if (Input.GetMouseButtonUp(1) || HighLogic.LoadedScene == GameScenes.SPACECENTER)
            {
                configDisplayActive = !configDisplayActive;

                EditorLock(configDisplayActive, "GUIToggle 1");
                //cfgWinData = false;
                //onRightButtonStockClick ();
                return;
            }
            else
            {
                // Only go into sim mode if cfg screen not being shown
                if (!configDisplayActive)
                {
                    //EditorLock(false, "GUIToggle 2");
                    SetVisible(!visible);
                    EditorLock(visible, "GUIToggle 2.1");
                    if (visible)
                    {
                        selectedSite = null;
                        bodiesList = getAllowableBodies();
                        selectType = SelectionType.launchsites;
                        selectedBody = FlightGlobals.Bodies.Where(cb => cb.isHomeWorld).FirstOrDefault();
                       // EditorLock(true, "GUIToggle 3");
                        APIManager.ApiInstance.SimMenuEvent.Fire((Vessel)FlightGlobals.ActiveVessel, KRASHShelter.simCost);
                    }
                }
            }
            Log.Info("Exiting GUIToggle");
        }

        //		Rect windowRect = new Rect(((Screen.width - Camera.main.rect.x) / 2) + Camera.main.rect.x - 125, (Screen.height / 2 - 250), 570, 580);
        //Rect windowRect = new Rect (((Screen.width - Camera.main.rect.x) / 2) + Camera.main.rect.x - 125, (Screen.height / 2 - 250), 425, 580);
        // ASH 28102014 - Needs to be bigger for filter
        bool rectConfigured = false;
        Rect windowRect;
        Rect cfgWindowRect;

        public void OnGUI()
        {
            if (LaunchGUI.toolbarControl != null)
                LaunchGUI.toolbarControl.UseBlizzy(HighLogic.CurrentGame.Parameters.CustomParams<KRASH_Settings>().useBlizzy);
            if (Camera.main == null)
                return;

            if (!rectConfigured)
            {
                windowRect = new Rect(((Screen.width - Camera.main.rect.x) / 2) + Camera.main.rect.x - 125, (Screen.height / 2 - 250), 570, 580);
                cfgWindowRect = new Rect(((Screen.width - Camera.main.rect.x) / 2) + Camera.main.rect.x - 350, (Screen.height / 2 - 300), 700, 525);
                rectConfigured = true;
            }

            if (configDisplayActive)
            {
                cfgWindowRect = ClickThruBlocker.GUIWindow(0xB00B1E6, cfgWindowRect, drawCfgWindow, "KRASH Config Window");

            }
            else
            {
                try
                {
                    if (this.Visible())
                    {
                        drawSelector();
                        //					windowRect = ClickThruBlocker.GUIWindow(0xB00B1E6, windowRect, drawSelectorWindow, "Launch Site Selector");
                        //					this.bounds = GUILayout.Window (this.GetInstanceID (), this.bounds, this.Window, TITLE, HighLogic.Skin.window);
                    }
                }
                catch (Exception e)
                {
                    Log.Error("exception: " + e.Message);
                }
            }
        }

        private void DrawTitle(String text)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(text, HighLogic.Skin.label);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        bool cfgWinData = false;
        string strConfigName;
        string strflatSetupCost;
        string strflatPerMinCost;

        string strperPartSetupCost;
        string strperPartPerMinCost;

        string strperTonSetupCost;
        string strperTonPerMinCost;

        string strpercentSetupCost;
        string strpercentPerMinCost;

        string strAtmoMultipler;
        bool bTerminateAtSoiWithoutData;
        bool bTerminateAtLandWithoutData;
        bool bTerminateAtAtmoWithoutData;

        bool bContinueIfNoCash;
        string strDefaultMaxAllowableSimCost;
        string strDefaultSimTime;

        //string strselectedCosts = KRASHShelter.instance.cfg.selectedCosts;

        bool bshowRunningSimCosts;
        string strhorizontalPos;
        string strverticalPos;
        bool bshowAllInCareer;

        string currentConfigName;

        void initCfgWinData()
        {
            Log.Info("initCfgWinData");
            cfgWinData = true;
            if (KRASHShelter.instance.cfg == null)
                Log.Info("initCfgWinData cfg == null");

            strConfigName = KRASHShelter.persistent.selectedCostsCfg;
            strflatSetupCost = KRASHShelter.instance.cfg.flatSetupCost.ToString();
            strflatPerMinCost = KRASHShelter.instance.cfg.flatPerMinCost.ToString();

            strperPartSetupCost = KRASHShelter.instance.cfg.perPartSetupCost.ToString();
            strperPartPerMinCost = KRASHShelter.instance.cfg.perPartPerMinCost.ToString();

            strperTonSetupCost = KRASHShelter.instance.cfg.perTonSetupCost.ToString();
            strperTonPerMinCost = KRASHShelter.instance.cfg.perTonPerMinCost.ToString();

            strpercentSetupCost = (100 * KRASHShelter.instance.cfg.percentSetupCost).ToString();
            strpercentPerMinCost = (100 * KRASHShelter.instance.cfg.percentPerMinCost).ToString();

            strAtmoMultipler = KRASHShelter.instance.cfg.AtmoMultipler.ToString();
            bTerminateAtSoiWithoutData = KRASHShelter.instance.cfg.TerminateAtSoiWithoutData;
            bTerminateAtLandWithoutData = KRASHShelter.instance.cfg.TerminateAtLandWithoutData;
            bTerminateAtAtmoWithoutData = KRASHShelter.instance.cfg.TerminateAtAtmoWithoutData;

            bContinueIfNoCash = KRASHShelter.instance.cfg.ContinueIfNoCash;
            strDefaultMaxAllowableSimCost = KRASHShelter.instance.cfg.DefaultMaxAllowableSimCost.ToString();
            strDefaultSimTime = KRASHShelter.instance.cfg.DefaultSimTime.ToString();

            //	strselectedCosts = KRASH.cfg.selectedCosts;
            strConfigName = KRASHShelter.persistent.selectedCostsCfg;
            bshowRunningSimCosts = KRASHShelter.instance.cfg.showRunningSimCosts;
            strhorizontalPos = KRASHShelter.instance.cfg.horizontalPos.ToString();
            strverticalPos = KRASHShelter.instance.cfg.verticalPos.ToString();
            bshowAllInCareer = KRASHShelter.instance.cfg.showAllInCareer;
        }

        void saveCfgData(bool saveToFile, string strConfigName, bool accept = false)
        {
            // if (!accept)
            {
                KRASHShelter.instance.cfg.flatSetupCost = Convert.ToSingle(Convert.ToDouble(strflatSetupCost));
                KRASHShelter.instance.cfg.flatPerMinCost = Convert.ToSingle(Convert.ToDouble(strflatPerMinCost));

                KRASHShelter.instance.cfg.perPartSetupCost = Convert.ToSingle(Convert.ToDouble(strperPartSetupCost));
                KRASHShelter.instance.cfg.perPartPerMinCost = Convert.ToSingle(Convert.ToDouble(strperPartPerMinCost));

                KRASHShelter.instance.cfg.perTonSetupCost = Convert.ToSingle(Convert.ToDouble(strperTonSetupCost));
                KRASHShelter.instance.cfg.perTonPerMinCost = Convert.ToSingle(Convert.ToDouble(strperTonPerMinCost));

                KRASHShelter.instance.cfg.percentSetupCost = Convert.ToSingle(Convert.ToDouble(strpercentSetupCost)) / 100.0f;
                KRASHShelter.instance.cfg.percentPerMinCost = Convert.ToSingle(Convert.ToDouble(strpercentPerMinCost)) / 100.0f;

                KRASHShelter.instance.cfg.AtmoMultipler = Convert.ToSingle(Convert.ToDouble(strAtmoMultipler));
                KRASHShelter.instance.cfg.TerminateAtSoiWithoutData = bTerminateAtSoiWithoutData;
                KRASHShelter.instance.cfg.TerminateAtLandWithoutData = bTerminateAtLandWithoutData;
                KRASHShelter.instance.cfg.TerminateAtAtmoWithoutData = bTerminateAtAtmoWithoutData;

                KRASHShelter.instance.cfg.ContinueIfNoCash = bContinueIfNoCash;
                KRASHShelter.instance.cfg.DefaultMaxAllowableSimCost = Convert.ToSingle(Convert.ToDouble(strDefaultMaxAllowableSimCost));
                KRASHShelter.instance.cfg.DefaultSimTime = Convert.ToUInt16(strDefaultSimTime);

                KRASHShelter.persistent.selectedCostsCfg = strConfigName;
            }
            KRASHShelter.instance.cfg.showRunningSimCosts = bshowRunningSimCosts;
            KRASHShelter.instance.cfg.horizontalPos = Convert.ToUInt16(strhorizontalPos);
            KRASHShelter.instance.cfg.verticalPos = Convert.ToUInt16(strverticalPos);
            KRASHShelter.instance.cfg.showAllInCareer = bshowAllInCareer;


            if (saveToFile)
                KRASHShelter.instance.cfg.SaveConfiguration(strConfigName);
            KRASHShelter.instance.cfg.saveDisplayValues();
        }

        public void drawCfgWindow(int id)
        {

            bool cfgExists;

            if (!cfgWinData)
            {
                Log.Info("drawCfgWindow cfgWinData == false");
                initCfgWinData();
                // currentConfigName = KRASHShelter.instance.cfg.configName;
                currentConfigName = KRASHShelter.persistent.selectedCostsCfg;
            }
            GUI.skin = HighLogic.Skin;

            List<string> cfgs = KRASHShelter.instance.cfg.GetAvailableCfgs();

            GUILayout.BeginArea(new Rect(5, 50, 170, 465));
            DrawTitle("Selected Cfg");
            GUILayout.Space(8);
            cfgExists = false;
            sitesScrollPosition = GUILayout.BeginScrollView(sitesScrollPosition);
            foreach (string cfg in cfgs)
            {
                GUILayout.BeginHorizontal();

                GUI.enabled = !(cfg == strConfigName);
                if (cfg == strConfigName)
                    cfgExists = true;
                if (GUILayout.Button(cfg, GUILayout.Height(25), GUILayout.Width(140)))
                {
                    KRASHShelter.instance.cfg.LoadConfiguration(cfg);
                    initCfgWinData();
                }
                GUI.enabled = true;
                GUILayout.EndHorizontal();

            }

            GUILayout.EndScrollView();
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(185, 25, 700 - 185, 40));
            GUILayout.BeginHorizontal();
            if (HighLogic.CurrentGame.Mode != Game.Modes.CAREER)
            {
                GUILayout.Label("Configs don't apply to Sandbox or Science games");
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
            GUILayout.BeginArea(new Rect(185, 50, 700 - 185, 75));

            GUILayout.BeginVertical();
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Config name: ");
            strConfigName = GUILayout.TextField(strConfigName, GUILayout.MinWidth(150.0F), GUILayout.MaxWidth(150.0F));

            if (strConfigName.Length > 0 && strConfigName[0] == '*')
            {
                var lstyle = new GUIStyle(GUI.skin.label);
                lstyle.normal.textColor = Color.red;
                GUILayout.Label("<-----> Must be renamed to save", lstyle);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            var bstyle = new GUIStyle(GUI.skin.button);
            bstyle.normal.textColor = Color.yellow;
            GUILayout.FlexibleSpace();
            GUI.enabled = strConfigName.Length == 0 || (!(strConfigName[0] == '*') && strConfigName != "defaults"); ;
            if (GUILayout.Button("Save", bstyle, GUILayout.Width(70)))
            {
                saveCfgData(true, strConfigName);
                configDisplayActive = !configDisplayActive;
                EditorLock(configDisplayActive, "drawCfgWindow 4");
                return;
            }
            GUI.enabled = true;
            GUILayout.FlexibleSpace();

            if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER)
            {
                if (GUILayout.Button("Accept", bstyle, GUILayout.Width(70)))
                {
                    saveCfgData(false, strConfigName, true);
                    cfgWinData = false;
                    configDisplayActive = !configDisplayActive;
                    EditorLock(configDisplayActive, "drawCfgWindow 5");
                    KRASHShelter.persistent.selectedCostsCfg = strConfigName;
                    initCfgWinData();
                    return;
                }
                GUILayout.FlexibleSpace();
            }
            if (GUILayout.Button("Reset", bstyle, GUILayout.Width(70)))
            {
                KRASHShelter.instance.cfg.LoadConfiguration(currentConfigName);
                initCfgWinData();
            }
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Cancel", bstyle, GUILayout.Width(70)))
            {
                configDisplayActive = !configDisplayActive;
                EditorLock(configDisplayActive, "drawCfgWindow 6");
                cfgWinData = false;
                Log.Info("Cancel  currentConfigName: [" + currentConfigName + "]");
                KRASHShelter.instance.cfg.LoadConfiguration(currentConfigName);
                return;
            }
            GUILayout.FlexibleSpace();
            GUI.enabled = strConfigName.Length == 0 || (!(strConfigName[0] == '*') && cfgExists);
            if (GUILayout.Button("Delete", bstyle, GUILayout.Width(70)))
            {
                KRASHShelter.instance.cfg.DeleteConfiguration(strConfigName);
                KRASHShelter.instance.cfg.LoadConfiguration("* " + HighLogic.CurrentGame.Parameters.preset.ToString());
                strConfigName = KRASHShelter.instance.cfg.configName;
                //				configDisplayActive = !configDisplayActive;
                //				EditorLock (configDisplayActive);
                //				return;
            }
            GUILayout.FlexibleSpace();
            GUI.enabled = true;

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(185, 125, 225, 330));
            GUILayout.BeginVertical();

            DrawTitle("Cost Options");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Flat Setup Costs: ");
            GUILayout.FlexibleSpace();
            strflatSetupCost = GUILayout.TextField(strflatSetupCost, GUILayout.MinWidth(50.0F), GUILayout.MaxWidth(50.0F));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Flat Per-minute Costs: ");
            GUILayout.FlexibleSpace();
            strflatPerMinCost = GUILayout.TextField(strflatPerMinCost, GUILayout.MinWidth(50.0F), GUILayout.MaxWidth(50.0F));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Per-part Setup Costs: ");
            GUILayout.FlexibleSpace();
            strperPartSetupCost = GUILayout.TextField(strperPartSetupCost, GUILayout.MinWidth(50.0F), GUILayout.MaxWidth(50.0F));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Per-part Per-minute Costs: ");
            GUILayout.FlexibleSpace();
            strperPartPerMinCost = GUILayout.TextField(strperPartPerMinCost, GUILayout.MinWidth(50.0F), GUILayout.MaxWidth(50.0F));
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            GUILayout.Label("Per-ton Setup Costs: ");
            GUILayout.FlexibleSpace();
            strperTonSetupCost = GUILayout.TextField(strperTonSetupCost, GUILayout.MinWidth(50.0F), GUILayout.MaxWidth(50.0F));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Per-ton Per-minute Costs: ");
            GUILayout.FlexibleSpace();
            strperTonPerMinCost = GUILayout.TextField(strperTonPerMinCost, GUILayout.MinWidth(50.0F), GUILayout.MaxWidth(50.0F));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Percentage Setup Costs: ");
            GUILayout.FlexibleSpace();
            strpercentSetupCost = GUILayout.TextField(strpercentSetupCost, GUILayout.MinWidth(50.0F), GUILayout.MaxWidth(50.0F));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Percentage Per-minute Costs: ");
            GUILayout.FlexibleSpace();
            strpercentPerMinCost = GUILayout.TextField(strpercentPerMinCost, GUILayout.MinWidth(50.0F), GUILayout.MaxWidth(50.0F));
            GUILayout.EndHorizontal(); ;

            GUILayout.BeginHorizontal();
            GUILayout.Label("Atmospheric Multiplier: ");
            GUILayout.FlexibleSpace();
            strAtmoMultipler = GUILayout.TextField(strAtmoMultipler, GUILayout.MinWidth(50.0F), GUILayout.MaxWidth(50.0F));
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(425, 125, 250, 450));
            GUILayout.BeginVertical();

            DrawTitle("Termination Options (no data)");

            GUILayout.BeginHorizontal();
            bTerminateAtSoiWithoutData = GUILayout.Toggle(bTerminateAtSoiWithoutData, "Terminate at Soi");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            bTerminateAtLandWithoutData = GUILayout.Toggle(bTerminateAtLandWithoutData, "Terminate at Land");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            bTerminateAtAtmoWithoutData = GUILayout.Toggle(bTerminateAtAtmoWithoutData, "Terminate at Atmo");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(15);
            DrawTitle("Miscellaneous Options (global)");

            GUILayout.BeginHorizontal();
            bContinueIfNoCash = GUILayout.Toggle(bContinueIfNoCash, "Continue sim if no cash");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Default Max Sim Cost: ");
            GUILayout.FlexibleSpace();
            strDefaultMaxAllowableSimCost = GUILayout.TextField(strDefaultMaxAllowableSimCost, GUILayout.MinWidth(60.0F), GUILayout.MaxWidth(60.0F));
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
#if false
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Default time for calculations: ");
			GUILayout.FlexibleSpace ();
			strDefaultSimTime = GUILayout.TextField (strDefaultSimTime, GUILayout.MinWidth (40.0F), GUILayout.MaxWidth (40.0F));
			GUILayout.EndHorizontal ();

			GUILayout.Space (10);
#endif

            GUILayout.BeginHorizontal();
            bshowRunningSimCosts = GUILayout.Toggle(bshowRunningSimCosts, "Show Running Sim Costs");
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            bshowAllInCareer = GUILayout.Toggle(bshowAllInCareer, "Show all bodies in career");


            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (bshowRunningSimCosts)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Horizontal Position: ");
                GUILayout.FlexibleSpace();
                strhorizontalPos = GUILayout.TextField(strhorizontalPos, GUILayout.MinWidth(50.0F), GUILayout.MaxWidth(50.0F));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Vertical Position: ");
                GUILayout.FlexibleSpace();
                strverticalPos = GUILayout.TextField(strverticalPos, GUILayout.MinWidth(50.0F), GUILayout.MaxWidth(50.0F));
                GUILayout.EndHorizontal();

            }

            GUILayout.EndVertical();
            GUILayout.EndArea();

            GUI.DragWindow();
        }

#if true

        [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
        public class PersistentKey : Attribute
        {
        }

        [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
        public class PersistentField : Attribute
        {
        }

#if false
		public class LaunchSite
		{
			[PersistentKey]
			public string name;

			public string author;
			public SiteType type;
			public Texture logo;
			public Texture icon;
			public string description;

			public string category;
			public float opencost;
			public float closevalue;

			[PersistentField]
			public string openclosestate;

			public GameObject GameObject;
			public PSystemSetup.SpaceCenterFacility facility;

			public LaunchSite(string sName, string sAuthor, SiteType sType, Texture sLogo, Texture sIcon, string sDescription, string sDevice, float fOpenCost, float fCloseValue, string sOpenCloseState, GameObject gameObject, PSystemSetup.SpaceCenterFacility newFacility = null)
			{
				name = sName;
				author = sAuthor;
				type = sType;
				logo = sLogo;
				icon = sIcon;
				description = sDescription;
				category = sDevice;
				opencost = fOpenCost;
				closevalue = fCloseValue;
				openclosestate = sOpenCloseState;
				GameObject = gameObject;
				facility = newFacility;
			}
		}
		


#else
        public class LaunchSite
        {
            [PersistentKey]
            public string name;

            //			public PSystemSetup.SpaceCenterFacility facility;
            string author;
            SiteType type;
            GameObject gameObject;

            public LaunchSite(string sName, string sAuthor, SiteType sType, GameObject sGameObject)
            {
                name = sName;
                author = sAuthor;
                type = sType;
                gameObject = sGameObject;
            }
            public SiteType GetSiteType()
            {
                return type;
            }
        }
#endif
        public Vector2 sitesScrollPosition, bodiesScrollPosition;
        public Vector2 descriptionScrollPosition;
        //		Rect windowRect = new Rect(((Screen.width - Camera.main.rect.x) / 2) + Camera.main.rect.x - 125, (Screen.height / 2 - 250), 570, 580);
        //		private SiteType editorType = SiteType.Any;
        //		private bool orbitSelection = false;
        public static List<LaunchSite> sites = new List<LaunchSite>();

        public Boolean isCareerGame()
        {
            if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER)
            {
                // disableCareerStrategyLayer is configurable in KerbalKonstructs.cfg
                //if (!KerbalKonstructs.instance.disableCareerStrategyLayer)
                {
                    return true;
                }
                //	else
                //		return false;
            }
            else
            {
                return false;
            }
        }

        public bool inited = false;
        public static LaunchSite runway = new LaunchSite("Runway", "Squad", SiteType.SPH, SpaceCenter.Instance.gameObject/*, new PSystemSetup.SpaceCenterFacility()*/);
        public static LaunchSite launchpad = new LaunchSite("LaunchPad", "Squad", SiteType.VAB, SpaceCenter.Instance.gameObject/*, new PSystemSetup.SpaceCenterFacility()*/);

        void setOrbit(CelestialBody selectedBody)
        {
            
            if (simType != SimType.LANDED)
            {
                if (!selectedBody)
                {
                    newAltitude = HighLogic.CurrentGame.Parameters.CustomParams<KRASH_Settings>().defaultAtmoStartingAltitude;
                    altitude = newAltitude.ToString();
                    return;
                }
                if (selectedBody.atmosphereDepth <= 1)
                {
                    newAltitude = HighLogic.CurrentGame.Parameters.CustomParams<KRASH_Settings>().defaultNoAtmoStartingAltitude;
                }
                else
                {
                    //newAltitude = selectedBody.atmosphereDepth + 100;
                    newAltitude = HighLogic.CurrentGame.Parameters.CustomParams<KRASH_Settings>().defaultAtmoStartingAltitude;
                }
                altitude = newAltitude.ToString();
            }
        }


        public void drawSelector()
        {
            if (!inited)
            {
                //				orbitSelection = false;
                selectType = SelectionType.launchsites;
                simType = SimType.LAUNCHPAD; ;

#if false
				if (EditorDriver.editorFacility.Equals (EditorFacility.SPH)) {
					simType = SimType.RUNWAY;
					selectedSite = runway;
				} else {
					simType = SimType.LAUNCHPAD;
					selectedSite = launchpad;
				}
#endif
                inited = true;

                bodiesList = getAllowableBodies();
#if false
				if (sites.Count () == 0) {
					sites.Add (runway);
					sites.Add (launchpad);
				}
#endif
            }
            if (selectedSite == null)
            {
                if (EditorDriver.editorFacility.Equals(EditorFacility.SPH))
                {
                    simType = SimType.RUNWAY;
                    selectedSite = runway;
                }
                else
                {
                    simType = SimType.LAUNCHPAD;
                    selectedSite = launchpad;
                }
            }
            //if (sites == null)
            //	sites = new List<LaunchSite> ();
            if (sites.Count == 0)
            {
                Log.Info("adding to sites");
                // (string sName, string sAuthor, SiteType sType, GameObject sGameObject)
                sites.Add(launchpad);
                sites.Add(runway);
                selectType = SelectionType.launchsites;
            }

            // Camera.main is null when first loading a scene
            if (Camera.main != null)
            {
                windowRect = ClickThruBlocker.GUIWindow(0xB00B1E6, windowRect, drawSelectorWindow, "Launch Site Selector");
            }
#if false
            if (windowRect.Contains(Event.current.mousePosition))
            {
                EditorLock(true);
            }
            else
            {
                EditorLock(false);
            }
#endif
        }

        private enum ProgressItem
        {
            REACHED,
            ORBITED,
            LANDED,
            ESCAPED,
            RETURNED_FROM
        }

        public enum SimType
        {
            RUNWAY,
            LAUNCHPAD,
            LANDED,
            ORBITING
        }

        static string altitude = "";
        static string latitude = "", longitude = "";
        static public double newAltitude = 0.0, newLatitude = 0.0, newLongitude = 0.0;
        static public SimType simType = SimType.LANDED;
        Vector3 shipSize;

        List<CelestialBody> getAllowableBodies(String filter = "ALL")
        {
            CelestialBody parent;
            List<CelestialBody> bodiesList = new List<CelestialBody>();
            CelestialBody[] tmpBodies = GameObject.FindObjectsOfType(typeof(CelestialBody)) as CelestialBody[];
            Log.Info("numBodies: " + tmpBodies.Length);
            foreach (CelestialBody body in GameObject.FindObjectsOfType(typeof(CelestialBody)))
            {
                Log.Info("body name: " + body.name);
                if (body.orbit != null && body.orbit.referenceBody != null)
                {
                    parent = body.orbit.referenceBody;
                }
                else
                    parent = null;
                switch (filter)
                {
                    case "ALL":
                        bodiesList.Add(body);
                        break;
                    case "PLANETS":
                        if (parent != null && parent == Sun.Instance.sun)
                            bodiesList.Add(body);
                        break;
                    case "MOONS":
                        if (parent != null && parent != Sun.Instance.sun)
                            bodiesList.Add(body);
                        break;
                    default:
                        bodiesList.Add(body);
                        break;
                }
            }
            Log.Info("allowableBodies: " + bodiesList.Count);
            return bodiesList;
        }

        public void drawSelectorWindow(int id)
        {
            Log.Info("drawSelectorWindow 1");
            GUI.skin = HighLogic.Skin;
            //string smessage = "";
            //ScreenMessageStyle smsStyle = ScreenMessageStyle.UPPER_RIGHT;

            // ASH 28102014 Category filter handling added.
            // ASH 07112014 Disabling of restricted categories added.
            //GUILayout.BeginArea (new Rect (10, 25, 415, 550));
            GUILayout.BeginArea(new Rect(10, 25, 370, 545));

            GUILayout.BeginHorizontal();

            // 			if (GUILayout.Button ("Current Launch Facility", GUILayout.Width (175))) {
            if (GUILayout.Button(FlightGlobals.Bodies.Where(cb => cb.isHomeWorld).FirstOrDefault().name))
            {
                //				orbitSelection = false;
                if (EditorDriver.editorFacility.Equals(EditorFacility.VAB))
                {
                    selectType = SelectionType.launchsites;
                    simType = SimType.LAUNCHPAD;
                    selectedSite = launchpad;
                }
                else
                {
                    selectType = SelectionType.launchsites;
                    simType = SimType.RUNWAY;
                    selectedSite = runway;
                }
            }

            if (GUILayout.Button("Orbit selection"))
            {
                selectType = SelectionType.celestialbodies;
                simType = SimType.ORBITING;
                setOrbit(selectedBody);
                //newAltitude = selectedBody.atmosphereDepth + 100;
                //altitude = newAltitude.ToString ();
            }

            if (GUILayout.Button("Landed"))
            {
                selectType = SelectionType.celestialbodies;
                simType = SimType.LANDED;

                // Make sure ship is high enough to avoid exploding inside the surface
                // Get largest dimension, then add 5 for safety
                shipSize = ShipConstruction.CalculateCraftSize(EditorLogic.fetch.ship);
                newAltitude = Math.Floor(Math.Max(shipSize.z, Math.Max(shipSize.y, shipSize.x))) + 5;

                altitude = newAltitude.ToString();
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (simType != SimType.LAUNCHPAD && simType != SimType.RUNWAY)
            {

                if (GUILayout.Button("All", GUILayout.Width(45)))
                {
                    selectType = SelectionType.celestialbodies;
                    bodiesList = getAllowableBodies("ALL");

                    //bodies = GameObject.FindObjectsOfType (typeof(CelestialBody)) as CelestialBody[]; 
                    //sites = (editorType == SiteType.Any) ? LaunchSiteManager.getLaunchSites() : LaunchSiteManager.getLaunchSites(editorType, true, "RocketPad");
                }
                if (GUILayout.Button("Planets", GUILayout.Width(60)))
                {
                    bodiesList = getAllowableBodies("PLANETS");
                    selectType = SelectionType.celestialbodies;
                    //				sites = (editorType == SiteType.Any) ? LaunchSiteManager.getLaunchSites() : LaunchSiteManager.getLaunchSites(editorType, true, "RocketPad");
                }
                if (GUILayout.Button("Moons", GUILayout.Width(60)))
                {
                    bodiesList = getAllowableBodies("MOONS");
                    selectType = SelectionType.celestialbodies;
                    //				sites = (editorType == SiteType.Any) ? LaunchSiteManager.getLaunchSites() : LaunchSiteManager.getLaunchSites(editorType, true, "RocketPad");
                }
            }

            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            // ASH 28102014 Category filter handling added
            //			if (sites == null) sites = (editorType == SiteType.Any) ? LaunchSiteManager.getLaunchSites() : LaunchSiteManager.getLaunchSites(editorType, true, "ALL");

            if (selectType == SelectionType.launchsites)
            {
                if (sites == null)
                    Log.Info("sites is null");
#if true
                sitesScrollPosition = GUILayout.BeginScrollView(sitesScrollPosition);
                foreach (LaunchSite site in sites)
                {
                    Log.Info("site: " + site.name);

                    //					if (isCareerGame ())
                    GUILayout.BeginHorizontal();
                    // Light icons in the launchsite list only shown in career so only need horizontal for two elements for that mode

                    GUI.enabled = !(selectedSite == site);
                    if (GUILayout.Button(site.name, GUILayout.Height(30)))
                    {
                        selectedSite = site;

                        // ASH Career Mode Unlocking
                        // In career the launchsite is not set by the launchsite list but rather in the launchsite description
                        // panel on the right
                        //					if (!isCareerGame())
                        {
                            //smessage = "Launchsite set to " + site.name;
                            //ScreenMessages.PostScreenMessage (smessage, 10, smsStyle);
                            if (selectedSite.GetSiteType() == SiteType.VAB)
                                simType = SimType.LAUNCHPAD;
                            else
                                simType = SimType.RUNWAY;

                            selectType = SelectionType.launchsites;

                        }
                    }
                    GUI.enabled = true;
                    //					if (isCareerGame ()) {
                    //					// if site is closed show red light
                    //					// if site is open show green light

                    //					// If a site has an open cost of 0 it's always open
                    //					if (site.openclosestate == "Open" || site.opencost == 0)
                    //					{
                    //						site.openclosestate = "Open";
                    //						GUILayout.Label(tIconOpen, GUILayout.Height(30), GUILayout.Width(30));
                    //					}
                    //					else
                    //					{
                    //						GUILayout.Label(tIconClosed, GUILayout.Height(30), GUILayout.Width(30));
                    //					}
                    //					// Light icons in the launchsite list only shown in career
                    GUILayout.EndHorizontal();
                    //					}
                }
                GUILayout.EndScrollView();
#endif
            }
            else
            {

                if (bodiesList == null)
                    Log.Info("bodiesList is null");
                if (KRASHShelter.instance == null)
                    Log.Info("KRASHShelter.instance is null");
                Log.Info("KRASHShelter.instance.cfg.showAllInCareer: " + KRASHShelter.instance.cfg.showAllInCareer.ToString());
                bodiesScrollPosition = GUILayout.BeginScrollView(bodiesScrollPosition);
                //                if (ProgressTracking.Instance != null || !isCareerGame() || KRASHShelter.instance.cfg.showAllInCareer)
                //                if ( !isCareerGame() || KRASHShelter.instance.cfg.showAllInCareer)
                foreach (CelestialBody body in bodiesList)
                {
                    KSPAchievements.CelestialBodySubtree tree = null;
                    //                        if (ProgressTracking.Instance != null)
                    //                            Log.Info("ProgressTracking.Instance is not null");
                    //                        else
                    //                            Log.Info("ProgressTracking.Instance is null");

                    //                            tree = ProgressTracking.Instance.celestialBodyNodes.Where(node => node.Body == body).FirstOrDefault();

                    if (KRASHShelter.persistent == null)
                        Log.Info("KRASHShelter.persistent is null");
                    if (KRASHPersistent.celestialBodyNodes == null)
                        Log.Info("KRASHShelter.persistent.celestialBodyNodes is  null");



                    tree = KRASHPersistent.celestialBodyNodes.Where(node => node.Body == body).FirstOrDefault();

#if false
                    Log.Info("Dumping body info");
                        foreach (var cbn in KRASHPersistent.celestialBodyNodes)
                        {
                            Log.Info("body: " + cbn.Body.name + "  IsReached: " + cbn.IsReached.ToString());
                            foreach (var cbnchild in cbn.childTrees)
                            {
                                Log.Info("moon: " + cbnchild.Body.name + "  IsReached: " + cbnchild.IsReached.ToString());
                            }
                        }
#endif

                    Log.Info("isCareerGame: " + isCareerGame().ToString());
                    bool scienceOrbit;

                    if (!isCareerGame() || KRASHShelter.instance.cfg.showAllInCareer)
                        scienceOrbit = true;
                    else
                        scienceOrbit = ResearchAndDevelopment.GetSubjects().Where(ss => ss.science > 0.0f && ss.IsFromBody(body) && ss.id.Contains("InSpace")).Any();

                    if (!isCareerGame() ||
                            KRASHShelter.instance.cfg.showAllInCareer ||
                            scienceOrbit ||
                            body.isHomeWorld ||
                            (tree != null && (simType == SimType.LANDED && tree.landing.IsComplete) || (simType == SimType.ORBITING && tree.IsReached)))
                    {
                        if (tree != null)
                            Log.Info("body: " + body.name + "  is reached: " + tree.IsReached);
                        else
                            Log.Info("body: " + body.name + "  is reached: (tree is null) false");
                        GUI.enabled = !(selectedBody == body);
                        if (GUILayout.Button(body.name, GUILayout.Height(30)))
                        {
                            selectedBody = body;

                            setOrbit(selectedBody);

                            //						LaunchSiteManager.setLaunchSite(site);
                            //smessage = "Reference body set to " + body.name;
                            //ScreenMessages.PostScreenMessage (smessage, 10, smsStyle);


                        }
                    }
                }
                GUILayout.EndScrollView();
            }


            GUILayout.EndArea();

            GUI.enabled = true;
            drawRightSelectorWindow(selectType);
            GUI.DragWindow();
        }
        // ======================================================================================


        float getMassLimit()
        {
            SimType s = simType;
            if (simType != SimType.LAUNCHPAD && simType != SimType.RUNWAY)
            {
                switch (selectedSite.GetSiteType())
                {
                    case SiteType.VAB:
                        s = SimType.LAUNCHPAD;
                        break;
                    case SiteType.SPH:
                        s = SimType.RUNWAY;
                        break;

                }
            }
            bool isPad = false;
            float editorNormLevel;
            if (s == SimType.LAUNCHPAD)
            {
                editorNormLevel = ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.LaunchPad);
                isPad = true;
            }
            else
            {
                editorNormLevel = ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.Runway);
            }
            return GameVariables.Instance.GetCraftMassLimit(editorNormLevel, isPad);
        }

        Vector3 getSizeLimit()
        {
            //		if (simType != SimType.LAUNCHPAD && simType != SimType.RUNWAY)
            //			return new Vector3(9999,9999,9999);

            SimType s = simType;
            if (simType != SimType.LAUNCHPAD && simType != SimType.RUNWAY)
            {
                switch (selectedSite.GetSiteType())
                {
                    case SiteType.VAB:
                        s = SimType.LAUNCHPAD;
                        break;
                    case SiteType.SPH:
                        s = SimType.RUNWAY;
                        break;

                }
            }


            bool isPad = false;
            float editorNormLevel;
            if (s == SimType.LAUNCHPAD)
            {
                editorNormLevel = ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.LaunchPad);
                isPad = true;
            }
            else
            {
                editorNormLevel = ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.Runway);
            }
            return GameVariables.Instance.GetCraftSizeLimit(editorNormLevel, isPad);
        }

        int getPartLimit()
        {
            //			if (simType != SimType.LAUNCHPAD && simType != SimType.RUNWAY)
            //				return 9999;

            SimType s = simType;
            if (simType != SimType.LAUNCHPAD && simType != SimType.RUNWAY)
            {
                switch (selectedSite.GetSiteType())
                {
                    case SiteType.VAB:
                        s = SimType.LAUNCHPAD;
                        break;
                    case SiteType.SPH:
                        s = SimType.RUNWAY;
                        break;

                }
            }

            bool isPad = false;
            float editorNormLevel;

            if (s == SimType.LAUNCHPAD)
            {
                editorNormLevel = ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.VehicleAssemblyBuilding);
                isPad = true;
            }
            else
            {
                editorNormLevel = ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.SpaceplaneHangar);
            }
            return GameVariables.Instance.GetPartCountLimit(editorNormLevel, isPad);

        }

        // ======================================================================================
        bool checkForCrew(VesselCrewManifest dialogVessel)
        {
            
            List<PartCrewManifest> dialogParts = dialogVessel.GetCrewableParts();
            for (int partIndex = 0; partIndex < dialogParts.Count; ++partIndex)
            {
                PartCrewManifest dialogPart = dialogParts[partIndex];
                ProtoCrewMember[] dialogCrew = dialogPart.GetPartCrew();
                for (int slotIndex = 0; slotIndex < dialogCrew.Length; ++slotIndex)
                {
                    if (dialogCrew[slotIndex] != null)
                    {
                        ProtoCrewMember dialogMember = dialogCrew[slotIndex];
                        Log.Info("dialogMember: " + dialogMember.name);
                        return true;
                    }
                }
            }
            return false;
        }
        void drawRightSelectorWindow(SelectionType type)
        {
            Log.Info("drawRightSelectorWindow 1");
            //string smessage = "";
            //ScreenMessageStyle smsStyle = (ScreenMessageStyle)2;
            //GUI.skin = HighLogic.Skin;

            GUI.skin.label.padding = new RectOffset(0, 0, 0, 0);
            GUILayout.BeginArea(new Rect(385, 25, 180, 545));
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            var oldColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.green;
            var bstyle = new GUIStyle(GUI.skin.button);
            bstyle.normal.textColor = Color.yellow;
            //bstyle.normal.background = new Texture2D(2,2);


            float dryMass, fuelMass;
            string startSim = "";
            bool flyable = true;
            //if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER)
            {

                float launchMassLimit = getMassLimit();

                //EditorLogic.fetch.ship.GetShipMass (out dryMass, out fuelMass);
                //if (dryMass + fuelMass > launchMassLimit) {
                if (EditorLogic.fetch.ship.GetTotalMass() > launchMassLimit)
                {
                    flyable = false;
                    startSim = "Vessel too heavy: " + Math.Round(EditorLogic.fetch.ship.GetTotalMass(), 1).ToString() + "t";
                    startSim += "\nWeight limit: " + launchMassLimit + "t";
                }

                Vector3 s = EditorLogic.fetch.ship.shipSize;
                Vector3 sizeLimit = getSizeLimit();
                if (s.x > sizeLimit.x || s.z > sizeLimit.z)
                {
                    startSim += "\nVessel too wide: \n" + Math.Round(s.x, 1).ToString() + "m x" + Math.Round(s.z, 1).ToString() + "m";
                    startSim += "\nWidth limit: " + sizeLimit.x.ToString() + "m";
                    flyable = false;
                }
                if (s.y > sizeLimit.y)
                {
                    startSim += "\nVessel too tall: \n" + Math.Round(s.y, 1).ToString() + "m";
                    startSim += "\nHeight limit: " + sizeLimit.y.ToString() + "m";
                    flyable = false;
                }

                if (EditorLogic.fetch.ship.parts.Count() > getPartLimit())
                {
                    startSim += "\nToo many parts: " + EditorLogic.fetch.ship.parts.Count().ToString();
                    startSim += "\nPart limit: " + getPartLimit().ToString();
                    flyable = false;
                }

                if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER)
                {
                    EditorLogic.fetch.ship.GetShipMass(out dryMass, out fuelMass);
                    KRASHShelter.simSetupCost = (float)Math.Round(KRASHShelter.instance.cfg.flatSetupCost +
                        EditorLogic.fetch.ship.parts.Count * KRASHShelter.instance.cfg.perPartSetupCost +
                        (dryMass + fuelMass) * KRASHShelter.instance.cfg.perTonSetupCost + KRASHShelter.shipCost * KRASHShelter.instance.cfg.percentSetupCost, 1);


                    if (Funding.Instance.Funds <= 0 ||
                        (KRASHShelter.simSetupCost > Funding.Instance.Funds &&
                        !KRASHShelter.instance.cfg.ContinueIfNoCash &&
                        !KRASHShelter.persistent.shelterSimulationActive))
                    {
                        Log.Info("Not enough money to start sim");

                        startSim += "\nNot enough money to start sim";
                        flyable = false;
                    }
                }
                bool lockedparts = false;
                foreach (var part in EditorLogic.fetch.ship.parts)
                {
                    AvailablePart ap = PartLoader.getPartInfoByName(part.name);                   
                    if (!ResearchAndDevelopment.PartTechAvailable(ap))
                    {                            
                        lockedparts = true;
                        break;
                    }
                }
                if (lockedparts)
                {
                    startSim += "\nVessel has locked parts";
                    flyable = false;

                }
                bool controllable = false;

                if (CrewAssignmentDialog.Instance != null)
                {
                    controllable = checkForCrew(CrewAssignmentDialog.Instance.GetManifest());
                }
                else
                {
                    controllable = checkForCrew(ShipConstruction.ShipManifest);
                }

                if (!controllable)
                {
                    Log.Info(ShipConstruction.ShipManifest.CrewCount.ToString());
                    foreach (Part p in EditorLogic.fetch.ship.parts)
                    {
                        foreach (PartModule m in p.Modules)
                        {
                            if (m.moduleName == "ModuleCommand" && p.CrewCapacity == 0)
                            {
                                controllable = true;
                                break;
                            }
                        }
                    }
                }

                if (!controllable)
                { 
                    startSim += "\nVessel is not controllable";
                    flyable = false;
                }

                Log.Info("flyable 1: " + flyable.ToString());

                //                Log.Info("KRASHShelter.LimitSimCost: " + KRASHShelter.LimitSimCost.ToString());
                if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER)
                    if (KRASHShelter.simSetupCost > KRASHShelter.LimitSimCost && KRASHShelter.LimitSimCost > 0)
                {
                    startSim += "\nSim cost exceeds limit";
                    flyable = false;
                }
            }
            Log.Info("flyable 3: " + flyable.ToString());
            if (flyable)
            {
                startSim = "Start simulation";

                if (GUILayout.Button("Start simulation", bstyle, GUILayout.Width(170.0f), GUILayout.Height(125.0f)))
                {
                    Log.Info("Start simulation");
                    GUI.backgroundColor = oldColor;
                    EditorLock(false, "drawCfgWindow 7");

                    LaunchSim();
                    SetVisible(false);

                    return;
                }
            }
            else
            {

                GUILayout.Label("Vessel Unlaunchable for following reason(s):");
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();

                GUIStyle t = new GUIStyle("TextField");
                t.fontSize = 11;
                //				GUILayout.Label (startSim, "TextField");
                GUILayout.Label(startSim, t);
            }


            GUI.backgroundColor = oldColor;
            GUILayout.EndHorizontal();
            GUILayout.Space(15);
            GUILayout.BeginHorizontal();
            //var style = new GUIStyle();
            //style.normal.textColor = Color.green;

            GUIStyle fontColorCyan = new GUIStyle(GUI.skin.label);
            fontColorCyan.normal.textColor = Color.cyan;

            GUILayout.Label("Simulation Settings", fontColorCyan);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Start location:");
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            switch (type)
            {
                case SelectionType.launchsites:
                    GUILayout.Box(FlightGlobals.Bodies.Where(cb => cb.isHomeWorld).FirstOrDefault().name, GUILayout.Height(21));
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    switch (simType)
                    {
                        case SimType.LAUNCHPAD:
                            GUILayout.Box("Launchpad");
                            break;
                        case SimType.RUNWAY:
                            GUILayout.Box("Runway");
                            break;
                    }
                    GUILayout.EndHorizontal();
                    break;
                case SelectionType.celestialbodies:
                    if (selectedBody != null)
                    {
                        GUILayout.Box(selectedBody.name);
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Altitude:");
                        GUILayout.FlexibleSpace();
                        altitude = GUILayout.TextField(altitude, GUILayout.MinWidth(90.0F), GUILayout.MaxWidth(90.0F), GUILayout.Height(18));
                        try
                        {
                            newAltitude = Convert.ToDouble(altitude);
                        }
                        catch (Exception)
                        {
                        }
                        finally
                        {
                            altitude = newAltitude.ToString();
                            GUILayout.EndHorizontal();
                            if (newAltitude > selectedBody.sphereOfInfluence - selectedBody.Radius)
                                newAltitude = selectedBody.sphereOfInfluence - selectedBody.Radius;

                            if (newAltitude <= Math.Floor(Math.Max(shipSize.z, Math.Max(shipSize.y, shipSize.x))) + 1)
                                newAltitude = Math.Floor(Math.Max(shipSize.z, Math.Max(shipSize.y, shipSize.x))) + 1;

                        }
                        if (simType == SimType.LANDED)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Latitude:");
                            GUILayout.FlexibleSpace();
                            latitude = GUILayout.TextField(latitude, GUILayout.MinWidth(90.0F), GUILayout.MaxWidth(90.0F), GUILayout.Height(18));
                            try
                            {
                                newLatitude = Convert.ToDouble(latitude);
                            }
                            catch (Exception)
                            {
                            }
                            finally
                            {
                            }
                            char last;
                            if (latitude.Length > 0)
                                last = latitude[latitude.Length - 1];
                            else last = ' ';
                            latitude = newLatitude.ToString();
                            if (last == '.')
                                latitude += ".";
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Longitude:");
                            GUILayout.FlexibleSpace();
                            longitude = GUILayout.TextField(longitude, GUILayout.MinWidth(90.0F), GUILayout.MaxWidth(90.0F), GUILayout.Height(18));
                            try
                            {
                                newLongitude = Convert.ToDouble(longitude);
                            }
                            catch (Exception)
                            {
                            }
                            finally
                            {
                            }
                            if (longitude.Length > 0)
                                last = longitude[longitude.Length - 1];
                            else last = ' ';
                            longitude = newLongitude.ToString();
                            if (last == '.')
                                longitude += '.';
                            GUILayout.EndHorizontal();
                        }
                    }

                    break;
#if false
			case SelectionType.launchsites:
				if (selectedSite != null)
					GUILayout.Box(selectedSite.name);
				//GUILayout.Box("By " + selectedSite.author);
				//GUILayout.Box(selectedSite.logo);
				descriptionScrollPosition = GUILayout.BeginScrollView(descriptionScrollPosition);
				GUI.enabled = false;
				//GUILayout.TextArea(selectedSite.description);//, GUILayout.ExpandHeight(true));
				GUILayout.TextArea("Test selectedSite description");
				GUI.enabled = true;
				GUILayout.EndScrollView();

				break;
#endif
            }

            GUILayout.Space(10);
            GUIStyle fontColorYellow = new GUIStyle(GUI.skin.label);
            fontColorYellow.normal.textColor = Color.yellow;
            GUIStyle fontColorStd = new GUIStyle(GUI.skin.label);
            //GUIStyle fontColorCyan = new GUIStyle(GUI.skin.label);
            //fontColorCyan.normal.textColor = Color.cyan;

            GUILayout.BeginHorizontal();
            GUILayout.Label("Part count:");
            GUILayout.FlexibleSpace();
            GUILayout.Label(EditorLogic.fetch.ship.parts.Count.ToString(), fontColorYellow);
            GUILayout.EndHorizontal();

            //			float dryMass, fuelMass;
            EditorLogic.fetch.ship.GetShipMass(out dryMass, out fuelMass);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Dry Mass:");
            GUILayout.FlexibleSpace();

            GUILayout.Label(dryMass.ToString(), fontColorYellow);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Fuel Mass:");
            GUILayout.FlexibleSpace();
            GUILayout.Label(fuelMass.ToString(), fontColorYellow);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Total Mass:");
            GUILayout.FlexibleSpace();
            GUILayout.Label((dryMass + fuelMass).ToString(), fontColorYellow);
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            if (KRASH.testFlightLoaded)
            {
                GUILayout.BeginHorizontal();
                KRASHShelter.instance.disableTestFlightForSim = GUILayout.Toggle(KRASHShelter.instance.disableTestFlightForSim, "Disable TestFlight");
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

            if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER)
            {

                Log.Info("drawRightSelectorWindow 5");

                GUILayout.BeginHorizontal();
                GUILayout.Label("Simulation Costs:", fontColorCyan);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Sim setup cost:");
                GUILayout.FlexibleSpace();

                EditorLogic.fetch.ship.GetShipMass(out dryMass, out fuelMass);
                KRASHShelter.dryMass = dryMass;
                KRASHShelter.fuelMass = fuelMass;

                KRASHShelter.simSetupCost = (float)Math.Round(KRASHShelter.instance.cfg.flatSetupCost +
                    EditorLogic.fetch.ship.parts.Count * KRASHShelter.instance.cfg.perPartSetupCost +
                    (dryMass + fuelMass) * KRASHShelter.instance.cfg.perTonSetupCost + KRASHShelter.shipCost * KRASHShelter.instance.cfg.percentSetupCost, 1);

                GUILayout.Label(KRASHShelter.simSetupCost.ToString(), fontColorYellow);
                GUILayout.EndHorizontal();
                if (!flyable)
                    KRASHShelter.simSetupCost = 0;

                GUILayout.BeginHorizontal();
                GUILayout.Label("Est. Sim/min cost:");
                GUILayout.FlexibleSpace();

                float estSimPerMin = KRASHShelter.instance.cfg.flatPerMinCost +
                    EditorLogic.fetch.ship.parts.Count * KRASHShelter.instance.cfg.perPartPerMinCost +
                    (dryMass + fuelMass) * KRASHShelter.instance.cfg.perTonPerMinCost + KRASHShelter.shipCost * KRASHShelter.instance.cfg.percentPerMinCost;

                GUILayout.Label(Math.Round(estSimPerMin, 1).ToString(), fontColorYellow);
                GUILayout.EndHorizontal();
                float m = KRASHShelter.instance.cfg.AtmoMultipler;
                float estSimAtmoPerMin = estSimPerMin;

                if (m < 1.0F)
                    m = 1.0F;
                if (m > 1.0)
                {
                    estSimAtmoPerMin = (float)Math.Round(estSimPerMin * m, 1);
                    GUILayout.BeginHorizontal(GUILayout.Height(18));
                    GUILayout.Label("Est. Atmo Sim/min cost:");
                    GUILayout.FlexibleSpace();

                    GUILayout.Label(estSimAtmoPerMin.ToString(), fontColorYellow);
                    GUILayout.EndHorizontal();
                }

                GUILayout.BeginHorizontal();
                limitMaxCosts = GUILayout.Toggle(limitMaxCosts, "Limit max costs");
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                if (limitMaxCosts)
                {
                    if (KRASHShelter.LimitSimCost == 0)
                    {
                        if (KRASHShelter.instance.cfg.DefaultMaxAllowableSimCost > 0)
                            KRASHShelter.LimitSimCost = KRASHShelter.instance.cfg.DefaultMaxAllowableSimCost;
                        else
                            KRASHShelter.LimitSimCost = Math.Round(KRASHShelter.simSetupCost + KRASHShelter.instance.cfg.DefaultSimTime * estSimAtmoPerMin, 1);
                    }
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Limit: ");
                    GUILayout.FlexibleSpace();
                    double f;
                    string s = GUILayout.TextField(KRASHShelter.LimitSimCost.ToString(), GUILayout.MinWidth(90.0F), GUILayout.MaxWidth(90.0F), GUILayout.Height(18));
                    try
                    {
                        f = Convert.ToDouble(s);
                    }
                    catch (Exception)
                    {
                        f = KRASHShelter.LimitSimCost;
                    }
                    finally
                    {
                    }
                    if (f < KRASHShelter.simSetupCost && f < KRASHShelter.instance.cfg.DefaultMaxAllowableSimCost)
                        f = Math.Min(KRASHShelter.simSetupCost, KRASHShelter.instance.cfg.DefaultMaxAllowableSimCost);
                    KRASHShelter.LimitSimCost = f;
                    GUILayout.EndHorizontal();
                }
                else
                    KRASHShelter.LimitSimCost = 0;
            }

            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            bstyle = new GUIStyle(GUI.skin.button);
            bstyle.normal.textColor = Color.yellow;
            //bstyle.normal.background = new Texture2D(2,2);
            GUI.backgroundColor = Color.red;

            if (GUILayout.Button("Cancel", bstyle, GUILayout.Width(170.0f), GUILayout.Height(30.0f)))
            {
                GUI.backgroundColor = oldColor;
                GUIToggle();
                return;
            }
            GUI.backgroundColor = oldColor;
            GUILayout.EndHorizontal();
            //GUILayout.FlexibleSpace ();
            GUILayout.EndVertical();
            GUILayout.EndArea();
            GUI.DragWindow(new Rect(0, 0, 10000, 10000));
        }
#endif

        public void setLaunchSite(LaunchSite site)
        {
            Log.Info("setLaunchSite");
            Log.Info("simType: " + simType.ToString());
            Log.Info("site.name: " + site.name);
            KRASHShelter.bodiesListAtSimStart = getAllowableBodies("ALL");

            if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER)
            {
                KRASHShelter.preSimStatus = new List<PreSimStatus>();
                foreach (CelestialBody body in KRASHShelter.bodiesListAtSimStart)
                {
                    PreSimStatus s = new PreSimStatus();
                    s.flightsGlobalIndex = body.flightGlobalsIndex;
                    KSPAchievements.CelestialBodySubtree tree = null;
                    //                    if (ProgressTracking.Instance != null)
                    //                        tree = ProgressTracking.Instance.celestialBodyNodes.Where(node => node.Body == body).FirstOrDefault();
                    tree = KRASHPersistent.celestialBodyNodes.Where(node => node.Body == body).FirstOrDefault();
                    if (tree != null)
                    {
                        s.isReached = tree.IsReached;

                        s.landed = tree.landing.IsComplete;

                        bool scienceFlying = ResearchAndDevelopment.GetSubjects().Where(ss => ss.science > 0.0f && ss.IsFromBody(body) && ss.id.Contains("Flying")).Any();
                        s.scienceFromAtmo = scienceFlying;

                        bool scienceOrbit = ResearchAndDevelopment.GetSubjects().Where(ss => ss.science > 0.0f && ss.IsFromBody(body) && ss.id.Contains("InSpace")).Any();
                        s.scienceFromOrbit = scienceOrbit;

                        KRASHShelter.preSimStatus.Add(s);

                        s = null;
                    }
                }
            }
            Log.Info("KK: EditorLogic.fetch.launchSiteName set to " + site.name);

            //Trick KSP to think that you launched from Runway or LaunchPad
            //I'm sure Squad will break this in the future
            //This only works because they use multiple variables to store the same value, basically its black magic.
            //--medsouz
            if (simType == SimType.LAUNCHPAD || simType == SimType.RUNWAY)
            {
                Log.Info("site.name: " + site.name);
                EditorLogic.fetch.launchSiteName = site.name;
            }

            //			}
        }

        public void LaunchSim()
        {
            Log.Info("LaunchSim");

            KRASHShelter.persistent.SetSuspendUpdate(true);
            KRASHShelter.instance.Activate();
            if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER)                
                Funding.Instance.AddFunds(KRASHShelter.shipCost, TransactionReasons.Any);
            EditorLogic.fetch.launchVessel();
        }

    }
}
