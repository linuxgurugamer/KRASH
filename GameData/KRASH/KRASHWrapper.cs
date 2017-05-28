//This KRASHWrapper.cs file is provided as-is and is not to be modified other than to update
//the namespace. Should further modification be made, no support will be provided by the author,
//linuxgurugamer.
//
// If you wish to use your own logging functions, you can replace the function Infolog with a call
// to your functions

// KRASH is defined when compiling KRASH, to avoid this module from being included.  
#if KRASH
//#if !KRASH

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;


namespace KRASH
{
	public class KRASHWrapper
	{


		private static bool? available = null;
		private static Type KRASHType = null;
		private static object apiInstance;


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

		[ConditionalAttribute("DEBUG")]
		public static void InfoLog (String msg)
		{
			UnityEngine.Debug.Log ("KRASHWrapper: " + msg);
		}
			
		static UnityEngine.Object KRASHRef;
		private static MethodInfo GetMethod(string type, string method)
		{
			if (KRASHAvailable) {
				Type calledType = Type.GetType ("KRASH.APIManager,KRASH");
				if (calledType != null) {
					KRASHRef = UnityEngine.Object.FindObjectOfType (calledType); //assumes only one instance of class KRASH exists as this command returns first instance found, also must inherit MonoBehavior for this command to work. Getting a reference to your Historian object another way would work also.
					if (KRASHRef != null) {
						MethodInfo myMethod = calledType.GetMethod (type, BindingFlags.Instance | BindingFlags.Public);
						return myMethod;
					}
				}
			}
			return null;
		}

		public static bool doTerminateSim(string msg)
		{
			if (KRASHAvailable) {
				try {
					Type calledType = Type.GetType ("KRASH.APIManager,KRASH");

					if (calledType != null) {
						MonoBehaviour KRASHRef = (MonoBehaviour)UnityEngine.Object.FindObjectOfType (calledType); //assumes only one instance of class KRASH exists as this command returns first instance found, also must inherit MonoBehavior for this command to work. Getting a reference to your Historian object another way would work also.
						if (KRASHRef != null) {
							MethodInfo myMethod = calledType.GetMethod ("TerminateSim", BindingFlags.Instance | BindingFlags.Public);

							if (myMethod != null) {
								myMethod.Invoke (KRASHRef, new object[]{msg});
								return true;
							} else { 
								InfoLog ("TerminateSim not available in KRASH");
								return false;
							}
						} else {
							InfoLog ("KRASHRef failed 1");
							return false;
						}
					}
					InfoLog ("calledtype failed");
					return false;
				} catch (Exception e) {
					InfoLog ("Error calling type: " + e);
					return false;
				}
			} else
				return false;
		}

		public static bool SetOverrideNode(string node)
		{
			InfoLog ("SetOverrideNode:  node: " + node);

			if (KRASHAvailable) {
				if (KRASHType != null) {
					object successList = GetMemberInfoValue (KRASHType.GetMember ("simAPI") [0], ApiInstance);
					if (successList != null) {
						System.Reflection.MethodInfo addMethod = successList.GetType ().GetMethod ("SetOverrideNode");
						addMethod.Invoke (successList, new object[] { node });
						return true;
					} else {
						InfoLog ("successList == null");
						return false;
					}
				} else {
					InfoLog ("KRASHType == null");
					return false;
				}
			}
			return false;
		}

		public static bool SetAllCosts(float flatSetupCost, float perPartSetupCost, float perTonSetupCost, float flatPerMinCost, float perPartPerMinCost, float perTonPerMinCost)
		{
			if (SetSetupCosts (flatSetupCost, perPartSetupCost, perTonSetupCost))
				return SetPerMinCost (flatPerMinCost, perPartPerMinCost, perTonPerMinCost);
			return false;
		}

