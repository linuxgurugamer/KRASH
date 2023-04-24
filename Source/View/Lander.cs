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
using KSP.Localization;

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
			if (FlightGlobals.fetch == null || FlightGlobals.ActiveVessel == null || body == null)
				return;
			Log.Info ("KRASH.HyperEdit.ToggleLanding");
			var lander = FlightGlobals.ActiveVessel.GetComponent<LanderAttachment>();
			if (lander == null)
			{
				lander = FlightGlobals.ActiveVessel.gameObject.AddComponent<LanderAttachment>();
				lander.Latitude = latitude;
				lander.Longitude = longitude;
				lander.Altitude = altitude;
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

		//private PopupDialog _activePopup;
		bool pausebeforestarting = false;

		private void drawPauseAfterLanding ()
		{
			GUILayout.Label (Localizer.GetStringByTag("#KRASH_PauseAfterVesselLanded")); // "Vessel has landed.  Click the OK button to continue"
			if (GUILayout.Button ("OK")) {
				FlightDriver.SetPause (false);
				KRASHShelter.instance.SetSimActiveNotification ();
				Destroy (this);
				//_activePopup.Dismiss ();
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
					if (!pausebeforestarting)
					{
						pausebeforestarting = true;
						FlightDriver.SetPause (true);
						//PopupDialog.SpawnPopupDialog ("Vessel has landed", "Vessel has landed.  Click the OK button to continue", "OK", true, HighLogic.Skin);
						var dialog = new MultiOptionDialog ("krash4", Localizer.GetStringByTag("#KRASH_PauseAfterVesselLanded"), "", HighLogic.UISkin, new DialogGUIBase[] { // "Vessel has landed.  Click the OK button to continue"
							new DialogGUIButton ("OK", () => {
								FlightDriver.SetPause (false);
								KRASHShelter.instance.SetSimActiveNotification ();
								Destroy (this);
							//	_activePopup.Dismiss ();
							})
						});
						PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), dialog, false, HighLogic.UISkin, true);

					}
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

                //var teleportPosition = Body.GetRelSurfacePosition(Latitude, Longitude, alt + Altitude);
                var teleportPosition = Body.GetWorldSurfacePosition(Latitude, Longitude, alt + Altitude) - Body.position;

                var teleportVelocity = Body.getRFrmVel(teleportPosition + Body.position);
				// convert from world space to orbit space
				teleportPosition = teleportPosition.xzy;
				teleportVelocity = teleportVelocity.xzy;
				// counter for the momentary fall when on rails (about one second)
				teleportVelocity += teleportPosition.normalized * (Body.gravParameter / teleportPosition.sqrMagnitude);

			//	var oldUp = vessel.orbit.pos.xzy.normalized; // can also be vessel.vesselTransform.position, I think
			//	var newUp = teleportPosition.xzy.normalized; // teleportPosition is the orbitspace location (note the .xzy)
			//	var rotation = Quaternion.FromToRotation(oldUp, newUp)*vessel.vesselTransform.rotation;

				var from = Vector3d.up;
				var to = teleportPosition.xzy.normalized;
				var rotation = Quaternion.FromToRotation(from, to);
			

				var orbit = vessel.orbitDriver.orbit.Clone();
				orbit.UpdateFromStateVectors(teleportPosition, teleportVelocity, Body, Planetarium.GetUniversalTime());


				vessel.SetOrbit(orbit);
				vessel.SetRotation(rotation);

				AlreadyTeleported = true;
			}
		}
	}
}
