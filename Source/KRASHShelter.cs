using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using KSP.UI.Screens;
using KSP.UI.Dialogs;

namespace KRASH
{
	[KSPScenario (ScenarioCreationOptions.AddToAllGames, new GameScenes[] {
		GameScenes.SPACECENTER,
		GameScenes.EDITOR,
		GameScenes.FLIGHT,
		GameScenes.TRACKSTATION,
		GameScenes.SPACECENTER
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
		public static UICLASS uiVisiblity;

        public static float dryMass { get; set; }
        public static float fuelMass { get; set; }

        void Awake()
		{
			Log.Info ("KRASHShelter.Awake");
			uiVisiblity = new UICLASS ();
			uiVisiblity.Awake ();
		}

		void Start()
        {
			Log.Info ("KRASHShelter.Start");
		


            DontDestroyOnLoad(this);
        }

		void OnDestroy()
		{

		}
		void RotateGizmoSpawnedSpawned(AltimeterSliderButtons asb) {
			Log.Info ("RotateGizmoSpawnedSpawned");
		}
    }

	#if false
	[KSPAddon(KSPAddon.Startup.Flight, true)]
	class GizmoEvents : MonoBehaviour
	{
		public static readonly EventData<AltimeterSliderButtons> onAltimeterSliderButtonsSpawned = new EventData<AltimeterSliderButtons>("onAltimeterSliderButtons");


		class GizmoCreationListener : MonoBehaviour
		{
			private void Start()
			// I use Start instead of Awake because whatever setup the editor does to the gizmo won't be done yet
			{
				AltimeterSliderButtons altimeterSliderButtons = null;


				if (gameObject.GetComponentCached(ref altimeterSliderButtons) != null)
				{
					onAltimeterSliderButtonsSpawned.Fire(altimeterSliderButtons);
				}
				else if (gameObject.GetComponentCached(ref altimeterSliderButtons) != null)
				{
					onAltimeterSliderButtonsSpawned.Fire(altimeterSliderButtons);
				}
				else Debug.LogError("Didn't find a gizmo on this GameObject -- something has broken");

				// could destroy this MB now, unless you wanted to use OnDestroy to sent an event
			}

			private void OnDestroy()
			{
				// could also send an event on despawn here
			}
		}


		private void Awake()
		{
			AddListenerToGizmo("AltimeterSliderButtons");

			Destroy(gameObject);
		}


		private static void AddListenerToGizmo(string prefabName)
		{
			var prefab = AssetBase.GetPrefab(prefabName);

			if (prefab == null)
			{
				Debug.LogError("Couldn't find Gizmo '" + prefabName + "'");
				return;
			}

			prefab.AddOrGetComponent<GizmoCreationListener>();

			#if DEBUG
			Debug.Log("Added listener to " + prefabName);
			#endif
		}
	}
#endif





}