		public static bool SetSetupCosts(float flatSetupCost, float perPartSetupCost, float perTonSetupCost)
		{
			InfoLog ("SetSetupCosts:  flatSetupCost: "+ flatSetupCost.ToString() + "   perPartSetupCost: " + perPartSetupCost.ToString() + "    perTonSetupCost: " + perTonSetupCost.ToString());

			if (KRASHAvailable) {
				if (KRASHType != null) {
					object successList = GetMemberInfoValue (KRASHType.GetMember ("simAPI") [0], ApiInstance);
					if (successList != null) {
						System.Reflection.MethodInfo addMethod = successList.GetType ().GetMethod ("SetSetupCosts");
						addMethod.Invoke (successList, new object[] { flatSetupCost, perPartSetupCost, perTonSetupCost });
						return true;
					} else {
						InfoLog ("successList == null");
						return false;
					}
				} else {
					InfoLog ("KRASHType == null");
					return false;
				}
			}
			return false;
		}

		public static bool SetPerMinCost(float flatPerMinCost, float perPartPerMinCost, float perTonPerMinCost)
		{
			if (KRASHAvailable) {
				if (KRASHType != null) {
					object successList = GetMemberInfoValue (KRASHType.GetMember ("simAPI") [0], ApiInstance);
					if (successList != null) {
						System.Reflection.MethodInfo addMethod = successList.GetType ().GetMethod ("SetPerMinCost");
						addMethod.Invoke (successList, new object[] { flatPerMinCost, perPartPerMinCost, perTonPerMinCost });
						return true;
					} else {
						InfoLog ("successList == null");
						return false;
					}
				} else {
					InfoLog ("KRASHType == null");
					return false;
				}
			}
			return false;
		}
			

		public static bool SetFlatCosts(float flatSetupCost, float flatPerMinCost)
		{
			if (KRASHAvailable) {
				if (KRASHType != null) {
					object successList = GetMemberInfoValue (KRASHType.GetMember ("simAPI") [0], ApiInstance);
					if (successList != null) {
						System.Reflection.MethodInfo addMethod = successList.GetType ().GetMethod ("SetFlatCosts");
						addMethod.Invoke (successList, new object[] { flatSetupCost, flatPerMinCost });
						return true;
					} else {
						InfoLog ("successList == null");
						return false;
					}
				} else {
					InfoLog ("KRASHType == null");
					return false;
				}
			}
			return false;
		}

		public static bool SetPerPartCosts(float perPartSetupCost, float perPartPerMinCost)
		{
			if (KRASHAvailable) {
				if (KRASHType != null) {
					object successList = GetMemberInfoValue (KRASHType.GetMember ("simAPI") [0], ApiInstance);
					if (successList != null) {
						System.Reflection.MethodInfo addMethod = successList.GetType ().GetMethod ("SetPerPartCosts");
						addMethod.Invoke (successList, new object[] { perPartSetupCost, perPartPerMinCost });
						return true;
					} else {
						InfoLog ("successList == null");
						return false;
					}
				} else {
					InfoLog ("KRASHType == null");
					return false;
				}
			}
			return false;
		}

		public static bool SetPerTonCosts(float perTonSetupCost, float perTonPerMinCost)
		{
			if (KRASHAvailable) {
				if (KRASHType != null) {
					object successList = GetMemberInfoValue (KRASHType.GetMember ("simAPI") [0], ApiInstance);
					if (successList != null) {
						System.Reflection.MethodInfo addMethod = successList.GetType ().GetMethod ("SetPerTonCosts");
						addMethod.Invoke (successList, new object[] { perTonSetupCost, perTonPerMinCost });
						return true;
					} else {
						InfoLog ("successList == null");
						return false;
					}
				} else {
					InfoLog ("KRASHType == null");
					return false;
				}
			}
			return false;
		}


		public static bool SetPercentCosts(float percentSetupCost, float percentPerMinCost)
		{
			if (KRASHAvailable) {
				if (KRASHType != null) {
					object successList = GetMemberInfoValue (KRASHType.GetMember ("simAPI") [0], ApiInstance);
					if (successList != null) {
						System.Reflection.MethodInfo addMethod = successList.GetType ().GetMethod ("SetPercentCosts");
						addMethod.Invoke (successList, new object[] { percentSetupCost, percentPerMinCost });
						return true;
					} else {
						InfoLog ("successList == null");
						return false;
					}
				} else {
					InfoLog ("KRASHType == null");
					return false;
				}
			}
			return false;
		}

