#if !RP_1_131
using UnityEngine;
using ToolbarControl_NS;

namespace KRASH
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class RegisterToolbar : MonoBehaviour
    {
        void Start()
        {
            ToolbarControl.RegisterMod(LaunchGUI.MODID, LaunchGUI.MODNAME);
        }
    }
}
#endif