using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace KRASH.SimFX
{
    /// <summary>
    /// Reflection bridge to TUFX (KSPModStewards/TUFX, tested against 1.1.1). No compile-time reference.
    ///
    /// TUFX keeps the player's per-scene choice in TUFXGameSettings (a GameParameters node saved in the
    /// .sfs) and re-applies it on every GameEvents.onLevelWasLoaded and GameEvents.OnCameraChange.
    /// We only ever call the private ApplyProfile(TUFXProfile, TUFXScene), which changes what is being
    /// rendered and nothing else. ChangeProfileForScene is never used, because it writes the choice into
    /// the save game (or TUFX.cfg for the main menu).
    ///
    /// Scenes are passed around as TUFXScene enum names ("Flight", "Map", "Internal", ...) so that no
    /// TUFX type leaks into our signatures.
    /// </summary>
    internal static class TufxBridge
    {
        private const string AssemblyName = "TUFX";

        internal const string SceneFlight = "Flight";
        internal const string SceneMap = "Map";
        internal const string SceneInternal = "Internal";

        private static bool initialized;
        private static bool available;

        private static FieldInfo instanceField;          // static TexturesUnlimitedFXLoader.INSTANCE
        private static PropertyInfo currentNameProp;      // string CurrentProfileName
        private static PropertyInfo profilesProp;         // Dictionary<string, TUFXProfile> Profiles
        private static MethodInfo applyProfileMethod;     // void ApplyProfile(TUFXProfile, TUFXScene)
        private static MethodInfo profileForSceneMethod;  // string GetProfileNameForScene(TUFXScene)
        private static Type sceneEnum;

        internal static string UnavailableReason { get; private set; }

        internal static bool Available
        {
            get
            {
                EnsureInitialized();
                return available;
            }
        }

        private static void EnsureInitialized()
        {
            if (initialized)
                return;
            initialized = true;

            try
            {
                Assembly asm = ReflectionUtil.FindLoadedAssembly(AssemblyName);
                if (asm == null)
                {
                    UnavailableReason = "TUFX is not installed";
                    return;
                }

                Type loaderType = ReflectionUtil.Require(asm.GetType("TUFX.TexturesUnlimitedFXLoader"), "TUFX.TexturesUnlimitedFXLoader");
                Type profileType = ReflectionUtil.Require(asm.GetType("TUFX.TUFXProfile"), "TUFX.TUFXProfile");
                sceneEnum = ReflectionUtil.Require(asm.GetType("TUFX.TUFXScene"), "TUFX.TUFXScene");

                instanceField = ReflectionUtil.Require(
                    loaderType.GetField("INSTANCE", ReflectionUtil.AnyStatic), "TexturesUnlimitedFXLoader.INSTANCE");
                currentNameProp = ReflectionUtil.Require(
                    loaderType.GetProperty("CurrentProfileName", ReflectionUtil.AnyInstance), "CurrentProfileName");
                profilesProp = ReflectionUtil.Require(
                    loaderType.GetProperty("Profiles", ReflectionUtil.AnyInstance), "Profiles");
                applyProfileMethod = ReflectionUtil.Require(
                    loaderType.GetMethod("ApplyProfile", ReflectionUtil.AnyInstance, null, new[] { profileType, sceneEnum }, null),
                    "ApplyProfile(TUFXProfile, TUFXScene)");
                profileForSceneMethod = ReflectionUtil.Require(
                    loaderType.GetMethod("GetProfileNameForScene", ReflectionUtil.AnyInstance, null, new[] { sceneEnum }, null),
                    "GetProfileNameForScene(TUFXScene)");

                if (!typeof(IDictionary).IsAssignableFrom(profilesProp.PropertyType))
                    throw new MissingMemberException("TexturesUnlimitedFXLoader.Profiles is not a dictionary");

                available = true;
                Log.Info($"TUFX {asm.GetName().Version} detected.");
            }
            catch (Exception e)
            {
                UnavailableReason = "TUFX found but its API is not compatible (" + e.Message + ")";
            }
        }

        private static object Loader
        {
            get
            {
                object loader = Available ? instanceField.GetValue(null) : null;
                return ReflectionUtil.IsNullOrDestroyed(loader) ? null : loader;
            }
        }

        /// <summary>True once TUFX's singleton exists (it is created at game start).</summary>
        internal static bool Ready => Loader != null;

        /// <summary>Name of the profile TUFX is rendering right now, or null.</summary>
        internal static string CurrentProfileName
        {
            get
            {
                object loader = Loader;
                return loader == null ? null : currentNameProp.GetValue(loader, null) as string;
            }
        }

        internal static bool HasProfile(string profileName)
        {
            IDictionary profiles = Profiles();
            return profiles != null && !string.IsNullOrEmpty(profileName) && profiles.Contains(profileName);
        }

        /// <summary>Names of all profiles TUFX has loaded, sorted; empty if TUFX is unavailable.</summary>
        internal static List<string> ProfileNames()
        {
            var names = new List<string>();
            IDictionary profiles = Profiles();
            if (profiles != null)
            {
                foreach (object key in profiles.Keys)
                {
                    if (key is string name)
                        names.Add(name);
                }
                names.Sort(StringComparer.OrdinalIgnoreCase);
            }
            return names;
        }

        /// <summary>The profile the player configured for <paramref name="scene"/> (read-only, from TUFX settings).</summary>
        internal static string ConfiguredProfileFor(string scene)
        {
            object loader = Loader;
            object sceneValue = ParseScene(scene);
            if (loader == null || sceneValue == null)
                return null;
            return profileForSceneMethod.Invoke(loader, new[] { sceneValue }) as string;
        }

        /// <summary>
        /// Renders <paramref name="profileName"/> for <paramref name="scene"/> without touching any
        /// persisted setting. Returns false if TUFX or the profile is unavailable.
        /// </summary>
        internal static bool Apply(string profileName, string scene)
        {
            object loader = Loader;
            object sceneValue = ParseScene(scene);
            IDictionary profiles = Profiles();
            if (loader == null || sceneValue == null || profiles == null || string.IsNullOrEmpty(profileName))
                return false;

            object profile = profiles.Contains(profileName) ? profiles[profileName] : null;
            if (profile == null)
                return false;

            applyProfileMethod.Invoke(loader, new[] { profile, sceneValue });
            return true;
        }

        private static IDictionary Profiles()
        {
            object loader = Loader;
            return loader == null ? null : profilesProp.GetValue(loader, null) as IDictionary;
        }

        private static object ParseScene(string scene)
        {
            if (sceneEnum == null || string.IsNullOrEmpty(scene))
                return null;
            try
            {
                return Enum.Parse(sceneEnum, scene);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }
    }
}
