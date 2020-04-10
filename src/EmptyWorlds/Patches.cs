using Harmony;
using PeterHan.PLib;
using PeterHan.PLib.Options;
using Pholib;
using System.Collections.Generic;

namespace EmptyWorlds
{

    public class OnLoadPatch
    {
        public static string modPath;
        public static EmptyWorldsOptions Settings { get; private set; }

        public static void OnLoad(string modPath)
        {
            OnLoadPatch.modPath = modPath;

            // Init PLib and settings
            PUtil.InitLibrary();
            POptions.RegisterOptions(typeof(EmptyWorldsOptions));

            Settings = POptions.ReadSettings<EmptyWorldsOptions>();
            if (Settings == null)
            {
                Settings = new EmptyWorldsOptions();
            }
        }
    }

    [HarmonyPatch(typeof(Db), "Initialize")]
    public class WorldsPatches
    {
        public static string modPath;
        
        public static LocString SPACE_HOLE_NAME = "Holes";
        public static LocString SPACE_HOLE_DESC = "This world is completely full of holes and emptiness everywhere.";

        public static LocString E_NAME = "Emptera";
        public static LocString E_DESCRIPTION = "Emptera is a difficult empty asteroid. Resources are limited and you will have to take advantage of your geysers to get them.\n\n<smallcaps> Colonizing Emptera will be a difficult experience. However, this asteroid has some advantages with its vacuum. </smallcaps>\n\n";
        public static LocString I_NAME = "Islands";
        public static LocString I_DESCRIPTION = "Islands is an asteroid composed of asteroids. The resources are scattered and you will have to go through space to colonize the place.\n\n<smallcaps> Colonizing Islands will be an original and fun experience. This will also allow you to easily access the vacuum for some setups</smallcaps>\n\n";

        public static void OnLoad(string modPath)
        {
            WorldsPatches.modPath = modPath;
        }

        public static void Prefix()
        {
            Utilities.AddWorldYaml("Asteroid_Emptera", typeof(WorldsPatches));
            Utilities.AddWorldYaml("Asteroid_Islands", typeof(WorldsPatches));
        }
    }


    [HarmonyPatch(typeof(Localization))]
    [HarmonyPatch("Initialize")]
    class LocalizationPatch
    {
        public static void Postfix()
        {
            Utilities.LoadTranslations(typeof(WorldsPatches), WorldsPatches.modPath);
        }
    }



    [HarmonyPatch(typeof(TemplateCache))]
    [HarmonyPatch("CollectBaseTemplateAssets")]
    public class TemplateCache_CollectBaseTemplateAssetsPatch
    {
        public static void Postfix(ref List<TemplateContainer> __result)
        {
            if (OnLoadPatch.Settings.RemoveBunkerAtWorldgen)
            {
                TemplateContainer toRemove = null;
                foreach (TemplateContainer item in __result)
                {
                    if (item.name == "poi_bunker_skyblock") toRemove = item;
                }
                if (toRemove != null) __result.Remove(toRemove);
            }
        }
    }

}
