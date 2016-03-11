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
using UnityEngine;

namespace KRASH.Hyperedit
{
	public static class DoLander
	{
		private const string OldFilename = "landcoords.txt";
		private const string FilenameNoExt = "landcoords";

		public static bool IsLanding()
		{
			if (FlightGlobals.fetch == null || FlightGlobals.ActiveVessel == null)
				return false;
			return FlightGlobals.ActiveVessel.GetComponent<LanderAttachment>() != null;
		}

		public static void ToggleLanding(double latitude, double longitude, double altitude, CelestialBody body, Action<double, double, CelestialBody> onManualEdit)
		{
			Log.Info ("ToggleLanding");
			if (FlightGlobals.fetch == null || FlightGlobals.ActiveVessel == null || body == null)
				return;
			var lander = FlightGlobals.ActiveVessel.GetComponent<LanderAttachment>();
			if (lander == null)
			{
				lander = FlightGlobals.ActiveVessel.gameObject.AddComponent<LanderAttachment>();
				lander.Latitude = latitude;
				lander.Longitude = longitude;
				lander.Altitude = altitude;
				Log.Info ("lander.Altitude: " + lander.Altitude.ToString ());
				lander.Body = body;
				lander.OnManualEdit = onManualEdit;
			}
			else
			{
				UnityEngine.Object.Destroy(lander);
			}
		}
	}

	public class LanderAttachment : MonoBehaviour
	{
		public bool AlreadyTeleported { get; set; }
		public Action<double, double, CelestialBody> OnManualEdit { get; set; }
		public CelestialBody Body { get; set; }
		public double Latitude { get; set; }
		public double Longitude { get; set; }
		public double Altitude { get; set; }

		private readonly object accelLogObject = new object();

		public void Update()
		{
			// 0.2 meters per frame
			var degrees = 0.2 / Body.Radius * (180 / Math.PI);
			var changed = false;
			if (GameSettings.TRANSLATE_UP.GetKey())
			{
				Latitude -= degrees;
				changed = true;
			}
			if (GameSettings.TRANSLATE_DOWN.GetKey())
			{
				Latitude += degrees;
				changed = true;
			}
			if (GameSettings.TRANSLATE_LEFT.GetKey())
			{
				Longitude -= degrees / Math.Cos(Latitude * (Math.PI / 180));
				changed = true;
			}
			if (GameSettings.TRANSLATE_RIGHT.GetKey())
			{
				Longitude += degrees / Math.Cos(Latitude * (Math.PI / 180));
				changed = true;
			}
			if (changed)
			{
				AlreadyTeleported = false;
				OnManualEdit(Latitude, Longitude, Body);
			}
		}

		public void FixedUpdate()
		{
			var vessel = GetComponent<Vessel>();
			if (vessel != FlightGlobals.ActiveVessel)
			{
				KRASHShelter.instance.SetSimActiveNotification ();
				Destroy(this);
				return;
			}
			KRASHShelter.instance.SetSimNotification ("Simulation Setup in Progress");
			if (AlreadyTeleported)
			{
				if (vessel.LandedOrSplashed)
				{
					KRASHShelter.instance.SetSimActiveNotification ();
					Destroy(this);
				}
				else
				{
					var accel = (vessel.srf_velocity + vessel.upAxis) * -0.5;
					vessel.ChangeWorldVelocity(accel);
					Hyperedit.RateLimitedLogger.Log(accelLogObject,
						string.Format("(Happening every frame) Soft-lander changed ship velocity this frame by vector {0},{1},{2} (mag {3})",
							accel.x, accel.y, accel.z, accel.magnitude));
				}
			}
			else
			{
				Log.Info ("NotTeleported");
				var pqs = Body.pqsController;
				if (pqs == null)
				{
					KRASHShelter.instance.SetSimActiveNotification ();
					Destroy(this);
					return;
				}

				var alt = pqs.GetSurfaceHeight(
					QuaternionD.AngleAxis(Longitude, Vector3d.down) *
					QuaternionD.AngleAxis(Latitude, Vector3d.forward) * Vector3d.right) -
					pqs.radius;
				alt = Math.Max(alt, 0); // Underwater!
				if (TimeWarp.CurrentRateIndex != 0)
				{
					TimeWarp.SetRate(0, true);
					Log.Info("Set time warp to index 0");
				}
				// HoldVesselUnpack is in display frames, not physics frames
				Log.Info("alt: " + alt.ToString() + "   Altitude: " + Altitude.ToString());
				Log.Info ("Latitude: " + Latitude.ToString () + "   Longitude: " + Longitude.ToString ());
				var teleportPosition = Body.GetRelSurfacePosition(Latitude, Longitude, alt + Altitude);
				var teleportVelocity = Body.getRFrmVel(teleportPosition + Body.position);
				// convert from world space to orbit space
				teleportPosition = teleportPosition.xzy;
				teleportVelocity = teleportVelocity.xzy;
				// counter for the momentary fall when on rails (about one second)
				teleportVelocity += teleportPosition.normalized * (Body.gravParameter / teleportPosition.sqrMagnitude);

				var orbit = vessel.orbitDriver.orbit.Clone();
				orbit.UpdateFromStateVectors(teleportPosition, teleportVelocity, Body, Planetarium.GetUniversalTime());

				vessel.SetOrbit(orbit);
#if false

				#if false
				try
				{
					Log.Info ("FixedUpdate HoldVesselUnpack");
					OrbitPhysicsManager.HoldVesselUnpack(60);
				}
				catch (NullReferenceException)
				{
					Log.Info("OrbitPhysicsManager.HoldVesselUnpack threw NullReferenceException");
				}
				#endif

				// rotation code
				var newUp = FlightGlobals.getUpAxis();

				#if false
				//set the vessel's up vector
				vessel.rootPart.partTransform.up = newUp;
				//get the rotation that resulted from setting the up vector
				var newRot = vessel.rootPart.partTransform.rotation;

				//set the rotation
				vessel.SetRotation(newRot);
				#endif

				// Get the direction from the world origin to target position
				Vector3d curpos = vessel.GetWorldPos3D ();
				Vector3d gee = FlightGlobals.getGeeForceAtPosition( vessel.GetWorldPos3D () );
				//curpos.x = 0;
				Quaternion normal = Quaternion.LookRotation (curpos);

				var diff = Quaternion.FromToRotation(KRASHShelter.originalUp, gee);

				vessel.SetRotation (diff * normal);
//				vessel.SetRotation(normal2);

				// adjust vessel rotation based on how much the up direction changed
//				var diff = Quaternion.FromToRotation(curpos, newpos);
//				vessel.SetRotation(diff * normal);

				// adjust vessel rotation based on how much the up direction changed
//				var diff = Quaternion.FromToRotation(KRASHShelter.originalUp, newUp);

//				vessel.SetRotation(diff * vessel.transform.rotation);
				// rotation code


				// rotation code
				Log.Info("originalUp: " + KRASHShelter.originalUp.ToString());
				Log.Info ("newUp: " + newUp.ToString ());
				Log.Info ("gee: " + gee.ToString ());
				Log.Info ("diff: " + diff.ToString ());

//				vessel.SetOrbit(orbit);
#endif
				AlreadyTeleported = true;
			}
		}
	}
}