		public static bool AddToCosts(float cost)
		{
			if (KRASHAvailable) {
				if (KRASHType != null) {
					object successList = GetMemberInfoValue (KRASHType.GetMember ("simAPI") [0], ApiInstance);
					if (successList != null) {
						System.Reflection.MethodInfo addMethod = successList.GetType ().GetMethod ("addToCosts");
						addMethod.Invoke (successList, new object[] { cost });
						return true;
					} else {
						InfoLog ("successList == null");
						return false;
					}
				} else {
					InfoLog ("KRASHType == null");
					return false;
				}
			}
			return false;
		}


		public static double GetCurrentSimCosts()
		{
			double currentSimCosts = 0.0f;
			if (KRASHAvailable) {
				if (KRASHType != null) {
					object successList = GetMemberInfoValue (KRASHType.GetMember ("simAPI") [0], ApiInstance);
					if (successList != null) {
						System.Reflection.MethodInfo addMethod = successList.GetType ().GetMethod ("getCurrentSimCosts");
						currentSimCosts = (double)addMethod.Invoke (successList, new object[] { });
						InfoLog("KRASHWrapper.getCurrentSimCosts  currentSimCosts: " + currentSimCosts.ToString());
						return currentSimCosts;
					} else {
						InfoLog ("successList == null");
						return currentSimCosts;
					}
				} else {
					InfoLog ("KRASHType == null");
					return currentSimCosts;
				}
			}
			return currentSimCosts;
		}
			
		public static bool simulationActive()
		{
			bool b = false;
			if (KRASHAvailable) {
				if (KRASHType != null) {
					object successList = GetMemberInfoValue (KRASHType.GetMember ("simAPI") [0], ApiInstance);
					if (successList != null) {
						System.Reflection.MethodInfo addMethod = successList.GetType ().GetMethod ("simulationActive");
						b = (bool)addMethod.Invoke (successList, new object[] { });
						InfoLog("KRASHWrapper.simulationActive  b: " + b.ToString());
						return b;
					} else {
						InfoLog ("successList == null");
						return b;
					}
				} else {
					InfoLog ("KRASHType == null");
					return b;
				}
			}
			return b;
		}

		#region APIMethods

		/***************/
		/* API methods */
		/***************/
		/* Adds a listener to the Sim Menu Event. When the initial KRASH menu is entered the method will 
		* be invoked with the Vessel; a double with 0 */
		public static void AddSimMenuEvent (Action<Vessel, double> method)
		{
			object successList = GetMemberInfoValue (KRASHType.GetMember ("SimMenuEvent") [0], ApiInstance);
			System.Reflection.MethodInfo addMethod = successList.GetType ().GetMethod ("Add");
			addMethod.Invoke (successList, new object[] { method });
		}

		/* Removes a listener from the Sim Menu Event */
		public static void RemoveSimMenuEvent (Action<Vessel, double> method)
		{
			object successList = GetMemberInfoValue (KRASHType.GetMember ("SimMenuEvent") [0], ApiInstance);
			System.Reflection.MethodInfo removeMethod = successList.GetType ().GetMethod ("Remove");
			removeMethod.Invoke (successList, new object[] { method });
		}

		/**********************************************************************/

		/* Adds a listener to the Sim Start Event. When the simulation is started the method will 
		* be invoked with the Vessel; a double with 0 */
		public static void AddSimStartEvent (Action<Vessel, double> method)
		{
			object successList = GetMemberInfoValue (KRASHType.GetMember ("SimStartEvent") [0], ApiInstance);
			System.Reflection.MethodInfo addMethod = successList.GetType ().GetMethod ("Add");
			addMethod.Invoke (successList, new object[] { method });
		}

		/* Removes a listener from the Sim Start Event */
		public static void RemoveSimStartEvent (Action<Vessel, double> method)
		{
			object successList = GetMemberInfoValue (KRASHType.GetMember ("SimStartEvent") [0], ApiInstance);
			System.Reflection.MethodInfo removeMethod = successList.GetType ().GetMethod ("Remove");
			removeMethod.Invoke (successList, new object[] { method });
		}

