using System;
using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;

namespace High_Pressure_Applications.Components
{
    public class HPA_ModSettingsMenu : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            PUtil.InitLibrary(true);
            new POptions().RegisterOptions(this, typeof(HPA_ModSettings));
        }
    }
}
