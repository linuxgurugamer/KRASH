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
        public static string Button_Landed = Localizer.Format("#KRASH_Button_Landed"); // Landed
        public static string Button_All = Localizer.Format("#KRASH_Button_All"); // All
        public static string Button_Planets = Localizer.Format("#KRASH_Button_Planets"); // Planets
        public static string Button_Moons = Localizer.Format("#KRASH_Button_Moons"); // Moons
        public static string Button_StartSimulation = Localizer.Format("#KRASH_Button_StartSimulation"); // Start simulation
        public static string Button_DisableTestFlight = Localizer.Format("#KRASH_Button_DisableTestFlight"); // Start simulation
        public static string Button_DisableTestLite = Localizer.Format("#KRASH_Button_DisableTestLite"); // Disable TestLite
        public static string Button_Select = Localizer.Format("#KRASH_Button_Select"); // Select
        public static string Button_Set= Localizer.Format("#KRASH_Button_Set"); // Set
        public static string Button_ResumeSimulation = Localizer.Format("#KRASH_Button_ResumeSimulation"); // Resume Simulation
        public static string Button_TerminateSimulation = Localizer.Format("#KRASH_Button_TerminateSimulation"); // <color=orange>Terminate Simulation</color>
        public static string Button_RestartSimulation = Localizer.Format("#KRASH_Button_RestartSimulation"); // <color=orange>Restart Simulation</color>
        public static string Button_Settings = Localizer.Format("#KRASH_Button_Settings"); // Settings
        public static string CostOptions = Localizer.Format("#KRASH_CostOptions"); // Cost Options
        public static string FlatSetupCosts = Localizer.Format("#KRASH_FlatSetupCosts"); // Flat Setup Costs: 
        public static string FlatPer_minuteCosts = Localizer.Format("#KRASH_FlatPer_minuteCosts"); // Flat Per-minute Costs: 
        public static string PerpartSetupCosts = Localizer.Format("#KRASH_PerpartSetupCosts"); // Per-part Setup Costs:  
        public static string PerpartPerminuteCosts = Localizer.Format("#KRASH_PerpartPerminuteCosts"); // Per-part Per-minute Costs:   
        public static string PertonSetupCosts = Localizer.Format("#KRASH_PertonSetupCosts"); // Per-ton Setup Costs:   
        public static string PertonPerminuteCosts = Localizer.Format("#KRASH_PertonPerminuteCosts"); // Per-ton Per-minute Costs: 
        public static string PercentageSetupCosts = Localizer.Format("#KRASH_PercentageSetupCosts"); // Percentage Setup Costs:     
        public static string PercentagePerminuteCosts = Localizer.Format("#KRASH_PercentagePerminuteCosts"); // Percentage Per-minute Costs:
        public static string AtmosphericMultiplier = Localizer.Format("#KRASH_AtmosphericMultiplier"); // Atmospheric Multiplier:
        public static string TerminationOptions = Localizer.Format("#KRASH_TerminationOptions"); // Termination Options (no data)
        public static string TerminateatSoi = Localizer.Format("#KRASH_TerminateatSoi"); // Terminate at Soi
        public static string TerminateatLand = Localizer.Format("#KRASH_TerminateatLand"); // Terminate at Land
        public static string TerminateatAtmo = Localizer.Format("#KRASH_TerminateatAtmo"); // Terminate at Atmo
        public static string MiscellaneousOptionsGlobal = Localizer.Format("#KRASH_MiscellaneousOptionsGlobal"); // Miscellaneous Options (global)
        public static string Continuesimifnocash = Localizer.Format("#KRASH_Continuesimifnocash"); // Continue sim if no cash
        public static string Obeylaunchpadlimits = Localizer.Format("#KRASH_Obeylaunchpadlimits"); // Obey launch pad limits
        public static string DefaultMaxSimCost = Localizer.Format("#KRASH_DefaultMaxSimCost"); // Default Max Sim Cost: 
        public static string ShowRunningSimCosts = Localizer.Format("#KRASH_ShowRunningSimCosts"); // Show Running Sim Costs 
        public static string Showallbodiesincareer = Localizer.Format("#KRASH_Showallbodiesincareer"); // Show all bodies in career 
        public static string HorizontalPosition = Localizer.Format("#KRASH_HorizontalPosition"); // Horizontal Position: 
        public static string VerticalPosition = Localizer.Format("#KRASH_VerticalPosition"); // Vertical Position:
        public static string LaunchSiteSelector_title = Localizer.Format("#KRASH_LaunchSiteSelector_title"); // Launch Site Selector
        public static string SimulateInfo_NoFundsToSim = Localizer.Format("#KRASH_SimulateInfo_NoFundsToSim"); // Not enough money to start sim
        public static string SimulateInfo_LockedParts = Localizer.Format("#KRASH_SimulateInfo_LockedParts"); // Vessel has locked parts
        public static string SimulateInfo_VesselNotControllable = Localizer.Format("#KRASH_SimulateInfo_VesselNotControllable"); // Vessel is not controllable
        public static string SimulateInfo_SimCostexceedslimit = Localizer.Format("#KRASH_SimulateInfo_SimCostexceedslimit"); // Sim cost exceeds limit
        public static string VesselUnlaunchableReasons = Localizer.Format("#KRASH_VesselUnlaunchableReasons"); // Vessel Unlaunchable for following reason(s):
        public static string SimulationSettings = Localizer.Format("#KRASH_SimulationSettings"); // Simulation Settings
        public static string StartLocation = Localizer.Format("#KRASH_StartLocation"); // Start location:
        public static string Launchpad = Localizer.Format("#KRASH_Launchpad"); // Launchpad
        public static string Runway = Localizer.Format("#KRASH_Runway"); // Runway
        public static string Altitude = Localizer.Format("#KRASH_Altitude"); // Altitude:
        public static string Latitude = Localizer.Format("#KRASH_Latitude"); // Latitude:
        public static string Longitude = Localizer.Format("#KRASH_Longitude"); // Longitude:
        public static string PartCount = Localizer.Format("#KRASH_PartCount"); // Part count:
        public static string DryMass = Localizer.Format("#KRASH_DryMass"); // Dry Mass:
        public static string FuelMass = Localizer.Format("#KRASH_FuelMass"); // Fuel Mass:
        public static string TotalMass = Localizer.Format("#KRASH_TotalMass"); // Total Mass:
        public static string SimulationCosts = Localizer.Format("#KRASH_SimulationCosts"); // Simulation Costs:
        public static string SimSetupCost = Localizer.Format("#KRASH_SimSetupCost"); // Sim setup cost:
        public static string EstSimMinCost = Localizer.Format("#KRASH_EstSimMinCost"); // Est. Sim/min cost:
        public static string EstAtmoSimMinCost = Localizer.Format("#KRASH_EstAtmoSimMinCost"); // Est. Atmo Sim/min cost:
        public static string LimitMaxCosts = Localizer.Format("#KRASH_LimitMaxCosts"); // Limit max costs
        public static string Limit = Localizer.Format("#KRASH_Limit"); // Limit:
        public static string PauseAfterVesselLanded = Localizer.Format("#KRASH_PauseAfterVesselLanded"); // Vessel has landed.  Click the OK button to continue
        public static string GamePaused = Localizer.Format("#KRASH_GamePaused"); // Game Paused
        public static string SpaceplaneHangar = Localizer.Format("#KRASH_SpaceplaneHangar"); // Spaceplane Hangar
        public static string VehicleAssemblyBuilding = Localizer.Format("#KRASH_VehicleAssemblyBuilding"); // Vehicle Assembly Building
        public static string SpaceCenter = Localizer.Format("#KRASH_SpaceCenter"); // Space Center
        public static string PreSimulation = Localizer.Format("#KRASH_PreSimulation"); // Pre-Simulation
        public static string TerminatingSimulation_Msg = Localizer.Format("#KRASH_TerminatingSimulation_Msg"); // Terminating will set the game back to an earlier state. Are you sure you want to continue?
        public static string TerminatingSimulation_title = Localizer.Format("#KRASH_TerminatingSimulation_title"); // Terminating Simulation
        public static string RevertingSimulation_Msg = Localizer.Format("#KRASH_RevertingSimulation_Msg"); // Reverting will set the game back to an earlier state. Are you sure you want to continue?
        public static string RevertingSimulation_title = Localizer.Format("#KRASH_RevertingSimulation_title"); // Reverting Simulation
        public static string SimulationActive = Localizer.Format("#KRASH_SimulationActive"); // Simulation Active
        public static string SimulationEnded = Localizer.Format("#KRASH_SimulationEnded"); // Simulation ended!
        public static string SimulationResult = Localizer.Format("#KRASH_SimulationResult"); // Simulation results!
        public static string SimulationTitle = Localizer.Format("#KRASH_SimulationTitle"); // Sim Costs:

    }
}
