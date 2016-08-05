using System;
using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;

namespace KRASH
{
	public class Configuration
	{
		private static int staticCnt = 0;

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
		private static String KRASH_CUSTOM_NODE = "KRASHCustom";
		private static String KRASH_CFG_FILE = KRASH_BASE_FOLDER + "PluginData/KRASH.cfg";
		private static String KRASH_CUSTOM_CFG_FILE = KRASH_BASE_FOLDER + "PluginData/KRASHCustom.cfg";

		public string overrideNode { get; private set;  }
 	
		public void setOverrideNode(string node)
		{
			overrideNode = node;
		}	

		// This stores the nodename
		public string	configName{ get; set; }

		public float	flatSetupCost{ get; set; }
		public float	flatPerMinCost{ get; set; }

		public float	perPartSetupCost{ get; set; }
		public float	perPartPerMinCost{ get; set; }

		public float	perTonSetupCost{ get; set; }
		public float	perTonPerMinCost{ get; set; }

		public float	percentSetupCost{ get; set; }
		public float	percentPerMinCost{ get; set; }

		public float	AtmoMultipler{ get; set; }

		public bool		ContinueIfNoCash{ get; set; }

		public bool		TerminateAtSoiWithoutData{ get; set; }
		public bool		TerminateAtLandWithoutData{ get; set; }
		public bool		TerminateAtAtmoWithoutData{ get; set; }

		public float	DefaultMaxAllowableSimCost { get; set; }
		public int		DefaultSimTime { get; set; }

	//	public string	selectedCosts {get; set; }
		public bool		showRunningSimCosts{ get; set; }
		public int		horizontalPos {get; set; }
		public int		verticalPos{ get; set;}
        public bool     showAllInCareer { get; set; }

		public int cnt;

