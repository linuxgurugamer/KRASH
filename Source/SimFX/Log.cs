using UnityEngine;

namespace KRASH.SimFX
{
    /// <summary>
    /// Logging for the simulation visuals, with their own prefix so their lines are easy to find in KSP.log.
    /// Inside KRASH.SimFX this class takes precedence over KRASH.Log.
    /// </summary>
    internal static class Log
    {
        private const string Prefix = "[KRASH SimFX] ";

        /// <summary>Enables <see cref="Detail"/> output; driven by debugLogging in SimFX/Settings.cfg.</summary>
        internal static bool DebugEnabled { get; set; }

        internal static void Info(string message) => Debug.Log(Prefix + message);

        internal static void Warn(string message) => Debug.LogWarning(Prefix + message);

        internal static void Error(string message) => Debug.LogError(Prefix + message);

        internal static void Detail(string message)
        {
            if (DebugEnabled)
                Debug.Log(Prefix + "[debug] " + message);
        }
    }
}
