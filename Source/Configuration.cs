using System;
using UnityEngine;


namespace KRASH
{
	public class Configuration
	{
		public static string EASY = "Easy";
		public static string NORMAL = "Normal";
		public static string MODERATE = "Moderate";
		public static string HARD = "Hard";
		public static string CUSTOM = "Custom";

		private static ConfigNode configFile = null;
		private static ConfigNode configFileNode = null;
		private static ConfigNode krashNode = null;

		public static readonly String ROOT_PATH = KSPUtil.ApplicationRootPath;
		private static readonly String CONFIG_BASE_FOLDER = ROOT_PATH + "GameData/";
		private static String KRASH_BASE_FOLDER = CONFIG_BASE_FOLDER + "KRASH/";
		private static String KRASH_NODE = "KRASH";
		private static String KRASH_CFG_FILE = KRASH_BASE_FOLDER + "KRASH.cfg";

		public string overrideNode { get; private set;  }
 	
		public void setOverrideNode(string node)
		{
			overrideNode = node;
		}	

		public float	flatSetupCost{ get; set; }
		public float	flatPerMinCost{ get; set; }

		public float	perPartSetupCost{ get; set; }
		public float	perPartPerMinCost{ get; set; }

		public float	perTonSetupCost{ get; set; }
		public float	perTonPerMinCost{ get; set; }

		public float	AtmoMultipler{ get; set; }

		public bool		ContinueIfNoCash{ get; set; }

		public bool		TerminateAtSoiWithoutData{ get; set; }
		public bool		TerminateAtLandWithoutData{ get; set; }
		public bool		TerminateAtAtmoWithoutData{ get; set; }

		public float	MaxAllowableSimCost { get; set; }
		public int		DefaultSimTime { get; set; }

		public Configuration ()
		{
			overrideNode = "";

			flatSetupCost = 0.0F;
			flatPerMinCost = 0.0F;

			perPartSetupCost = 0.0F;
			perPartPerMinCost = 0.0F;

			perTonSetupCost = 0.0F;
			perTonPerMinCost = 0.0F;

			AtmoMultipler = 1.0F;
			TerminateAtSoiWithoutData = false;
			TerminateAtLandWithoutData = false;
			TerminateAtAtmoWithoutData = false;

			ContinueIfNoCash = false;
			MaxAllowableSimCost = 0.0F;
			DefaultSimTime = 5;
		}

		static string SafeLoad (string value, float oldvalue)
		{
			if (value == null)
				return oldvalue.ToString();
			return value;
		}
		static string SafeLoad (string value, int oldvalue)
		{
			if (value == null)
				return oldvalue.ToString();
			return value;
		}
		static string SafeLoad (string value, bool oldvalue)
		{
			if (value == null)
				return oldvalue.ToString();
			return value;
		}

		#if false
		private void LogConfiguration(string node)
		{
			Log.Info ("Config node: " + node);
			Log.Info ("flatSetupCost: " + flatSetupCost.ToString ());
			Log.Info ("flatPerMinCost: " + flatPerMinCost.ToString ());
			Log.Info ("perPartSetupCost: " + perPartSetupCost.ToString ());
			Log.Info ("perPartPerMinCost: " + perPartPerMinCost.ToString ());
			Log.Info ("perTonSetupCost: " + perTonSetupCost.ToString ());
			Log.Info ("perTonPerMinCost: " + perTonPerMinCost.ToString ());
			Log.Info ("AtmoPerMinMultipler: " + AtmoMultipler.ToString ());
			Log.Info ("TerminateAtLandWithoutData: " + TerminateAtLandWithoutData.ToString ());
			Log.Info ("TerminateAtSoiWithoutData: " + TerminateAtSoiWithoutData.ToString ());
			Log.Info ("TerminateAtAtmoWithoutData: " + TerminateAtAtmoWithoutData.ToString ());
			Log.Info ("ContinueIfNoCash: " + ContinueIfNoCash.ToString());
			Log.Info ("MaxAllowableSimCost: " + MaxAllowableSimCost.ToString());
			Log.Info ("DefaultSimTime: " + DefaultSimTime.ToString ());
		}
		#endif

		private void SetConfiguration(string nodename, ConfigNode node)
		{
			flatSetupCost = float.Parse (SafeLoad (node.GetValue ("flatSetupCost"), flatSetupCost));
			flatPerMinCost = float.Parse (SafeLoad (node.GetValue ("flatPerMinCost"), flatPerMinCost));

			perPartSetupCost = float.Parse (SafeLoad (node.GetValue ("perPartSetupCost"), perPartSetupCost));
			perPartPerMinCost = float.Parse (SafeLoad (node.GetValue ("perPartPerMinCost"), perPartPerMinCost));

			perTonSetupCost = float.Parse (SafeLoad (node.GetValue ("perTonSetupCost"), perTonSetupCost));
			perTonPerMinCost = float.Parse (SafeLoad (node.GetValue ("perTonPerMinCost"), perTonPerMinCost));

			AtmoMultipler = float.Parse (SafeLoad (node.GetValue ("AtmoPerMinMultipler"), AtmoMultipler));
			TerminateAtSoiWithoutData = bool.Parse(SafeLoad(node.GetValue("TerminateAtSoiWithoutData"), TerminateAtSoiWithoutData));
			TerminateAtLandWithoutData = bool.Parse(SafeLoad(node.GetValue("TerminateAtLandWithoutData"), TerminateAtLandWithoutData));
			TerminateAtAtmoWithoutData = bool.Parse(SafeLoad(node.GetValue("TerminateAtAtmoWithoutData"), TerminateAtAtmoWithoutData));

			ContinueIfNoCash = bool.Parse (SafeLoad (node.GetValue ("ContinueIfNoCash"), ContinueIfNoCash));
			MaxAllowableSimCost = float.Parse (SafeLoad (node.GetValue ("MaxAllowableSimCost"), MaxAllowableSimCost));
			DefaultSimTime = int.Parse (SafeLoad (node.GetValue ("DefaultSimTime"), DefaultSimTime));
			//LogConfiguration(nodename);
		}

		public void LoadConfiguration (string nodename)
		{
			configFile = ConfigNode.Load (KRASH_CFG_FILE);

			if (configFile != null) {
				configFileNode = configFile.GetNode (KRASH_NODE);
				if (configFileNode != null) {
					SetConfiguration (nodename + "  defaults: ", configFileNode);

					if (overrideNode == "") {
						krashNode = configFileNode.GetNode (nodename);
					} else {
						krashNode = configFileNode.GetNode (overrideNode);
					}
					if (krashNode != null) {
						SetConfiguration (nodename, krashNode);
					} 
				}
			}
		}
	}
}