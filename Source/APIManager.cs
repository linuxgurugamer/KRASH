using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP;

#if false
namespace KRASH
{
	public class APIManager
	{

		//This is the actual instance. It gets instantiated when someone calls for it, below.
		private static APIManager instance_ = null;
		//This is the public reference to the instance. Nobody else can change the instance, it's read only.
		public static APIManager instance {
			//get and set let you get the value or set the value. Providing only one (here: get) makes it read only or write only.
			get {
				//If the instance is null we make a new one
				if (instance_ == null)
					instance_ = new APIManager ();
				//Then we return the instance
				return instance_;
			}
		}

		public void TerminateSim()
		{
			Log.Info("TerminateSim called");
		}

		public void SetSetupCosts(float flatSetupCost, float perPartSetupCost, float perTonSetupCost)
		{
			Log.Info ("SetSetupCosts");
		}
		public void SetPerMinCost(float flatPerMinCost, float perPartPerMinCost, float perTonPerMinCost)
		{
			Log.Info ("SetPerMinCost");
		}
		public void SetFlatCosts(float flatSetupCost, float flatPerMinCost)
		{
			Log.Info ("SetFlatCosts");
		}
		public void SetPerPartCosts(float perPartSetupCost, float perPartPerMinCost)
		{
			Log.Info ("SetPerPartCosts");
		}
		public void SetPerTonCosts(float perTonSetupCost, float perTonPerMinCost)
		{
			Log.Info ("SetPerTonCosts");
		}
		public float getCurrentSimCosts()
		{
			Log.Info ("getCurrentSimCosts");
			return 0.0f;
		}
		public void addToCosts(float cost)
		{
			Log.Info ("addToCosts");
		}

		public SimEvent SimMenuEvent = new SimEvent ();
		public SimEvent SimStartEvent = new SimEvent ();
		public SimEvent SimRestartEvent = new SimEvent ();
		public SimEvent SimTerminationEvent = new SimEvent ();
		public SimEvent SimTimedEvent = new SimEvent();

		//The SimEvent class is used by all events. It basically just lets you add a listening method to the event, remove one, or fire all the events.
		public class SimEvent
		{
			//This is the list of methods that should be activated when the event fires
			private List<Action<Vessel, float>> listeningMethods = new List<Action<Vessel, float>> ();

			//This adds an event to the List of listening methods
			public void Add (Action<Vessel, float> method)
			{
				//We only add it if it isn't already added. Just in case.
				if (!listeningMethods.Contains (method))
					listeningMethods.Add (method);
			}

			//This removes and event from the List
			public void Remove (Action<Vessel, float> method)
			{
				//We also only remove it if it's actually in the list.
				if (listeningMethods.Contains (method))
					listeningMethods.Remove (method);
			}

			//This fires the event off, activating all the listening methods.
			public void Fire (Vessel vessel, float cost)
			{
				//Loop through the list of listening methods and Invoke them.
				foreach (Action<Vessel, float> method in listeningMethods)
					method.Invoke (vessel, cost);
			}
		}
	}


}

#endif