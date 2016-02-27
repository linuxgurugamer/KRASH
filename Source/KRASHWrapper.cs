//This KRASHWrapper.cs file is provided as-is and is not to be modified other than to update
//the namespace. Should further modification be made, no support will be provided by the author,
//linuxgurugamer.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;

#if false
namespace KRASH
{
	public class KRASHWrapper
	{


		private static bool? available = null;
		private static Type KRASHType = null;
		private static object instance_;


		/* Call this to see if the addon is available. If this returns false, no additional API calls should be made! */
		public static bool KRASHAvailable {
			get {
				if (available == null) {
					KRASHType = AssemblyLoader.loadedAssemblies
						.Select (a => a.assembly.GetExportedTypes ())
						.SelectMany (t => t)
						.FirstOrDefault (t => t.FullName == "KRASH.APIManager");
					available = KRASHType != null;
				}
				return (bool)available;
			}
		}

		MonoBehaviour KRASHRef;
		private MethodInfo GetMethod(string type, string method)
		{
			if (KRASHAvailable) {
				Type calledType = Type.GetType ("KRASH.APIManager,KRASH");
				if (calledType != null) {
					KRASHRef = (MonoBehaviour)UnityEngine.Object.FindObjectOfType (calledType); //assumes only one instance of class KRASH exists as this command returns first instance found, also must inherit MonoBehavior for this command to work. Getting a reference to your Historian object another way would work also.
					if (KRASHRef != null) {
						MethodInfo myMethod = calledType.GetMethod ("TerminateSim", BindingFlags.Instance | BindingFlags.Public);
						return myMethod;
					}
				}
			}
			return null;
		}

		public static bool TerminateSim()
		{
			if (KRASHAvailable) {
				try {
					Type calledType = Type.GetType ("KRASH.APIManager,KRASH");
					if (calledType != null) {
						MonoBehaviour KRASHRef = (MonoBehaviour)UnityEngine.Object.FindObjectOfType (calledType); //assumes only one instance of class KRASH exists as this command returns first instance found, also must inherit MonoBehavior for this command to work. Getting a reference to your Historian object another way would work also.
						if (KRASHRef != null) {
							MethodInfo myMethod = calledType.GetMethod ("TerminateSim", BindingFlags.Instance | BindingFlags.Public);

							if (myMethod != null) {
								myMethod.Invoke (KRASHRef, null);
								return true;
							} else { 
								Log.Info ("TerminateSim not available in KRASH");
								return false;
							}
						} else {
							Log.Info ("KRASHRef  failed");
							return false;
						}
					}
					Log.Info ("calledtype failed");
					return false;
				} catch (Exception e) {
					Log.Info ("Error calling type: " + e);
					return false;
				}
			} else
				return false;
		}

		public bool SetAllCosts(float flatSetupCost, float perPartSetupCost, float perTonSetupCost, float flatPerMinCost, float perPartPerMinCost, float perTonPerMinCost)
		{
			if (SetSetupCosts (flatSetupCost, perPartSetupCost, perTonSetupCost))
				return SetPerMinCost (flatPerMinCost, perPartPerMinCost, perTonPerMinCost);
//			MethodInfo myMethod = GetMethod ("KRASH.APIManager,KRASH", "SetAllCosts");
//			if (myMethod != null) {
//				myMethod.Invoke (KRASHRef, new object[]{flatSetupCost, perPartSetupCost, perTonSetupCost, flatPerMinCost, perPartPerMinCost, perTonPerMinCost});
//				return true;
//			}
			return false;
		}

