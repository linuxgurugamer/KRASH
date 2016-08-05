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
	public delegate bool TryParse<T>(string str, out T value);

	public static class Extensions
	{

//		public static void Log(string message)
//		{
//			Debug.Log("KRASH: " + message);
//		}
		public static void ErrorPopup(string message)
		{
			//PopupDialog.SpawnPopupDialog("Error", message, "Close", true, HighLogic.Skin);
			PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), "Error", message, "Close", true, HighLogic.UISkin);
		}
		public static void PrepVesselTeleport(this Vessel vessel)
		{
			Log.Info ("PrepVesselTeleport");
			if (vessel.Landed)
			{
				vessel.Landed = false;
				Log.Info("Set ActiveVessel.Landed = false");
			}
			if (vessel.Splashed)
			{
				vessel.Splashed = false;
				Log.Info("Set ActiveVessel.Splashed = false");
			}
			if (vessel.landedAt != string.Empty)
			{
				vessel.landedAt = string.Empty;
				Log.Info("Set ActiveVessel.landedAt = \"\"");
			}
			var parts = vessel.parts;
			if (parts != null)
			{
				int killcount = 0;
				foreach (var part in parts.Where(part => part.Modules.OfType<LaunchClamp>().Any()).ToList())
				{
					killcount++;
					part.Die();
				}
				if (killcount != 0)
				{
					Log.Info(string.Format("Removed {0} launch clamps from {1}", killcount, vessel.vesselName));
				}
			}
		}
		public static double Soi(this CelestialBody body)
		{
			var radius = body.sphereOfInfluence * 0.95;
			if (double.IsNaN(radius) || double.IsInfinity(radius) || radius < 0 || radius > 200000000000)
				radius = 200000000000; // jool apo = 72,212,238,387
			return radius;
		}
		public static double Mod(this double x, double y)
		{
			var result = x % y;
			if (result < 0)
				result += y;
			return result;
		}
		public static string CbToString(this CelestialBody body)
		{
			return body.bodyName;
		}
		public static string OrbitDriverToString(this OrbitDriver driver)
		{
			if (driver == null)
				return null;
			if (driver.celestialBody != null)
				return driver.celestialBody.bodyName;
			if (driver.vessel != null)
				return driver.vessel.VesselToString();
			if (!string.IsNullOrEmpty(driver.name))
				return driver.name;
			return "Unknown";
		}

		public static string VesselToString(this Vessel vessel)
		{
			if (FlightGlobals.fetch != null && FlightGlobals.ActiveVessel == vessel)
				return "Active vessel";
			return vessel.vesselName;
		}
		public static void ClearGuiFocus()
		{
			GUIUtility.keyboardControl = 0;
		}
		private static GUIStyle _pressedButton;

		public static GUIStyle PressedButton
		{
			get
			{
				return _pressedButton ?? (_pressedButton = new GUIStyle(HighLogic.Skin.button)
					{
						normal = HighLogic.Skin.button.active,
						hover = HighLogic.Skin.button.active,
						active = HighLogic.Skin.button.normal
					});
			}
		}
	}
}

