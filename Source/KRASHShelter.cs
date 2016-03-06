using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KRASH
{
	class PreSimStatus
	{
		public int	flightsGlobalIndex { get; set; }
		public bool scienceFromOrbit { get; set; }
		public bool landed { get; set; }
		public bool isReached { get; set; }
		public bool scienceFromAtmo { get; set; }
	}

    // This class is a place to stick data that will not be destroyed by KRASH.Deactivate(). 
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    class KRASHShelter : MonoBehaviour
    {
        // Editor stuff
        public static EditorFacility lastEditor = EditorFacility.None;
        public static ConfigNode lastShip = null;
		public static double simCost = 0.0F;
        public static GameScenes lastScene;
		public static List<CelestialBody> bodiesListAtSimStart;
		public static List<PreSimStatus> preSimStatus;
		public static double LimitSimCost { get; set; }


        void Awake()
        {
            DontDestroyOnLoad(this);
        }
    }
}
