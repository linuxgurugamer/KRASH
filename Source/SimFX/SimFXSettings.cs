using System;
using System.IO;

namespace KRASH.SimFX
{
    /// <summary>
    /// Values read from GameData/KRASH/SimFX/Settings.cfg. The file holds plain top-level
    /// "key = value" lines, so it is read with ConfigNode.Load instead of through the GameDatabase.
    /// Missing or malformed values silently keep their defaults; out-of-range values are clamped.
    ///
    /// Everything except enabled/debugLogging is a default: each save can override it under
    /// Difficulty Options > KRASH (see SimFXGameSettings).
    /// </summary>
    internal sealed class SimFXSettings
    {
        internal const string FolderName = "SimFX";
        internal const string FileName = "Settings.cfg";

        // Value ranges shared with the in-game sliders.
        // Percentages are integers: KSP's float sliders round to their display format.
        internal const int MinFrameRate = 0, MaxFrameRate = 60;
        internal const int MinResolutionPercent = 25, MaxResolutionPercent = 100;
        internal const int MinPatternSize = 1, MaxPatternSize = 8;
        internal const int MaxShutterAngle = 360;

        internal bool Enabled { get; private set; } = true;
        internal bool DebugLogging { get; private set; }

        internal string SimProfile { get; private set; } = "KRASH-SimFX";

        /// <summary>Also use the simulation look while the map view is open.</summary>
        internal bool ApplyInMap { get; private set; }

        /// <summary>Also use the simulation look in IVA / internal camera.</summary>
        internal bool ApplyInIVA { get; private set; }

        /// <summary>
        /// Simulated frame rate of the 3D image (0 = off). The game itself keeps running at full speed:
        /// only the scene image is held between updates, so the UI and the screen overlays stay fluid.
        /// </summary>
        internal int FrameRateLimit { get; private set; } = 20;

        /// <summary>
        /// Motion blur of the simulated camera: real frames inside the exposure window of each simulated
        /// frame are averaged. Shutter angle in degrees (180 = half the frame time, 360 = all of it).
        /// Needs a simulated frame rate.
        /// </summary>
        internal bool MotionBlur { get; private set; } = true;
        internal int MotionBlurShutter { get; private set; } = 180;

        /// <summary>Percentage of the screen resolution the 3D view is shown at (100 = off).</summary>
        internal int ResolutionPercent { get; private set; } = 50;

        /// <summary>Dark gaps between lines; strength in %, one line per <see cref="ScanlineSize"/> rendered rows.</summary>
        internal bool Scanlines { get; private set; } = true;
        internal int ScanlineStrength { get; private set; } = 40;
        internal int ScanlineSize { get; private set; } = 2;

        /// <summary>Rolling refresh band of a CRT filmed out of sync; strength in %.</summary>
        internal bool RefreshBand { get; private set; } = true;
        internal int RefreshBandStrength { get; private set; } = 30;

        internal static SimFXSettings Load()
        {
            var s = new SimFXSettings();
            string path = GetSettingsPath();

            try
            {
                if (!File.Exists(path))
                {
                    Log.Warn($"{path} not found; using defaults.");
                    return s;
                }

                ConfigNode node = ConfigNode.Load(path);
                if (node == null)
                {
                    Log.Warn($"Could not parse {path}; using defaults.");
                    return s;
                }

                s.Enabled = ReadBool(node, "enabled", s.Enabled);
                s.DebugLogging = ReadBool(node, "debugLogging", s.DebugLogging);
                s.ApplyInMap = ReadBool(node, "applyInMap", s.ApplyInMap);
                s.ApplyInIVA = ReadBool(node, "applyInIVA", s.ApplyInIVA);

                string profile = node.GetValue("simProfile");
                if (!string.IsNullOrEmpty(profile)) s.SimProfile = profile.Trim();

                s.FrameRateLimit = Clamp(ReadInt(node, "frameRateLimit", s.FrameRateLimit), MinFrameRate, MaxFrameRate);
                s.MotionBlur = ReadBool(node, "motionBlur", s.MotionBlur);
                s.MotionBlurShutter = Clamp(ReadInt(node, "motionBlurShutter", s.MotionBlurShutter), 0, MaxShutterAngle);
                s.ResolutionPercent = Clamp(ReadInt(node, "resolutionPercent", s.ResolutionPercent), MinResolutionPercent, MaxResolutionPercent);

                s.Scanlines = ReadBool(node, "scanlines", s.Scanlines);
                s.ScanlineStrength = Clamp(ReadInt(node, "scanlineStrength", s.ScanlineStrength), 0, 100);
                s.ScanlineSize = Clamp(ReadInt(node, "scanlineSize", s.ScanlineSize), MinPatternSize, MaxPatternSize);

                s.RefreshBand = ReadBool(node, "refreshBand", s.RefreshBand);
                s.RefreshBandStrength = Clamp(ReadInt(node, "refreshBandStrength", s.RefreshBandStrength), 0, 100);
            }
            catch (Exception e)
            {
                Log.Warn($"Failed to read {path}, using defaults: {e.Message}");
            }

            return s;
        }

        public override string ToString() =>
            $"enabled={Enabled}, simProfile={SimProfile}, debugLogging={DebugLogging}";

        private static bool ReadBool(ConfigNode node, string key, bool fallback)
        {
            bool value = fallback;
            return node.TryGetValue(key, ref value) ? value : fallback;
        }

        private static int ReadInt(ConfigNode node, string key, int fallback)
        {
            int value = fallback;
            return node.TryGetValue(key, ref value) ? value : fallback;
        }

        internal static int Clamp(int v, int min, int max) => v < min ? min : v > max ? max : v;

        /// <summary>SimFX/Settings.cfg sits in KRASH's mod folder, one level above Plugins/ where this DLL was loaded from.</summary>
        private static string GetSettingsPath()
        {
            try
            {
                string pluginDir = Path.GetDirectoryName(typeof(SimFXSettings).Assembly.Location);
                string modDir = Path.GetDirectoryName(pluginDir);
                if (!string.IsNullOrEmpty(modDir))
                    return Path.Combine(Path.Combine(modDir, FolderName), FileName);
            }
            catch (Exception)
            {
                // Fall through to the conventional location.
            }

            return Path.Combine(KSPUtil.ApplicationRootPath, "GameData", "KRASH", FolderName, FileName);
        }
    }
}
