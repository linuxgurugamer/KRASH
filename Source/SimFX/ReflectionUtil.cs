using System;
using System.Reflection;

namespace KRASH.SimFX
{
    internal static class ReflectionUtil
    {
        internal const BindingFlags AnyStatic = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
        internal const BindingFlags AnyInstance = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        /// <summary>Finds a mod assembly among those KSP loaded from GameData, by assembly name.</summary>
        internal static Assembly FindLoadedAssembly(string assemblyName)
        {
            foreach (AssemblyLoader.LoadedAssembly loaded in AssemblyLoader.loadedAssemblies)
            {
                Assembly asm = loaded.assembly;
                if (asm == null)
                    continue;
                if (string.Equals(asm.GetName().Name, assemblyName, StringComparison.OrdinalIgnoreCase))
                    return asm;
            }

            return null;
        }

        /// <summary>Throws a descriptive exception when a required member is missing (caught by the bridges).</summary>
        internal static T Require<T>(T member, string description) where T : class
        {
            if (member == null)
                throw new MissingMemberException(description);
            return member;
        }

        /// <summary>True when <paramref name="obj"/> is null or a destroyed Unity object.</summary>
        internal static bool IsNullOrDestroyed(object obj)
        {
            if (obj == null)
                return true;
            if (obj is UnityEngine.Object unityObject)
                return unityObject == null; // Unity's overloaded == detects destroyed objects
            return false;
        }

        /// <summary>
        /// True when the old standalone KRASH-SimFX mod is installed as well. It would apply the same effects
        /// a second time, so the built-in version stays inactive until it is removed.
        /// </summary>
        internal static bool StandaloneSimFXInstalled => FindLoadedAssembly("KRASH-SimFX") != null;
    }
}