		public bool SetSetupCosts(float flatSetupCost, float perPartSetupCost, float perTonSetupCost)
		{
			MethodInfo myMethod = GetMethod ("KRASH.APIManager,KRASH", "SetSetupCosts");
			if (myMethod != null) {
				myMethod.Invoke (KRASHRef,  new object[]{flatSetupCost, perPartSetupCost, perTonSetupCost});
				return true;
			}
			return false;

		}
		public bool SetPerMinCost(float flatPerMinCost, float perPartPerMinCost, float perTonPerMinCost)
		{
			MethodInfo myMethod = GetMethod ("KRASH.APIManager,KRASH", "SetPerMinCost");
			if (myMethod != null) {
							myMethod.Invoke (KRASHRef,  new object[]{flatPerMinCost, perPartPerMinCost, perTonPerMinCost});
				return true;
			}
			return false;

		}
			

		public bool SetFlatCosts(float flatSetupCost, float flatPerMinCost)
		{
			MethodInfo myMethod = GetMethod ("KRASH.APIManager,KRASH", "SetFlatCosts");
			if (myMethod != null) {
				myMethod.Invoke (KRASHRef,  new object[]{flatPerMinCost, flatPerMinCost});
				return true;
			}
			return false;
		}

		public bool SetPerPartCosts(float perPartSetupCost, float perPartPerMinCost)
		{
			MethodInfo myMethod = GetMethod ("KRASH.APIManager,KRASH", "SetPerPartCosts");
			if (myMethod != null) {
				myMethod.Invoke (KRASHRef,  new object[]{ perPartSetupCost, perPartPerMinCost});
				return true;
			}
			return false;
		}

		public bool SetPerTonCosts(float perTonSetupCost, float perTonPerMinCost)
		{
			MethodInfo myMethod = GetMethod ("KRASH.APIManager,KRASH", "SetPerTonCosts");
			if (myMethod != null) {
				myMethod.Invoke (KRASHRef,  new object[]{perTonSetupCost, perTonPerMinCost});
				return true;
			}
			return false;
		}

		public float getCurrentSimCosts()
		{
			MethodInfo myMethod = GetMethod ("KRASH.APIManager,KRASH", "getCurrentSimCosts");
			if (myMethod != null) {
				object costs = myMethod.Invoke (KRASHRef,  null);
				return (float)costs;
			}
			return 0.0F;
		}

		public bool addToCosts(float cost)
		{
			MethodInfo myMethod = GetMethod ("KRASH.APIManager,KRASH", "addToCosts");
			if (myMethod != null) {
				myMethod.Invoke (KRASHRef,  new object[]{cost});
				return true;
			}
			return false;
		}

		#region APIMethods

		/***************/
		/* API methods */
		/***************/
		/* Adds a listener to the Sim Menu Event. When the initial KRASH menu is entered the method will 
		* be invoked with the Vessel; a float with 0 */
		public static void AddSimMenuEvent (Action<Vessel, float> method)
		{
			object successList = GetMemberInfoValue (KRASHType.GetMember ("SimMenuEvent") [0], Instance);
			System.Reflection.MethodInfo addMethod = successList.GetType ().GetMethod ("Add");
			addMethod.Invoke (successList, new object[] { method });
		}

		/* Removes a listener from the Sim Menu Event */
		public static void RemoveSimMenuEvent (Action<Vessel, float[], string> method)
		{
			object successList = GetMemberInfoValue (KRASHType.GetMember ("SimMenuEvent") [0], Instance);
			System.Reflection.MethodInfo removeMethod = successList.GetType ().GetMethod ("Remove");
			removeMethod.Invoke (successList, new object[] { method });
		}

		/**********************************************************************/

		/* Adds a listener to the Sim Start Event. When the simulation is started the method will 
		* be invoked with the Vessel; a float with 0 */
		public static void AddSimStartEvent (Action<Vessel, float> method)
		{
			object successList = GetMemberInfoValue (KRASHType.GetMember ("SimStartEvent") [0], Instance);
			System.Reflection.MethodInfo addMethod = successList.GetType ().GetMethod ("Add");
			addMethod.Invoke (successList, new object[] { method });
		}

		/* Removes a listener from the Sim Start Event */
		public static void RemoveSimStartEvent (Action<Vessel, float[], string> method)
		{
			object successList = GetMemberInfoValue (KRASHType.GetMember ("SimStartEvent") [0], Instance);
			System.Reflection.MethodInfo removeMethod = successList.GetType ().GetMethod ("Remove");
			removeMethod.Invoke (successList, new object[] { method });
		}

