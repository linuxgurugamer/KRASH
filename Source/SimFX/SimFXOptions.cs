using System.Globalization;

namespace KRASH.SimFX
{
    /// <summary>
    /// The effective options for the current game: the per-save values when a game is loaded,
    /// the SimFX/Settings.cfg defaults otherwise. Resolved on every reconcile tick (cheap), so changes made in
    /// Difficulty Options take effect without reloading the scene.
    /// </summary>
    internal sealed class SimFXOptions
    {
        /// <summary>Per-save on/off switch (launch window toggle / Difficulty Options). Off = no profile switch, no screen effect.</summary>
        internal bool UseInSimulations = true;

        /// <summary>Per-save profile choice, or null to use the SimFX/Settings.cfg default.</summary>
        internal string ProfileOverride;
        internal bool ApplyInMap;
        internal bool ApplyInIVA;

        /// <summary>Everything the screen effect does: frame hold, motion blur, pixelation, overlays.</summary>
        internal ScreenLook Look;

        internal static SimFXOptions Resolve(SimFXSettings cfg)
        {
            var o = new SimFXOptions
            {
                ApplyInMap = cfg.ApplyInMap,
                ApplyInIVA = cfg.ApplyInIVA,
            };

            int fps = cfg.FrameRateLimit;
            bool blur = cfg.MotionBlur;
            int shutter = cfg.MotionBlurShutter;
            int resolution = cfg.ResolutionPercent;
            bool scanlines = cfg.Scanlines, band = cfg.RefreshBand;
            int scanStrength = cfg.ScanlineStrength, scanSize = cfg.ScanlineSize;
            int bandStrength = cfg.RefreshBandStrength;

            SimFXGameSettings game = SimFXGameSettings.Current;
            if (game != null)
            {
                o.UseInSimulations = game.useInSimulations;
                string p = game.simProfile;
                o.ProfileOverride = SimFXGameSettings.IsUseDefault(p) ? null : p;
                o.ApplyInMap = game.applyInMap;
                o.ApplyInIVA = game.applyInIVA;
                fps = game.frameRateLimit;
                blur = game.motionBlur;
                shutter = game.motionBlurShutter;
                resolution = game.resolutionPercent;
            }

            SimFXEffectSettings fx = SimFXEffectSettings.Current;
            if (fx != null)
            {
                scanlines = fx.scanlines;
                scanStrength = fx.scanlineOpacity;
                scanSize = fx.scanlineSize;
                band = fx.refreshBand;
                bandStrength = fx.refreshBandOpacity;
            }

            fps = SimFXSettings.Clamp(fps, SimFXSettings.MinFrameRate, SimFXSettings.MaxFrameRate);
            o.Look = new ScreenLook(
                fps,
                fps > 0 && blur ? SimFXSettings.Clamp(shutter, 0, SimFXSettings.MaxShutterAngle) / 360f : 0f,
                SimFXSettings.Clamp(resolution, SimFXSettings.MinResolutionPercent, SimFXSettings.MaxResolutionPercent) / 100f,
                scanlines ? SimFXSettings.Clamp(scanStrength, 0, 100) / 100f : 0f,
                SimFXSettings.Clamp(scanSize, SimFXSettings.MinPatternSize, SimFXSettings.MaxPatternSize),
                band ? SimFXSettings.Clamp(bandStrength, 0, 100) / 100f : 0f);

            return o;
        }

        public override string ToString() =>
            string.Format(CultureInfo.InvariantCulture, "on={0}, profile={1}, map={2}, iva={3}, {4}",
                UseInSimulations, ProfileOverride ?? SimFXGameSettings.UseDefault, ApplyInMap, ApplyInIVA, Look);
    }
}
