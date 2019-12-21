using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP;

#if true
namespace KRASH
{

	public class APIManager:MonoBehaviour
	{

		//This is the actual instance. It gets instantiated when someone calls for it, below.
		private static APIManager apiInstance = null;
		//This is the public reference to the instance. Nobody else can change the instance, it's read only.
		public static APIManager ApiInstance {
			//get and set let you get the value or set the value. Providing only one (here: get) makes it read only or write only.
			get {
				Log.Info ("ApiInstance called");
				//If the instance is null we make a new one
				if (apiInstance == null)
					apiInstance = new APIManager ();
				//Then we return the instance
				return apiInstance;
			}
		}


#if true
		public SimAPI simAPI = new SimAPI();

		public class SimAPI
		{
			public void SetOverrideNode(string node)
			{
				Log.Info ("SetoverrideNode: " + node);
				KRASHShelter.instance.cfg.setOverrideNode (node);
			}

			public void TerminateSim(string msg)
			{
				Log.Info ("TerminateSim called");
				KRASHShelter.instance.simPauseMenuInstance.DisplayTerminationMessage (msg);
			}
            public void TerminateSimNoDialog()
            {
                SimulationPauseMenu.instance.TerminateSimNoDialog();

            }

			public void SetSetupCosts(float flatSetupCost, float perPartSetupCost, float perTonSetupCost)
			{
				Log.Info ("APIManager.SetSetupCosts:  flatSetupCost: "+ flatSetupCost.ToString() + "   perPartSetupCost: " + perPartSetupCost.ToString() + "    perTonSetupCost: " + perTonSetupCost.ToString());
				KRASHShelter.instance.cfg.flatSetupCost = flatSetupCost;
				KRASHShelter.instance.cfg.perPartSetupCost = perPartSetupCost;
				KRASHShelter.instance.cfg.perTonSetupCost = perTonSetupCost;
			}

			public void SetPerMinCost(float flatPerMinCost, float perPartPerMinCost, float perTonPerMinCost)
			{
				Log.Info ("APIManager.SetPerMinCost:  flatPerMinCost: " + flatPerMinCost.ToString () + "   perPartPerMinCost: " + perPartPerMinCost.ToString () + "    perTonPerMinCost: " + perTonPerMinCost.ToString ());
				KRASHShelter.instance.cfg.flatPerMinCost = flatPerMinCost;
				KRASHShelter.instance.cfg.perPartPerMinCost = perPartPerMinCost;
				KRASHShelter.instance.cfg.perTonPerMinCost = perTonPerMinCost;
			}

			public void SetFlatCosts(float flatSetupCost, float flatPerMinCost)
			{
				Log.Info ("APIManager.SetFlatCosts:  flatSetupCost: " + flatSetupCost.ToString() + "    flatPerMinCost: " + flatPerMinCost.ToString());
				KRASHShelter.instance.cfg.flatSetupCost = flatSetupCost;
				KRASHShelter.instance.cfg.flatPerMinCost = flatPerMinCost;
			}

			public void SetPerPartCosts(float perPartSetupCost, float perPartPerMinCost)
			{
				Log.Info ("APIManager.SetPerPartCosts:  perPartSetupCost: " + perPartSetupCost.ToString() + "    perPartPerMinCost: " + perPartPerMinCost.ToString());
				KRASHShelter.instance.cfg.perPartSetupCost = perPartSetupCost;
				KRASHShelter.instance.cfg.perPartPerMinCost = perPartPerMinCost;
			}

			public void SetPercentCosts(float percentSetupCost, float percentPerMinCost)
			{
				Log.Info ("APIManager.percentSetupCost:  percentSetupCost: " + percentSetupCost.ToString() + "    percentPerMinCost: " + percentPerMinCost.ToString());
				KRASHShelter.instance.cfg.percentSetupCost = percentSetupCost;
				KRASHShelter.instance.cfg.percentPerMinCost = percentPerMinCost;
			}

			public void SetPerTonCosts(float perTonSetupCost, float perTonPerMinCost)
			{
				Log.Info ("APIManager.SetPerTonCosts:  perTonSetupCost: " + perTonSetupCost.ToString() + "    perTonPerMinCost: " + perTonPerMinCost.ToString());
				KRASHShelter.instance.cfg.perTonSetupCost = perTonSetupCost;
				KRASHShelter.instance.cfg.perTonPerMinCost = perTonPerMinCost;
			}

			public double getCurrentSimCosts()
			{
				Log.Info ("APIManager.getCurrentSimCosts");
				return KRASHShelter.simCost + KRASHShelter.simSetupCost;
			}
			public void addToCosts(float cost)
			{
				Log.Info ("APIManager.addToCosts");
				KRASHShelter.simCost += cost;
			}

			public bool simulationActive()
			{
				return KRASHShelter.instance.simPauseMenuInstance.SimStarted();
			}
		}
#endif
		
		public SimEvent SimMenuEvent = new SimEvent ();
		public SimEvent SimStartEvent = new SimEvent ();
		public SimEvent SimRestartEvent = new SimEvent ();
		public SimEvent SimTerminationEvent = new SimEvent ();

		//The SimEvent class is used by all events. It basically just lets you add a listening method to the event, remove one, or fire all the events.
		public class SimEvent
		{
			//This is the list of methods that should be activated when the event fires
			private List<Action<Vessel, double>> listeningMethods = new List<Action<Vessel, double>> ();

			//This adds an event to the List of listening methods
			public void Add (Action<Vessel, double> method)
			{
				//We only add it if it isn't already added. Just in case.
				Log.Info("SimEvent.Add");
				if (!listeningMethods.Contains (method))
					listeningMethods.Add (method);
			}

			//This removes and event from the List
			public void Remove (Action<Vessel, double> method)
			{
				//We also only remove it if it's actually in the list.
				if (listeningMethods.Contains (method))
					listeningMethods.Remove (method);
			}

			//This fires the event off, activating all the listening methods.
			public void Fire (Vessel vessel, double cost)
			{
				//Loop through the list of listening methods and Invoke them.
				Log.Info("listeningMethods.count: " + listeningMethods.Count());
				foreach (Action<Vessel, double> method in listeningMethods) {
					Log.Info ("method.Invoke");
					method.Invoke (vessel, cost);
				}
			}
		}
	}


}
#endif