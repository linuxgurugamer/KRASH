using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KRASH
{
	[KSPScenario (ScenarioCreationOptions.AddToAllGames, new GameScenes[] {
		GameScenes.SPACECENTER,
		GameScenes.EDITOR,
		GameScenes.FLIGHT,
		GameScenes.TRACKSTATION
	})]
	public class KRASHPersistent : ScenarioModule
	{
		public static bool inited = false;
		[KSPField (isPersistant = true)]
		public bool shelterSimulationActive = false;

		[KSPField (isPersistant = true)]
		public string selectedCostsCfg = "";

		override public void  OnAwake()
		{
			//Log.Info ("KRASHPersistent.Awake");
			KRASHShelter.persistent = this;
			inited = true;
		}

		public void Start()
		{
			//Log.Info ("KRASHPersistent.Start");
			//KRASHShelter.persistent = this;
		}
	}


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
		public static KRASHPersistent persistent;

        // Editor stuff
        public static EditorFacility lastEditor = EditorFacility.None;
        public static ConfigNode lastShip = null;
		public static double simcost = 0.0f;
		public static double simCost {
			get {
				return simcost;
			}
			set {
				Log.Info ("Setting simCost: " + value.ToString ());
				simcost = value;
			}
		} // = 0.0F;
		public static double startingFunds { get; set; } = 0.0f;
        public static GameScenes lastScene;
		public static List<CelestialBody> bodiesListAtSimStart;
		public static List<PreSimStatus> preSimStatus;
		public static double LimitSimCost { get; set; }
		public static KRASH instance = null;
		public static float shipCost {get; set; }
		public static float simSetupCost { get; set; } = 0;

		void Awake()
		{
			Log.Info ("KRASHShelter.Awake");
		}

		void Start()
        {
			Log.Info ("KRASHShelter.Start");
            DontDestroyOnLoad(this);
        }
    }
}