		public Configuration ()
		{
//			StackTrace s  = new System.Diagnostics.StackTrace() ;
//			Log.Info("Configuration Stacktrace: " + s);

			cnt = staticCnt++;
			configName = "";
			overrideNode = "";

			flatSetupCost = 0.0F;
			flatPerMinCost = 0.0F;

			perPartSetupCost = 0.0F;
			perPartPerMinCost = 0.0F;

			perTonSetupCost = 0.0F;
			perTonPerMinCost = 0.0F;

			percentSetupCost = 0.0F;
			percentPerMinCost = 0.0F;

			AtmoMultipler = 1.0F;
			TerminateAtSoiWithoutData = false;
			TerminateAtLandWithoutData = false;
			TerminateAtAtmoWithoutData = false;

			ContinueIfNoCash = false;
			DefaultMaxAllowableSimCost = 0.0F;
			DefaultSimTime = 5;

			//selectedCosts = "default";

			showRunningSimCosts = true;
			horizontalPos = 10;
			verticalPos = 50;

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
		static string SafeLoad (string value, string oldvalue)
		{
			if (value == null)
				return oldvalue;
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

			percentSetupCost = float.Parse (SafeLoad (node.GetValue ("percentSetupCost"), perTonSetupCost * 100)) / 100;
			percentPerMinCost = float.Parse (SafeLoad (node.GetValue ("percentPerMinCost"), perTonPerMinCost * 100)) / 100;


			AtmoMultipler = float.Parse (SafeLoad (node.GetValue ("AtmoPerMinMultipler"), AtmoMultipler));
			TerminateAtSoiWithoutData = bool.Parse(SafeLoad(node.GetValue("TerminateAtSoiWithoutData"), TerminateAtSoiWithoutData));
			TerminateAtLandWithoutData = bool.Parse(SafeLoad(node.GetValue("TerminateAtLandWithoutData"), TerminateAtLandWithoutData));
			TerminateAtAtmoWithoutData = bool.Parse(SafeLoad(node.GetValue("TerminateAtAtmoWithoutData"), TerminateAtAtmoWithoutData));

			ContinueIfNoCash = bool.Parse (SafeLoad (node.GetValue ("ContinueIfNoCash"), ContinueIfNoCash));
			DefaultMaxAllowableSimCost = float.Parse (SafeLoad (node.GetValue ("DefaultMaxAllowableSimCost"), DefaultMaxAllowableSimCost));
			DefaultSimTime = int.Parse (SafeLoad (node.GetValue ("DefaultSimTime"), DefaultSimTime));

			KRASHShelter.persistent.selectedCostsCfg = nodename;

			//selectedCosts = SafeLoad (node.GetValue ("selectedCosts"), selectedCosts);
		}

		public void setDisplayValues()
		{
			Log.Info ("setDisplayValues");
			configFile = ConfigNode.Load (KRASH_CUSTOM_CFG_FILE);
			if (configFile != null) {
				ConfigNode node = configFile.GetNode (KRASH_CUSTOM_NODE);

				showRunningSimCosts = bool.Parse (SafeLoad (node.GetValue ("showRunningSimCosts"), showRunningSimCosts));
				horizontalPos = int.Parse (SafeLoad (node.GetValue ("horizontalPos"), horizontalPos));
				verticalPos = int.Parse (SafeLoad (node.GetValue ("verticalPos"), verticalPos));
                showAllInCareer = bool.Parse(SafeLoad(node.GetValue("showAllInCareer"), showAllInCareer));

            } else {
				showRunningSimCosts = true;
                showAllInCareer = false;
				horizontalPos = 10;
				verticalPos = 50;

			}
			//Log.Info ("horizontalPos: " + horizontalPos.ToString ());
			//Log.Info ("verticalpos: " + verticalPos.ToString ());
			//LogConfiguration(nodename);
		}

		public void saveDisplayValues()
		{
 //           ConfigNode node;

            Log.Info ("saveDisplayValues");
			configFile = ConfigNode.Load (KRASH_CUSTOM_CFG_FILE);
            if (configFile == null)
            {
                configFile = new ConfigNode();
                configFileNode = new ConfigNode(KRASH_CUSTOM_NODE);
            }
            else
            {
                configFileNode = configFile.GetNode(KRASH_CUSTOM_NODE);
               
                configFile.RemoveNode(KRASH_CUSTOM_NODE);
                //configFile.SetNode(KRASH_CUSTOM_NODE, configFileNode, true);
            }
            Log.Info("saveDisplayValues 1");
            configFileNode.SetValue ("showRunningSimCosts", showRunningSimCosts.ToString(), true);
            configFileNode.SetValue("horizontalPos", horizontalPos.ToString(), true);
            configFileNode.SetValue("verticalPos", verticalPos.ToString(), true);
            configFileNode.SetValue("showAllInCareer", showAllInCareer.ToString(), true);

            configFile.SetNode(KRASH_CUSTOM_NODE, configFileNode, true);
            //configFile.AddNode (KRASH_CUSTOM_NODE, configFileNode);
            configFile.Save(KRASH_CUSTOM_CFG_FILE);
		}

		public void DeleteConfiguration (string strConfigName)
		{
			if (strConfigName [0] == '*') {
				return;
			} 
			configFile = ConfigNode.Load (KRASH_CUSTOM_CFG_FILE);

			if (configFile != null) {
				configFileNode = configFile.GetNode (KRASH_CUSTOM_NODE);
				if (configFileNode != null) {
					Log.Info ("Deleting node: " + strConfigName);
					configFileNode.RemoveNode (strConfigName);
					configFile.Save (KRASH_CUSTOM_CFG_FILE);
				}
			}
		}

		public void SaveConfiguration(string configName)
		{
			Log.Info ("SaveConfiguration");
			ConfigNode cfgNode = new ConfigNode();

			if (configName [0] == '*') {
				configName = configName.Substring (2);

			} 
			configFile = ConfigNode.Load (KRASH_CUSTOM_CFG_FILE);

			if (configFile != null) {
				configFileNode = configFile.GetNode (KRASH_CUSTOM_NODE);
				if (configFileNode != null) {
					cfgNode.SetValue ("flatSetupCost", flatSetupCost.ToString (), true);
					cfgNode.SetValue ("flatPerMinCost", flatPerMinCost.ToString (), true);
					cfgNode.SetValue ("perPartSetupCost", perPartSetupCost.ToString (), true);
					cfgNode.SetValue ("perPartPerMinCost", perPartPerMinCost.ToString (), true);
					cfgNode.SetValue ("perTonSetupCost", perTonSetupCost.ToString (), true);
					cfgNode.SetValue ("perTonPerMinCost", perTonPerMinCost.ToString (), true);
					cfgNode.SetValue ("percentSetupCost", (100*percentSetupCost).ToString (), true);
						cfgNode.SetValue ("percentPerMinCost", (100*percentPerMinCost).ToString (), true);
					cfgNode.SetValue ("AtmoMultipler", AtmoMultipler.ToString (), true);
					cfgNode.SetValue ("TerminateAtSoiWithoutData", TerminateAtSoiWithoutData.ToString (), true);
					cfgNode.SetValue ("TerminateAtLandWithoutData", TerminateAtLandWithoutData.ToString (), true);
					cfgNode.SetValue ("TerminateAtAtmoWithoutData", TerminateAtAtmoWithoutData.ToString (), true);
					cfgNode.SetValue ("ContinueIfNoCash", ContinueIfNoCash.ToString (), true);
					cfgNode.SetValue ("DefaultMaxAllowableSimCost", DefaultMaxAllowableSimCost.ToString (), true);
					cfgNode.SetValue ("DefaultSimTime", DefaultSimTime.ToString (), true);

                    //configFileNode.SetValue ("selectedCosts", selectedCosts.ToString (), true);
                    configFileNode.SetValue("showRunningSimCosts", showRunningSimCosts.ToString(), true);
                    configFileNode.SetValue("showAllInCareer", showAllInCareer.ToString(), true);

                    configFileNode.SetValue ("horizontalPos", horizontalPos.ToString (), true);
					configFileNode.SetValue ("verticalPos", verticalPos.ToString (), true);

					configFileNode.RemoveNode (configName);
					configFileNode.AddNode (configName, cfgNode);
					configFile.Save (KRASH_CUSTOM_CFG_FILE);
				}
			}
			saveDisplayValues ();
		}

		public bool LoadConfiguration (string nodename)
		{
			Log.Info ("Loading config: " + nodename);
			string node;
			configName = nodename;
			if (configName [0] == '*') {
				configName = configName.Substring (2);
				configFile = ConfigNode.Load (KRASH_CFG_FILE);
				node = KRASH_NODE;
			} else {
				configFile = ConfigNode.Load (KRASH_CUSTOM_CFG_FILE);
				node = KRASH_CUSTOM_NODE;
			}

			if (configFile != null) {
				configFileNode = configFile.GetNode (node);
				if (configFileNode != null) {
					SetConfiguration ("defaults", configFileNode);

					if (overrideNode == "") {
						krashNode = configFileNode.GetNode (configName);
					} else {
						krashNode = configFileNode.GetNode (overrideNode);
					}
					if (krashNode != null) {
						SetConfiguration (nodename, krashNode);
					} 
				} else
					return false;
			} else
				return false;

			setDisplayValues ();

			Log.Info ("config loaded");
			return true;
		}

		public List<string> GetAvailableCfgs()
		{
			List<string> cfgs = new List<string> ();
			configFile = ConfigNode.Load (KRASH_CFG_FILE);

			if (configFile != null) {
				configFileNode = configFile.GetNode (KRASH_NODE);
				ConfigNode[] l = configFileNode.GetNodes ();
				foreach (ConfigNode n in l) {
					string s = n.name;
					cfgs.Add ("* " + s);
				}
			}

			configFile = ConfigNode.Load (KRASH_CUSTOM_CFG_FILE);

			if (configFile != null) {
				configFileNode = configFile.GetNode (KRASH_CUSTOM_NODE);
				if (configFileNode != null) {
					ConfigNode[] l = configFileNode.GetNodes ();
					foreach (ConfigNode n in l) {
						string s = n.name;
						cfgs.Add (s);
					}
				}
			}
				
			return cfgs;
		}
	}
}