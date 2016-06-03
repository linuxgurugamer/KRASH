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

		void Start() {
			Log.Info ("EditorModule.Start");
		}
		void Awake() {
			Log.Info ("EditorModule.Awake");
		}
		void LateUpdate() {
			Log.Info ("EditorModule.LateUpdate, shelterSimulationActive: " + KRASHShelter.persistent.shelterSimulationActive.ToString());

			if (KRASHShelter.persistent.shelterSimulationActive) {
				string s = InputLockManager.PrintLockStack();

				if (InputLockManager.LockMask == 0) {
					Log.Info ("Disabling simulation");
					KRASHShelter.persistent.shelterSimulationActive = false;
					KRASHShelter.instance.SimulationNotification (false);

				}
			}
		}

	}
}
#endif
