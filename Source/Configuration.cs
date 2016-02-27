using System;
using UnityEngine;

#if false
namespace KRASH
{
	public class Configuration
	{
		private static ConfigNode configFile = null;
		private static ConfigNode configFileNode = null;

		public static readonly String ROOT_PATH = KSPUtil.ApplicationRootPath;
		private static readonly String CONFIG_BASE_FOLDER = ROOT_PATH + "GameData/";
		private static String KRASH_BASE_FOLDER = CONFIG_BASE_FOLDER + "KRASH/";
		private static String KRASH_NODENAME = "KRASH";
		private static String KRASH_CFG_FILE = KRASH_BASE_FOLDER + "KRASH.cfg";


		public float	flatSetupCost{ get; set; }
		public float	flatPerMinCost{ get; set; }

		public float	perPartSetupCost{ get; set; }
		public float	perPartPerMinCost{ get; set; }

		public float	perTonSetupCost{ get; set; }
		public float	perTonPerMinCost{ get; set; }

		public Configuration ()
		{
			flatSetupCost = 0.0F;
			flatPerMinCost = 0.0F;

			perPartSetupCost = 0.0F;
			perPartPerMinCost = 0.0F;

			perTonSetupCost = 0.0F;
			perTonPerMinCost = 0.0F;
		}

		static string SafeLoad (string value, float oldvalue)
		{
			if (value == null)
				return oldvalue.ToString();
			return value;
		}

		public static void LoadConfiguration (Configuration configuration, String file)
		{
			configFile = ConfigNode.Load (KRASH_CFG_FILE);

			if (configFile != null) {
				configFileNode = configFile.GetNode (KRASH_NODENAME);
				if (configFileNode != null) {
					configuration.flatSetupCost = float.Parse (SafeLoad(configFileNode.GetValue ("flatSetupCost"),configuration.flatSetupCost));
					configuration.flatPerMinCost = float.Parse (SafeLoad(configFileNode.GetValue ("flatPerMinCost"),configuration.flatPerMinCost));

					configuration.perPartSetupCost = float.Parse (SafeLoad(configFileNode.GetValue ("perPartSetupCost"),configuration.perPartSetupCost));
					configuration.perPartPerMinCost = float.Parse (SafeLoad(configFileNode.GetValue ("perPartPerMinCost"),configuration.perPartPerMinCost));

					configuration.perTonSetupCost = float.Parse (SafeLoad(configFileNode.GetValue ("perTonSetupCost"),configuration.perTonSetupCost));
					configuration.perTonPerMinCost = float.Parse (SafeLoad(configFileNode.GetValue ("perTonPerMinCost"),configuration.perTonPerMinCost));

				}
			}
		}
	}
}

#endif