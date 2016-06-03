#if true
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace KRASH.SceneModules
{
[KSPAddon(KSPAddon.Startup.EditorAny, true)]
    class EditorModule : MonoBehaviour
    {
        private const string TAG = "KRASH.EditorModule";
		DateTime lastUpdate;

		void Start() {
			Log.Info ("EditorModule.Start");
		}
		void Awake() {
			Log.Info ("EditorModule.Awake");
		}
		void LateUpdate() {
			Log.Info ("EditorModule.LateUpdate, shelterSimulationActive: " + KRASHShelter.persistent.shelterSimulationActive.ToString());

			// This is a hack until I can figure out a better way to determine where we are still in the editor
			// The problem comes when you try to start a simulation, and it is stopped by KSP because of
			// either locked parts or no control units available, and you then cancel.
			// First, this is only active in the Editor
			// Second, if a sim is active, then check to see if the LockMask is 0, if it is, and if
			// more than a second of real time has happened since the last time the lockmast wasn't 0
			// then disable the sim
			//
			// This is dependent on the LockMask being set to 0 when exiting a menu (this is the hack)

			if (KRASHShelter.persistent.shelterSimulationActive) {

				if (InputLockManager.LockMask == 0) {
					
					if (System.DateTime.Now.CompareTo(lastUpdate) < 0) {

						Log.Info ("Disabling simulation");
						KRASHShelter.persistent.shelterSimulationActive = false;
						KRASHShelter.instance.SimulationNotification (false);
						HighLogic.CurrentGame.Parameters.Flight.CanQuickSave = true;
						HighLogic.CurrentGame.Parameters.Flight.CanQuickLoad = true;
					}

				} else {
					
					lastUpdate = System.DateTime.Now.AddSeconds(1);
					Log.Info ("LastUpdate: " + lastUpdate.ToString ());
				}
			}
		}

	}
}
#endif
