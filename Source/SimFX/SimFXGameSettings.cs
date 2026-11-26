using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using KSP.Localization;

namespace KRASH.SimFX
{
    /// <summary>
    /// Per-save options for the simulation visuals, shown under Settings > Difficulty Options > KRASH,
    /// in two columns next to KRASH's own (KRASH_Settings is column 1).
    /// New saves, and saves created before an option existed, start from the SimFX/Settings.cfg defaults.
    ///
    /// These nodes are stored in the .sfs like any GameParameters node. Changes made outside a
    /// simulation persist normally. During a simulation the pause menu does not expose this screen,
    /// and KRASH reloads the pre-sim save when the sim ends anyway.
    ///
    /// Percentages are int fields on purpose: KSP's float sliders round the stored value to their
    /// display format, so a 0..1 float could only be 0 or 1.
    /// </summary>
    public sealed class SimFXGameSettings : GameParameters.CustomParameterNode
    {
        private const string UseDefaultPrefix = "Default (";
        private const string UseDefaultSuffix = ")";
        private static string useDefault;

        /// <summary>
        /// Sentinel value: use simProfile from SimFX/Settings.cfg, shown with that profile's name, e.g.
        /// "Default (KRASH-SimFX)". The difficulty screen shows the stored string as-is, so this is also the
        /// label. It is stored in the save, so it is deliberately not localized. Any "Default (...)" value
        /// counts as the sentinel, so editing simProfile later does not turn old saves into a missing profile.
        /// </summary>
        internal static string UseDefault =>
            useDefault ?? (useDefault = UseDefaultPrefix + SimFXSettings.Load().SimProfile + UseDefaultSuffix);

        /// <summary>
        /// Older forms of the sentinel: "Settings.cfg default" was written by the standalone KRASH-SimFX mod
        /// (same node name), "Default" by development builds of the built-in version.
        /// </summary>
        private static readonly string[] LegacyUseDefault = { "Settings.cfg default", "Default" };

        internal static bool IsUseDefault(string profile) =>
            string.IsNullOrEmpty(profile)
            || (profile.StartsWith(UseDefaultPrefix, StringComparison.Ordinal) && profile.EndsWith(UseDefaultSuffix, StringComparison.Ordinal))
            || Array.IndexOf(LegacyUseDefault, profile) >= 0;

        private static bool? columnsVisible;

        /// <summary>
        /// Both SimFX columns are hidden when the visuals are off in SimFX/Settings.cfg, or while the standalone
        /// KRASH-SimFX mod is installed (they would do nothing; it shows its own section). GameMode only filters
        /// the Difficulty Options screen, so the values are still saved and loaded either way.
        /// Checked once: the difficulty screen only exists after all assemblies are loaded.
        /// </summary>
        internal static GameParameters.GameMode ColumnsGameMode =>
            Usable ? GameParameters.GameMode.ANY : GameParameters.GameMode.NONE;

        /// <summary>
        /// False when the visuals are off in SimFX/Settings.cfg or the standalone mod is installed: then the
        /// columns and the launch window toggle are hidden.
        /// </summary>
        internal static bool Usable
        {
            get
            {
                if (columnsVisible == null)
                    columnsVisible = !ReflectionUtil.StandaloneSimFXInstalled && SimFXSettings.Load().Enabled;
                return columnsVisible.Value;
            }
        }

        public override string Title => Localizer.Format("#KRASH_SimFX_Settings_Title");
        public override string Section => "KRASH";
        public override string DisplaySection => "KRASH";
        public override int SectionOrder => 2;
        public override GameParameters.GameMode GameMode => ColumnsGameMode;
        public override bool HasPresets => false;

        /// <summary>
        /// The on/off switch. Also shown as a toggle in KRASH's simulation launch window, which edits this same
        /// field, so the next simulation starts with the last choice made for this save.
        /// </summary>
        [GameParameters.CustomParameterUI("#KRASH_SimFX_Settings_useInSimulations", // "Use simulation visuals"
            toolTip = "#KRASH_SimFX_Settings_useInSimulations_tooltip")]
        public bool useInSimulations = true;

        [GameParameters.CustomParameterUI("#KRASH_SimFX_Settings_simProfile", // "TUFX profile"
            toolTip = "#KRASH_SimFX_Settings_simProfile_tooltip")]
        public string simProfile = UseDefault;

        [GameParameters.CustomParameterUI("#KRASH_SimFX_Settings_applyInMap", // "Use in map view"
            toolTip = "#KRASH_SimFX_Settings_applyInMap_tooltip")]
        public bool applyInMap;

        [GameParameters.CustomParameterUI("#KRASH_SimFX_Settings_applyInIVA", // "Use in IVA"
            toolTip = "#KRASH_SimFX_Settings_applyInIVA_tooltip")]
        public bool applyInIVA;

        [GameParameters.CustomIntParameterUI("#KRASH_SimFX_Settings_frameRate", // "Simulated frame rate (0 = off)"
            minValue = SimFXSettings.MinFrameRate, maxValue = SimFXSettings.MaxFrameRate, stepSize = 5,
            toolTip = "#KRASH_SimFX_Settings_frameRate_tooltip")]
        public int frameRateLimit;

        [GameParameters.CustomParameterUI("#KRASH_SimFX_Settings_motionBlur", // "Motion blur"
            toolTip = "#KRASH_SimFX_Settings_motionBlur_tooltip")]
        public bool motionBlur;

        [GameParameters.CustomIntParameterUI("#KRASH_SimFX_Settings_motionBlurShutter", // "Motion blur shutter (°)"
            minValue = 0, maxValue = SimFXSettings.MaxShutterAngle, stepSize = 15,
            toolTip = "#KRASH_SimFX_Settings_motionBlurShutter_tooltip")]
        public int motionBlurShutter;