		/**********************************************************************/

		/* Adds a listener to the Sim Restart Event. When the simulation is restarted the method will 
		* be invoked with the Vessel; a float with the cost of the simulation to this point in time */
		public static void AddSimRestartEvent (Action<Vessel, float> method)
		{
			object successList = GetMemberInfoValue (KRASHType.GetMember ("SimRestartEvent") [0], Instance);
			System.Reflection.MethodInfo addMethod = successList.GetType ().GetMethod ("Add");
			addMethod.Invoke (successList, new object[] { method });
		}

		/* Removes a listener from the Sim Restart Event */
		public static void RemoveSimRestartEvent (Action<Vessel, float[], string> method)
		{
			object successList = GetMemberInfoValue (KRASHType.GetMember ("SimRestartEvent") [0], Instance);
			System.Reflection.MethodInfo removeMethod = successList.GetType ().GetMethod ("Remove");
			removeMethod.Invoke (successList, new object[] { method });
		}

		/**********************************************************************/

		/* Adds a listener to the Sim Termination Event. When the simulation is terminated the method will 
		* be invoked with the Vessel; a float with the total cost of the simulation */
		public static void AddSimTerminationEvent (Action<Vessel, float> method)
		{
			object successList = GetMemberInfoValue (KRASHType.GetMember ("SimTerminationEvent") [0], Instance);
			System.Reflection.MethodInfo addMethod = successList.GetType ().GetMethod ("Add");
			addMethod.Invoke (successList, new object[] { method });
		}

		/* Removes a listener from the Sim Termination Event */
		public static void RemoveSimTerminationEvent (Action<Vessel, float[], string> method)
		{
			object successList = GetMemberInfoValue (KRASHType.GetMember ("SimTerminationEvent") [0], Instance);
			System.Reflection.MethodInfo removeMethod = successList.GetType ().GetMethod ("Remove");
			removeMethod.Invoke (successList, new object[] { method });
		}

		/**********************************************************************/

		/* Adds a listener to the Sim Termination Event. When the simulation is terminated the method will 
		* be invoked with the Vessel; a float with the total cost of the simulation */
		public static void AddSimTimedEvent (Action<Vessel, float> method)
		{
			object successList = GetMemberInfoValue (KRASHType.GetMember ("SimTimedEvent") [0], Instance);
			System.Reflection.MethodInfo addMethod = successList.GetType ().GetMethod ("Add");
			addMethod.Invoke (successList, new object[] { method });
		}

		/* Removes a listener from the Sim Termination Event */
		public static void RemoveSimTimedEvent (Action<Vessel, float[], string> method)
		{
			object successList = GetMemberInfoValue (KRASHType.GetMember ("SimTimedEvent") [0], Instance);
			System.Reflection.MethodInfo removeMethod = successList.GetType ().GetMethod ("Remove");
			removeMethod.Invoke (successList, new object[] { method });
		}



		#endregion

		#region InternalFunctions

		/******************************************/
		/* Internal functions. Just ignore these. */
		/******************************************/

		/* The APIManager instance */
		private static object Instance {
			get {
				if (KRASHAvailable && instance_ == null) {
					instance_ = KRASHType.GetProperty ("instance").GetValue (null, null);
				}

				return instance_;
			}
		}

		/* A helper function I use since I'm bad at reflection. It's for getting the value of a MemberInfo */
		private static object GetMemberInfoValue (System.Reflection.MemberInfo member, object sourceObject)
		{
			object newVal;
			if (member is System.Reflection.FieldInfo)
				newVal = ((System.Reflection.FieldInfo)member).GetValue (sourceObject);
			else
				newVal = ((System.Reflection.PropertyInfo)member).GetValue (sourceObject, null);
			return newVal;
		}

		#endregion
	}
}

#endif