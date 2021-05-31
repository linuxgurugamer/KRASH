// Contents of this file are under the license of the GPL V3, a copy of which is
// included in this directory
// This file included with permission from the Hyperedit project
//
// HyperEdit Created by:
// khyperia (original creator, code)
// Ezriilc (web)
// sirkut (code)
// payo (code [Planet Editor])
// forecaster (graphics, logo)
//
using System;
using System.Collections.Generic;
using System.Linq;

namespace KRASH.Hyperedit
{
	public static class OrbitEditor
	{

		public static void Simple(OrbitDriver currentlyEditing, double altitude, CelestialBody body)
		{
			Log.Info ("Simple, currentlyEditing:" + currentlyEditing.ToString());
			SetOrbit(currentlyEditing, CreateOrbit(0, 0, altitude + body.Radius, 0, 0, 0, 0, body));
		} 


		public static void GetSimple(OrbitDriver currentlyEditing, out double altitude, out CelestialBody body)
		{
			const int min = 1000;
			const int defaultAlt = 100000;
			body = currentlyEditing.orbit.referenceBody;
			altitude = currentlyEditing.orbit.semiMajorAxis - body.Radius;
			if (altitude > min)
				return;
			altitude = currentlyEditing.orbit.ApA;
			if (altitude > min)
				return;
			altitude = defaultAlt;
		}

		private static void SetOrbit(OrbitDriver currentlyEditing, Orbit orbit)
		{
			Log.Info ("Setorbit");
			currentlyEditing.DynamicSetOrbit(orbit);
		}

		private static Orbit CreateOrbit(double inc, double e, double sma, double lan, double w, double mEp, double epoch, CelestialBody body)
		{
			if (inc == 0)
				inc = 0.0001d;
			if (double.IsNaN(inc))
				inc = 0.0001d;
			
			if (double.IsNaN(e))
				e = 0;
			if (double.IsNaN(sma))
				sma = body.Radius + body.atmosphereDepth + 10000d;
			if (double.IsNaN(lan))
				lan = 0.0001d;
			if (lan == 0)
				lan = 0.0001d;
			
			if (double.IsNaN(w))
				w = 0;
			if (double.IsNaN(mEp))
				mEp = 0;
			if (double.IsNaN(epoch))
				mEp = Planetarium.GetUniversalTime();

			if (Math.Sign(e - 1) == Math.Sign(sma))
				sma = -sma;

			if (Math.Sign(sma) >= 0)
			{
				while (mEp < 0)
					mEp += Math.PI * 2;
				while (mEp > Math.PI * 2)
					mEp -= Math.PI * 2;
			}

			return new Orbit(inc, e, sma, lan, w, mEp, epoch, body);
		}

		public static void DynamicSetOrbit(this OrbitDriver orbit, Orbit newOrbit)
		{
			Log.Info ("DynamicSetOrbit");
			var vessel = orbit.vessel;

			if (vessel != null)
				SetOrbit(vessel, newOrbit);
			else
				HardsetOrbit(orbit, newOrbit);
		}

		public static void SetOrbit(this Vessel vessel, Orbit newOrbit)
		{
			//var originalUp = FlightGlobals.getUpAxis ();
			//Log.Info ("originalUp: " + originalUp.ToString ());
				
			var destinationMagnitude = newOrbit.getRelativePositionAtUT(Planetarium.GetUniversalTime()).magnitude;
			if (destinationMagnitude > newOrbit.referenceBody.sphereOfInfluence)
			{
				Extensions.ErrorPopup("Destination position was above the sphere of influence");
				return;
			}
			if (destinationMagnitude < newOrbit.referenceBody.Radius)
			{
				Extensions.ErrorPopup("Destination position was below the surface");
				return;
			}

			FlightGlobals.fetch.SetShipOrbit(newOrbit.referenceBody.flightGlobalsIndex, newOrbit.eccentricity, newOrbit.semiMajorAxis, newOrbit.inclination, newOrbit.LAN, newOrbit.meanAnomalyAtEpoch, newOrbit.argumentOfPeriapsis, newOrbit.ObT);
			FloatingOrigin.ResetTerrainShaderOffset();

			try
			{
				OrbitPhysicsManager.HoldVesselUnpack(60);
			}
			catch (NullReferenceException)
			{
				Log.Info("OrbitPhysicsManager.HoldVesselUnpack threw NullReferenceException");
			}
		}

