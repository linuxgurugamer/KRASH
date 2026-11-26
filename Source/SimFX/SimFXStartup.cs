using System;
using KSP.Localization;
using UnityEngine;

namespace KRASH.SimFX
{
    /// <summary>
    /// Runs once when the main menu loads (all mod assemblies and TUFX profiles are loaded by then).
    /// It reports in KSP.log what the simulation visuals will do, so problems show up at startup rather
    /// than only after the first flight. TUFX is optional and only enables the profile switch.
    /// </summary>
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public sealed class SimFXStartup : MonoBehaviour
    {
        private void Start()
        {
            try
            {
                SimFXSettings settings = SimFXSettings.Load();
                Log.DebugEnabled = settings.DebugLogging;

                if (!settings.Enabled)
                {
                    Log.Info("Disabled in SimFX/Settings.cfg.");
                }
                else if (ReflectionUtil.StandaloneSimFXInstalled)
                {
                    Log.Warn("The standalone KRASH-SimFX mod is installed. Its features are now part of KRASH, so the built-in "
                           + "simulation visuals stay off to avoid applying them twice. Delete GameData/KRASH-SimFX to use the built-in version.");

                    // Players rarely read KSP.log, and while both are installed the standalone mod loses its
                    // per-save options (both use the same save node names), so say it on screen too.
                    PopupDialog.SpawnPopupDialog(
                        new Vector2(0.5f, 0.5f),
                        new Vector2(0.5f, 0.5f),
                        "KRASHSimFXStandalone",
                        Localizer.Format("#KRASH_SimFX_Standalone_Title"),
                        Localizer.Format("#KRASH_SimFX_Standalone_Message"),
                        Localizer.Format("#KRASH_SimFX_Standalone_OK"),
                        false,
                        HighLogic.UISkin);
                }
                else
                {
                    string profile;
                    if (!TufxBridge.Available)
                    {
                        profile = "TUFX not installed: profile switching off, screen effects on";
                        Log.Info($"{TufxBridge.UnavailableReason}; the TUFX profile is not switched, screen effects still work.");
                    }
                    else if (!TufxBridge.HasProfile(settings.SimProfile))
                    {
                        profile = $"default profile '{settings.SimProfile}' missing";
                        Log.Warn($"Default TUFX profile '{settings.SimProfile}' (SimFX/Settings.cfg) not found; only saves that pick an existing profile in Difficulty Options > KRASH will switch it. Screen effects still work.");
                    }
                    else
                    {
                        profile = "profile " + settings.SimProfile;
                    }

                    Log.Info($"Ready ({profile}).");
                }
            }
            catch (Exception e)
            {
                Log.Error("Startup check failed: " + e);
            }

            Destroy(this);
        }
    }
}
