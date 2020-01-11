using Harmony;
using Pholib;

namespace EmptyWorlds
{
    [HarmonyPatch(typeof(Db), "Initialize")]
    class WorldsPatches
    {
        public static string modPath;

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
            Utilities.AddWorldYaml(E_NAME, E_DESCRIPTION, "Asteroid_Emptera", typeof(WorldsPatches));
            Utilities.AddWorldYaml(I_NAME, I_DESCRIPTION, "Asteroid_Islands", typeof(WorldsPatches));
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

}
