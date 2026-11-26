using System;
using System.Collections;
using UnityEngine;

namespace KRASH.SimFX
{
    /// <summary>
    /// Simulation visuals: while a KRASH simulation runs in the flight scene, the flight is shown as if
    /// watched on a training simulator's CRT display. That is the TUFX profile (when TUFX is installed),
    /// plus a screen effect that simulates a low-frame-rate camera (frame hold and motion blur) watched on
    /// a CRT (pixelation, scanlines, rolling refresh band).
    ///
    /// A fresh instance is created for every flight scene (including "Restart Simulation", which reloads
    /// the scene via FlightDriver.RevertToLaunch). All state is in memory only: nothing is written to
    /// TUFX settings or the save game.
    ///
    /// Triggers:
    ///  1. KRASHPersistent.shelterSimulationActive, read every frame (a plain field read). A change starts
    ///     or stops the look on that frame.
    ///  2. GameEvents.OnCameraChange / onLevelWasLoaded: TUFX re-applies the configured profile on
    ///     these, so we re-check one frame later, after TUFX has run.
    ///  3. A 1 Hz reconcile that also picks up option changes and anything that replaced our profile.
    /// </summary>
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public sealed class SimFXController : MonoBehaviour
    {
        private const float ReconcileIntervalSeconds = 1f;

        private SimFXSettings settings;
        private bool tufx;             // TUFX present and initialized: profile switching available
        private bool running;          // initialized, listeners registered
        private bool stopped;          // scene is unloading: never re-apply
        private bool reconcileQueued;
        private bool lastSimActive;

        // Restore state. 'applied' guards the profile restore, which makes Restore() idempotent.
        private bool applied;
        private string originalProfile;
        private string lastOptions;

        private readonly ScreenEffectHost screen = new ScreenEffectHost();
        private bool effectsOn;        // screen effect currently attached (for the one-time log line)
        private bool loggedTurnedOff;  // "turned off for this save" already logged

        // Avoid repeating the same warning every reconcile tick.
        private string warnedMissingOverride;
        private bool warnedMissingDefault;

        #region Unity lifecycle

        private void Start()
        {
            settings = SimFXSettings.Load();
            Log.DebugEnabled = settings.DebugLogging;
            Log.Detail("Settings: " + settings);

            if (!settings.Enabled)
            {
                Log.Detail("Disabled in SimFX/Settings.cfg.");
                Destroy(this);
                return;
            }

            // SimFXStartup already logged this once at the main menu.
            if (ReflectionUtil.StandaloneSimFXInstalled)
            {
                Destroy(this);
                return;
            }

            // TUFX is optional: without it only the profile switch is skipped.
            tufx = TufxBridge.Available && TufxBridge.Ready;
            if (!tufx)
                Log.Detail(TufxBridge.Available
                    ? "TUFX is not initialized in this scene; profile switching is off, screen effects stay on."
                    : "TUFX not installed; profile switching is off, screen effects stay on.");
            else
                Log.Detail("Simulation profile for this save: " + (ResolveSimProfile(SimFXOptions.Resolve(settings)) ?? "(none, profile switching off)"));

            GameEvents.OnCameraChange.Add(OnCameraChange);
            GameEvents.onLevelWasLoaded.Add(OnLevelWasLoaded);
            GameEvents.onGameSceneLoadRequested.Add(OnGameSceneLoadRequested);
            running = true;

            // First check on the next frame so TUFX's own scene-load application has already happened.
            lastSimActive = IsSimulationActive();
            QueueReconcile();
            InvokeRepeating(nameof(Reconcile), ReconcileIntervalSeconds, ReconcileIntervalSeconds);
        }

        private void Update()
        {
            if (!running || stopped)
                return;

            bool simActive = IsSimulationActive();
            if (simActive == lastSimActive)
                return;

            lastSimActive = simActive;
            Log.Detail("Simulation " + (simActive ? "started" : "ended") + ".");
            Reconcile();
        }

        private void OnDestroy()
        {
            CancelInvoke();

            if (running)
            {
                GameEvents.OnCameraChange.Remove(OnCameraChange);
                GameEvents.onLevelWasLoaded.Remove(OnLevelWasLoaded);
                GameEvents.onGameSceneLoadRequested.Remove(OnGameSceneLoadRequested);
            }

            StopAll("flight scene destroyed");
            running = false;
        }

        #endregion

        #region Event handlers

        private void OnGameSceneLoadRequested(GameScenes scene)
        {
            try
            {
                Log.Detail("Scene load requested: " + scene);
                CancelInvoke();
                StopAll("leaving flight scene");
            }
            catch (Exception e)
            {
                Log.Error("OnGameSceneLoadRequested: " + e);
            }
        }

        private void OnCameraChange(CameraManager.CameraMode mode) => QueueReconcile();

        private void OnLevelWasLoaded(GameScenes scene) => QueueReconcile();

        #endregion

        #region Core logic

        /// <summary>
        /// Whether KRASH considers a simulation active. Set by KRASH.Activate() before the launch, cleared by
        /// KRASH.Deactivate() when the pre-sim save is restored. (APIManager.simAPI.simulationActive() is not
        /// used: it dereferences simPauseMenuInstance, which is not always set in the flight scene.)
        /// </summary>
        private static bool IsSimulationActive()
        {
            KRASHPersistent persistent = KRASHShelter.persistent;
            return persistent != null && persistent.shelterSimulationActive;
        }

        /// <summary>Defers a reconcile by one frame (coalescing repeated requests) so it runs after TUFX.</summary>
        private void QueueReconcile()
        {
            if (!running || stopped || reconcileQueued || !isActiveAndEnabled)
                return;
            reconcileQueued = true;
            StartCoroutine(ReconcileNextFrame());
        }

        private IEnumerator ReconcileNextFrame()
        {
            yield return null;
            reconcileQueued = false;
            Reconcile();
        }

        /// <summary>Brings the TUFX profile and the screen effect in line with the simulation state. Idempotent and cheap.</summary>
        private void Reconcile()
        {
            if (!running || stopped)
                return;

            try
            {
                if (!IsSimulationActive())
                {
                    StopEffects();
                    Restore("simulation no longer active");
                    return;
                }

                // Resolved every tick, so changes in Difficulty Options are picked up without a scene reload.
                SimFXOptions options = SimFXOptions.Resolve(settings);
                string optionsText = options.ToString();
                if (optionsText != lastOptions)
                {
                    lastOptions = optionsText;
                    Log.Detail("Options: " + optionsText);
                }

                if (!options.UseInSimulations)
                {
                    StopEffects();
                    Restore("simulation visuals turned off for this save");
                    if (!loggedTurnedOff)
                    {
                        loggedTurnedOff = true;
                        Log.Info("Simulation active, but simulation visuals are turned off for this save (launch window / Difficulty Options).");
                    }
                    return;
                }
                loggedTurnedOff = false;

                string scene = CurrentView();
                if (!IsSceneWanted(scene, options))
                {
                    StopEffects();
                    Restore($"camera switched to {scene}");
                    return;
                }

                if (tufx)
                    ApplySimProfile(scene, options);

                // Frame hold, motion blur, pixelation and overlays all live in the screen effect (no TUFX needed).
                screen.Update(scene, options.Look);
                if (!effectsOn)
                {
                    effectsOn = true;
                    Log.Info($"Simulation active: screen effects on ({scene}){(tufx ? "" : "; TUFX not installed, no profile switch")}.");
                }
            }
            catch (Exception e)
            {
                Log.Error("Reconcile: " + (e.InnerException ?? e));
            }
        }

        /// <summary>Shows the simulation TUFX profile (only called when TUFX is available).</summary>
        private void ApplySimProfile(string scene, SimFXOptions options)
        {
            string baseProfile = ResolveSimProfile(options);
            if (baseProfile == null)
            {
                Restore("no usable simulation profile");
                return;
            }

            string current = TufxBridge.CurrentProfileName;

            if (!applied)
            {
                // If something already shows the sim profile, remember the player's configured one instead.
                originalProfile = current == baseProfile ? TufxBridge.ConfiguredProfileFor(scene) : current;
                applied = true;
                Log.Info($"Simulation active: TUFX profile '{originalProfile}' -> '{baseProfile}' ({scene}).");
            }

            // Re-apply when TUFX replaced it (camera change) or the per-save choice changed.
            if (current != baseProfile)
            {
                if (!TufxBridge.Apply(baseProfile, scene))
                    Log.Warn($"Could not apply TUFX profile '{baseProfile}'.");
                else
                    Log.Detail($"Applied '{baseProfile}' (was '{current}', scene {scene}).");
            }
        }

        private void StopEffects()
        {
            screen.Detach();
            effectsOn = false;
        }

        /// <summary>Ends everything for this scene: profile and screen effect.</summary>
        private void StopAll(string reason)
        {
            stopped = true;
            StopEffects();
            Restore(reason);
        }

        /// <summary>
        /// The view the flight camera is in, with the same mapping TUFX uses (TUFX.Utils.GetTUFXSceneForCameraMode),
        /// computed here so that it also works without TUFX.
        /// </summary>
        private static string CurrentView()
        {
            CameraManager cm = CameraManager.Instance;
            if (cm == null)
                return TufxBridge.SceneFlight;

            switch (cm.currentCameraMode)
            {
                case CameraManager.CameraMode.Map:
                    return TufxBridge.SceneMap;
                case CameraManager.CameraMode.Internal:
                case CameraManager.CameraMode.IVA:
                    return TufxBridge.SceneInternal;
                default:
                    return TufxBridge.SceneFlight;
            }
        }

        /// <summary>
        /// Puts back the profile the player had. Safe to call any number of times.
        /// The target is TUFX's configured profile for the current camera. That is the remembered original
        /// unless the camera mode changed or the player picked another profile in the TUFX window mid-sim,
        /// and in both of those cases the configured one is the right choice. The remembered original is the fallback.
        /// </summary>
        private void Restore(string reason)
        {
            if (!applied)
                return;
            applied = false;

            try
            {
                string scene = CurrentView(); // null-safe, also during teardown
                string target = TufxBridge.ConfiguredProfileFor(scene);
                if (!TufxBridge.HasProfile(target))
                    target = originalProfile;

                if (TufxBridge.CurrentProfileName == target)
                {
                    Log.Info($"Restored TUFX profile '{target}' ({reason}; already active).");
                    return;
                }

                if (TufxBridge.Apply(target, scene))
                    Log.Info($"Restored TUFX profile '{target}' ({reason}).");
                else
                    Log.Warn($"Could not restore TUFX profile '{target}' ({reason}); TUFX will reapply it on the next scene load.");
            }
            catch (Exception e)
            {
                // Typically cameras being torn down during a scene switch; TUFX's onLevelWasLoaded
                // re-applies the configured profile in the next scene anyway.
                Log.Warn($"Restore during '{reason}' failed: {(e.InnerException ?? e).Message}");
            }
        }

        /// <summary>
        /// The base profile to show during the simulation: the per-save choice (Difficulty Options) if it names
        /// a loaded TUFX profile, otherwise simProfile from SimFX/Settings.cfg. Null when neither exists.
        /// </summary>
        private string ResolveSimProfile(SimFXOptions options)
        {
            string perSave = options.ProfileOverride;
            if (perSave != null)
            {
                if (TufxBridge.HasProfile(perSave))
                    return perSave;

                if (warnedMissingOverride != perSave)
                {
                    warnedMissingOverride = perSave;
                    Log.Warn($"TUFX profile '{perSave}' chosen for this save was not found; using the SimFX/Settings.cfg default '{settings.SimProfile}'.");
                }
            }

            if (TufxBridge.HasProfile(settings.SimProfile))
                return settings.SimProfile;

            if (!warnedMissingDefault)
            {
                warnedMissingDefault = true;
                Log.Warn($"TUFX profile '{settings.SimProfile}' not found (check GameData/KRASH/SimFX/Profiles and simProfile in SimFX/Settings.cfg); profile switching is off.");
            }
            return null;
        }

        private static bool IsSceneWanted(string scene, SimFXOptions options)
        {
            switch (scene)
            {
                case TufxBridge.SceneFlight: return true;
                case TufxBridge.SceneMap: return options.ApplyInMap;
                case TufxBridge.SceneInternal: return options.ApplyInIVA;
                default: return false;
            }
        }

        #endregion
    }
}
