using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace KRASH
{
    // http://forum.kerbalspaceprogram.com/index.php?/topic/147576-modders-notes-for-ksp-12/#comment-2754813
    // search for "Mod integration into Stock Settings
    public class KRASH_Settings : GameParameters.CustomParameterNode
    {
        public override string Title { get { return ""; } } // column heading
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "KRASH"; } }
        public override string DisplaySection { get { return "KRASH"; } }
        public override int SectionOrder { get { return 1; } }
        public override bool HasPresets { get { return false; } }


        [GameParameters.CustomParameterUI("#KRASH_Settings_Wireframes")] // "Wireframes"
        public bool wireframes = false;

        [GameParameters.CustomFloatParameterUI("#KRASH_Settings_defaultAtmoStartingAltitude", minValue = 10000.0f, maxValue = 250000.0f, // "Starting Orbital Altitude (atmo)"
            toolTip = "#KRASH_Settings_defaultAtmoStartingAltitude_tooltip")] //"For planets and moons with atmosphere"
        public double defaultAtmoStartingAltitude = 75000f;

        [GameParameters.CustomFloatParameterUI("#KRASH_Settings_defaultNoAtmoStartingAltitude", minValue = 1000.0f, maxValue = 250000.0f, //"Starting Orbital Altitude (no atmo)"
            toolTip = "#KRASH_Settings_defaultNoAtmoStartingAltitude_tooltip")] // "For planets and moons without any atmosphere"
        public double defaultNoAtmoStartingAltitude = 10000f;


        public float costAdjustment = 1.0f;
        [GameParameters.CustomFloatParameterUI("#KRASH_Settings_costAdjustment", minValue = 25f, maxValue = 200.0f, // "Cost Adjustment (%)"
                 toolTip = "#KRASH_Settings_costAdjustment_tooltip")] // "Adjusts the costs by this value "
        public float CostAdj
        {
            get { return costAdjustment * 100; }
            set { costAdjustment = value / 100.0f; }
        }
        public float CostAdjustment
        {
            get { return costAdjustment; }
            set { costAdjustment = value; }
        }

        [GameParameters.CustomParameterUI("#KRASH_Settings_noChargeDuringTimewarp", // "Don't charge during timewarp"
         toolTip = "#KRASH_Settings_noChargeDuringTimewarp_tooltip")] // "If enabled, will not charge any funds while timewarping"
        public bool noChargeDuringTimewarp = true;


        public override bool Enabled(MemberInfo member, GameParameters parameters)
        {
            return true; //otherwise return true
        }

        public override bool Interactible(MemberInfo member, GameParameters parameters)
        {

            return true;
            //            return true; //otherwise return true
        }

        public override IList ValidValues(MemberInfo member)
        {
            return null;
        }

    }
}