        [GameParameters.CustomIntParameterUI("#KRASH_SimFX_Settings_resolution", // "Render resolution (%)"
            minValue = SimFXSettings.MinResolutionPercent, maxValue = SimFXSettings.MaxResolutionPercent, stepSize = 5,
            toolTip = "#KRASH_SimFX_Settings_resolution_tooltip")]
        public int resolutionPercent;

        public SimFXGameSettings()
        {
            SimFXSettings defaults = SimFXSettings.Load();
            applyInMap = defaults.ApplyInMap;
            applyInIVA = defaults.ApplyInIVA;
            frameRateLimit = defaults.FrameRateLimit;
            motionBlur = defaults.MotionBlur;
            motionBlurShutter = defaults.MotionBlurShutter;
            resolutionPercent = defaults.ResolutionPercent;
        }

        /// <summary>
        /// Maps any form of the sentinel to the current one: the standalone mod's, older builds', or a
        /// "Default (...)" naming a profile that simProfile in Settings.cfg no longer names.
        /// </summary>
        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);
            if (IsUseDefault(simProfile))
                simProfile = UseDefault;
        }

        /// <summary>The TUFX profile option is hidden entirely when TUFX is not installed.</summary>
        public override bool Enabled(MemberInfo member, GameParameters parameters)
        {
            if (member.Name == nameof(simProfile))
                return TufxBridge.Available;
            return base.Enabled(member, parameters);
        }

        /// <summary>
        /// Everything below the on/off switch is greyed out while it is off. Motion blur needs a simulated
        /// frame rate; its shutter also needs motion blur on.
        /// </summary>
        public override bool Interactible(MemberInfo member, GameParameters parameters)
        {
            if (member.Name != nameof(useInSimulations) && !useInSimulations)
                return false;

            switch (member.Name)
            {
                case nameof(motionBlur):
                    return frameRateLimit > 0;
                case nameof(motionBlurShutter):
                    return frameRateLimit > 0 && motionBlur;
                default:
                    return base.Interactible(member, parameters);
            }
        }

        /// <summary>Offers the sentinel plus every profile TUFX has loaded.</summary>
        public override IList ValidValues(MemberInfo member)
        {
            if (member.Name != nameof(simProfile))
                return base.ValidValues(member);

            var values = new List<string> { UseDefault };
            values.AddRange(TufxBridge.ProfileNames());

            // Keep a saved choice selectable even if that profile has since been removed.
            if (!IsUseDefault(simProfile) && !values.Contains(simProfile))
                values.Add(simProfile);

            return values;
        }

        internal static SimFXGameSettings Current => HighLogic.CurrentGame?.Parameters?.CustomParams<SimFXGameSettings>();
    }

    /// <summary>Third column: CRT-style overlays drawn after all post-processing.</summary>
    public sealed class SimFXEffectSettings : GameParameters.CustomParameterNode
    {
        public override string Title => Localizer.Format("#KRASH_SimFX_Effects_Title");
        public override string Section => "KRASH";
        public override string DisplaySection => "KRASH";
        public override int SectionOrder => 3;
        public override GameParameters.GameMode GameMode => SimFXGameSettings.ColumnsGameMode;
        public override bool HasPresets => false;

        [GameParameters.CustomParameterUI("#KRASH_SimFX_Effects_scanlines", // "Scanlines"
            toolTip = "#KRASH_SimFX_Effects_scanlines_tooltip")]
        public bool scanlines;

        [GameParameters.CustomIntParameterUI("#KRASH_SimFX_Effects_scanlineStrength", // "Scanline strength (%)"
            minValue = 0, maxValue = 100, stepSize = 5)]
        public int scanlineOpacity;

        [GameParameters.CustomIntParameterUI("#KRASH_SimFX_Effects_scanlineSize", // "Rows per scanline"
            minValue = SimFXSettings.MinPatternSize, maxValue = SimFXSettings.MaxPatternSize,
            toolTip = "#KRASH_SimFX_Effects_scanlineSize_tooltip")]
        public int scanlineSize;

        [GameParameters.CustomParameterUI("#KRASH_SimFX_Effects_refreshBand", // "Rolling refresh band"
            toolTip = "#KRASH_SimFX_Effects_refreshBand_tooltip")]
        public bool refreshBand;

        [GameParameters.CustomIntParameterUI("#KRASH_SimFX_Effects_refreshBandStrength", // "Refresh band strength (%)"
            minValue = 0, maxValue = 100, stepSize = 5)]
        public int refreshBandOpacity;

        public SimFXEffectSettings()
        {
            SimFXSettings defaults = SimFXSettings.Load();
            scanlines = defaults.Scanlines;
            scanlineOpacity = defaults.ScanlineStrength;
            scanlineSize = defaults.ScanlineSize;
            refreshBand = defaults.RefreshBand;
            refreshBandOpacity = defaults.RefreshBandStrength;
        }

        /// <summary>Greys out everything while the simulation visuals are off, and the sliders of a disabled effect.</summary>
        public override bool Interactible(MemberInfo member, GameParameters parameters)
        {
            SimFXGameSettings main = parameters?.CustomParams<SimFXGameSettings>();
            if (main != null && !main.useInSimulations)
                return false;

            switch (member.Name)
            {
                case nameof(scanlineOpacity):
                case nameof(scanlineSize):
                    return scanlines;
                case nameof(refreshBandOpacity):
                    return refreshBand;
                default:
                    return base.Interactible(member, parameters);
            }
        }

        internal static SimFXEffectSettings Current => HighLogic.CurrentGame?.Parameters?.CustomParams<SimFXEffectSettings>();
    }
}
