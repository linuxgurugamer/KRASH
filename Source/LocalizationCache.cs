using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KSP.Localization;

namespace KRASH
{
    // Store non-parametrized localized strings
    public static class LocalizationCache
    {
        public static string ConfigWindow_title = Localizer.Format("#KRASH_ConfigWindow_title"); // KRASH Config Window
        public static string SelectedCFG = Localizer.Format("#KRASH_SelectedCFG"); // Selected Cfg
        public static string NotCareer = Localizer.Format("#KRASH_NotCareer"); // Configs don't apply to Sandbox or Science games
        public static string ConfigName = Localizer.Format("#KRASH_ConfigName"); // Config name: 
        public static string ConfigNameNotGood = Localizer.Format("#KRASH_ConfigNameNotGood"); // <-----> Must be renamed to save
        public static string Button_Save = Localizer.Format("#KRASH_Button_Save"); // Save
        public static string Button_Accept = Localizer.Format("#KRASH_Button_Accept"); // Accept
        public static string Button_Reset = Localizer.Format("#KRASH_Button_Reset"); // Reset
        public static string Button_Cancel = Localizer.Format("#KRASH_Button_Cancel"); // Cancel
        public static string Button_Delete = Localizer.Format("#KRASH_Button_Delete"); // Delete
        public static string Button_OrbitSelection = Localizer.Format("#KRASH_Button_OrbitSelection"); // Orbit selection
        public static string Button_Landed = Localizer.Format("#KRASH_Button_Landed");
        public static string Button_All = Localizer.Format("#KRASH_Button_All");
        public static string Button_Planets = Localizer.Format("#KRASH_Button_Planets");
        public static string Button_Moons = Localizer.Format("#KRASH_Button_Moons");
        public static string Button_StartSimulation = Localizer.Format("#KRASH_Button_StartSimulation");
        public static string Button_DisableTestFlight = Localizer.Format("#KRASH_Button_DisableTestFlight");
        public static string Button_DisableTestLite = Localizer.Format("#KRASH_Button_DisableTestLite");
        public static string Button_Select = Localizer.Format("#KRASH_Button_Select");
        public static string Button_Set= Localizer.Format("#KRASH_Button_Set");
        public static string Button_ResumeSimulation = Localizer.Format("#KRASH_Button_ResumeSimulation");
        public static string Button_TerminateSimulation = Localizer.Format("#KRASH_Button_TerminateSimulation");
        public static string Button_RestartSimulation = Localizer.Format("#KRASH_Button_RestartSimulation");
        public static string Button_Settings = Localizer.Format("#KRASH_Button_Settings");
        public static string CostOptions = Localizer.Format("#KRASH_CostOptions");
        public static string FlatSetupCosts = Localizer.Format("#KRASH_FlatSetupCosts");
        public static string FlatPer_minuteCosts = Localizer.Format("#KRASH_FlatPer_minuteCosts");
        public static string PerpartSetupCosts = Localizer.Format("#KRASH_PerpartSetupCosts");
        public static string PerpartPerminuteCosts = Localizer.Format("#KRASH_PerpartPerminuteCosts");
        public static string PertonSetupCosts = Localizer.Format("#KRASH_PertonSetupCosts");
        public static string PertonPerminuteCosts = Localizer.Format("#KRASH_PertonPerminuteCosts");
        public static string PercentageSetupCosts = Localizer.Format("#KRASH_PercentageSetupCosts");
        public static string PercentagePerminuteCosts = Localizer.Format("#KRASH_PercentagePerminuteCosts");
        public static string AtmosphericMultiplier = Localizer.Format("#KRASH_AtmosphericMultiplier");
        public static string TerminationOptions = Localizer.Format("#KRASH_TerminationOptions");
        public static string TerminateatSoi = Localizer.Format("#KRASH_TerminateatSoi");
        public static string TerminateatLand = Localizer.Format("#KRASH_TerminateatLand");
        public static string TerminateatAtmo = Localizer.Format("#KRASH_TerminateatAtmo");
        public static string MiscellaneousOptionsGlobal = Localizer.Format("#KRASH_MiscellaneousOptionsGlobal");
        public static string Continuesimifnocash = Localizer.Format("#KRASH_Continuesimifnocash");
        public static string Obeylaunchpadlimits = Localizer.Format("#KRASH_Obeylaunchpadlimits");
        public static string DefaultMaxSimCost = Localizer.Format("#KRASH_DefaultMaxSimCost");
        public static string ShowRunningSimCosts = Localizer.Format("#KRASH_ShowRunningSimCosts");
        public static string Showallbodiesincareer = Localizer.Format("#KRASH_Showallbodiesincareer");
        public static string HorizontalPosition = Localizer.Format("#KRASH_HorizontalPosition");
        public static string VerticalPosition = Localizer.Format("#KRASH_VerticalPosition");
        public static string LaunchSiteSelector_title = Localizer.Format("#KRASH_LaunchSiteSelector_title");
        public static string SimulateInfo_NoFundsToSim = Localizer.Format("#KRASH_SimulateInfo_NoFundsToSim");
        public static string SimulateInfo_LockedParts = Localizer.Format("#KRASH_SimulateInfo_LockedParts");
        public static string SimulateInfo_VesselNotControllable = Localizer.Format("#KRASH_SimulateInfo_VesselNotControllable");
        public static string SimulateInfo_SimCostexceedslimit = Localizer.Format("#KRASH_SimulateInfo_SimCostexceedslimit");
        public static string VesselUnlaunchableReasons = Localizer.Format("#KRASH_VesselUnlaunchableReasons");
        public static string SimulationSettings = Localizer.Format("#KRASH_SimulationSettings");
        public static string StartLocation = Localizer.Format("#KRASH_StartLocation");
        public static string Launchpad = Localizer.Format("#KRASH_Launchpad");
        public static string Runway = Localizer.Format("#KRASH_Runway");
        public static string Altitude = Localizer.Format("#KRASH_Altitude");
        public static string Latitude = Localizer.Format("#KRASH_Latitude");
        public static string Longitude = Localizer.Format("#KRASH_Longitude");
        public static string PartCount = Localizer.Format("#KRASH_PartCount");
        public static string DryMass = Localizer.Format("#KRASH_DryMass");
        public static string FuelMass = Localizer.Format("#KRASH_FuelMass");
        public static string TotalMass = Localizer.Format("#KRASH_TotalMass");
        public static string SimulationCosts = Localizer.Format("#KRASH_SimulationCosts");
        public static string SimSetupCost = Localizer.Format("#KRASH_SimSetupCost");
        public static string EstSimMinCost = Localizer.Format("#KRASH_EstSimMinCost");
        public static string EstAtmoSimMinCost = Localizer.Format("#KRASH_EstAtmoSimMinCost");
        public static string LimitMaxCosts = Localizer.Format("#KRASH_LimitMaxCosts");
        public static string Limit = Localizer.Format("#KRASH_Limit");
        public static string PauseAfterVesselLanded = Localizer.Format("#KRASH_PauseAfterVesselLanded");
        public static string GamePaused = Localizer.Format("#KRASH_GamePaused");
        public static string SpaceplaneHangar = Localizer.Format("#KRASH_SpaceplaneHangar");
        public static string VehicleAssemblyBuilding = Localizer.Format("#KRASH_VehicleAssemblyBuilding");
        public static string SpaceCenter = Localizer.Format("#KRASH_SpaceCenter");
        public static string PreSimulation = Localizer.Format("#KRASH_PreSimulation");
        public static string TerminatingSimulation_Msg = Localizer.Format("#KRASH_TerminatingSimulation_Msg");
        public static string TerminatingSimulation_title = Localizer.Format("#KRASH_TerminatingSimulation_title");
        public static string RevertingSimulation_Msg = Localizer.Format("#KRASH_RevertingSimulation_Msg");
        public static string RevertingSimulation_title = Localizer.Format("#KRASH_RevertingSimulation_title");
        public static string SimulationActive = Localizer.Format("#KRASH_SimulationActive");
        public static string SimulationEnded = Localizer.Format("#KRASH_SimulationEnded");
        public static string SimulationResult = Localizer.Format("#KRASH_SimulationResult");
        public static string SimulationTitle = Localizer.Format("#KRASH_SimulationTitle");

    }
}