		/**********************************************************************/

		/* Adds a listener to the Sim Restart Event. When the simulation is restarted the method will 
		* be invoked with the Vessel; a double with the cost of the simulation to this point in time */
		public static void AddSimRestartEvent (Action<Vessel, double> method)
		{
			object successList = GetMemberInfoValue (KRASHType.GetMember ("SimRestartEvent") [0], ApiInstance);
			System.Reflection.MethodInfo addMethod = successList.GetType ().GetMethod ("Add");
			addMethod.Invoke (successList, new object[] { method });
		}

		/* Removes a listener from the Sim Restart Event */
		public static void RemoveSimRestartEvent (Action<Vessel, double> method)
		{
			object successList = GetMemberInfoValue (KRASHType.GetMember ("SimRestartEvent") [0], ApiInstance);
			System.Reflection.MethodInfo removeMethod = successList.GetType ().GetMethod ("Remove");
			removeMethod.Invoke (successList, new object[] { method });
		}

		/**********************************************************************/

		/* Adds a listener to the Sim Termination Event. When the simulation is terminated the method will 
		* be invoked with the Vessel; a double with the total cost of the simulation */
		public static void AddSimTerminationEvent (Action<Vessel, double> method)
		{
			object successList = GetMemberInfoValue (KRASHType.GetMember ("SimTerminationEvent") [0], ApiInstance);
			System.Reflection.MethodInfo addMethod = successList.GetType ().GetMethod ("Add");
			addMethod.Invoke (successList, new object[] { method });
		}

		/* Removes a listener from the Sim Termination Event */
		public static void RemoveSimTerminationEvent (Action<Vessel, double> method)
		{
			object successList = GetMemberInfoValue (KRASHType.GetMember ("SimTerminationEvent") [0], ApiInstance);
			System.Reflection.MethodInfo removeMethod = successList.GetType ().GetMethod ("Remove");
			removeMethod.Invoke (successList, new object[] { method });
		}

		/**********************************************************************/

		/* Adds a listener to the Sim Termination Event. When the simulation is terminated the method will 
		* be invoked with the Vessel; a double with the total cost of the simulation */
		public static void AddSimTimedEvent (Action<Vessel, double> method)
		{
			object successList = GetMemberInfoValue (KRASHType.GetMember ("SimTimedEvent") [0], ApiInstance);
			System.Reflection.MethodInfo addMethod = successList.GetType ().GetMethod ("Add");
			addMethod.Invoke (successList, new object[] { method });
		}

		/* Removes a listener from the Sim Termination Event */
		public static void RemoveSimTimedEvent (Action<Vessel, double> method)
		{
			object successList = GetMemberInfoValue (KRASHType.GetMember ("SimTimedEvent") [0], ApiInstance);
			System.Reflection.MethodInfo removeMethod = successList.GetType ().GetMethod ("Remove");
			removeMethod.Invoke (successList, new object[] { method });
		}



		#endregion

		#region InternalFunctions

		/******************************************/
		/* Internal functions. Just ignore these. */
		/******************************************/

		/* The APIManager instance */
		private static object ApiInstance {
			get {
				if (KRASHAvailable && apiInstance == null) {
					apiInstance = KRASHType.GetProperty ("ApiInstance").GetValue (null, null);
				}

				return apiInstance;
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

		static double lastUpdate = 0;
		static double firstUpdate = 0;


		public void testWrapper()
		{
			if (firstUpdate == 0) {
				firstUpdate = Planetarium.GetUniversalTime ();
				//AddSimStartEvent (simStart);
			}
			if (Planetarium.GetUniversalTime () - lastUpdate >= 1) {
				lastUpdate = Planetarium.GetUniversalTime ();
				InfoLog ("KRASHAvailable: " + KRASHWrapper.KRASHAvailable.ToString ());

			}
			//if (Planetarium.GetUniversalTime () - firstUpdate > 5)
			//{
			//	doTerminateSim ("test termination");
			//}
		}
	}
}

#endif