		private static readonly object HardsetOrbitLogObject = new object();

		private static void HardsetOrbit(OrbitDriver orbitDriver, Orbit newOrbit)
		{
			Log.Info ("HardsetOrbit");
			var orbit = orbitDriver.orbit;
			orbit.inclination = newOrbit.inclination;
			orbit.eccentricity = newOrbit.eccentricity;
			orbit.semiMajorAxis = newOrbit.semiMajorAxis;
			orbit.LAN = newOrbit.LAN;
			orbit.argumentOfPeriapsis = newOrbit.argumentOfPeriapsis;
			orbit.meanAnomalyAtEpoch = newOrbit.meanAnomalyAtEpoch;
			orbit.epoch = newOrbit.epoch;
			orbit.referenceBody = newOrbit.referenceBody;
			orbit.Init();
			orbit.UpdateFromUT(Planetarium.GetUniversalTime());
			if (orbit.referenceBody != newOrbit.referenceBody)
			{
				if (orbitDriver.OnReferenceBodyChange != null)
					orbitDriver.OnReferenceBodyChange(newOrbit.referenceBody);
			}
			RateLimitedLogger.Log(HardsetOrbitLogObject,
				string.Format("Orbit \"{0}\" changed to: inc={1} ecc={2} sma={3} lan={4} argpe={5} mep={6} epoch={7} refbody={8}",
					orbitDriver.OrbitDriverToString(), orbit.inclination, orbit.eccentricity, orbit.semiMajorAxis,
					orbit.LAN, orbit.argumentOfPeriapsis, orbit.meanAnomalyAtEpoch, orbit.epoch, orbit.referenceBody.CbToString()));
		}
			
		public static Orbit Clone(this Orbit o)
		{
			return new Orbit(o.inclination, o.eccentricity, o.semiMajorAxis, o.LAN,
				o.argumentOfPeriapsis, o.meanAnomalyAtEpoch, o.epoch, o.referenceBody);
		}
	}

	public static class RateLimitedLogger
	{
		private const int MaxFrequency = 100; // measured in number of frames

		class Countdown
		{
			public string lastMessage;
			public int framesLeft;
			public bool needsPrint;

			public Countdown(string msg, int frames)
			{
				lastMessage = msg;
				framesLeft = frames;
				needsPrint = false;
			}
		}

		private static readonly Dictionary<object, Countdown> messages = new Dictionary<object, Countdown>();

		public static void Update()
		{
			List<object> toRemove = null;
			foreach (var kvp in messages)
			{
				if (kvp.Value.framesLeft == 0)
				{
					if (kvp.Value.needsPrint)
					{
						kvp.Value.needsPrint = false;
						kvp.Value.framesLeft = MaxFrequency;
						Info(kvp.Value.lastMessage);
					}
					else
					{
						if (toRemove == null)
							toRemove = new List<object>();
						toRemove.Add(kvp.Key);
					}
				}
				else
				{
					kvp.Value.framesLeft--;
				}
			}
			if (toRemove != null)
			{
				foreach (var key in toRemove)
				{
					messages.Remove(key);
				}
			}
		}

		public static void Log(object key, string message)
		{
			Countdown countdown;
			if (messages.TryGetValue(key, out countdown))
			{
				countdown.needsPrint = true;
				countdown.lastMessage = message;
			}
			else
			{
				Info(message);
				messages[key] = new Countdown(message, MaxFrequency);
			}
		}
		private static readonly String PREFIX = "KRASH" + ": ";
		public static void Info (String msg)
		{
				UnityEngine.Debug.Log (PREFIX + msg);
		}

	}

}
