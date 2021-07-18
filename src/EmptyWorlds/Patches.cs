using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Database;
using PeterHan.PLib.Options;

namespace EmptyWorlds
{

    public class EmptyWorlds : UserMod2
    {
        public static EmptyWorldsOptions Settings { get; private set; }

        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            new POptions().RegisterOptions(this, typeof(EmptyWorldsOptions));

            // Init PLib and settings
            PUtil.InitLibrary();

            Settings = POptions.ReadSettings<EmptyWorldsOptions>();
            if (Settings == null)
            {
                Settings = new EmptyWorldsOptions();
            }
            new PLocalization().Register();
        }
    }

    [HarmonyPatch(typeof(TemplateCache), "GetTemplate")]
    public class TemplateCache_GetTemplate
    {
        /*public static void Postfix(ref TemplateContainer __result)
        {
            if (EmptyWorlds.Settings.RemoveBunkerAtWorldgen)
            {
                if (__result.name == "poi_bunker_skyblock")
                {
                    //__result = new TemplateContainer();
                }
            }
        }*/
    }

}
