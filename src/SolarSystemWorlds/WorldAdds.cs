using Harmony;
using Pholib;
using UnityEngine;

namespace SolarSystemWorlds
{
    /*[HarmonyPatch(typeof(TerrainBG))]
    [HarmonyPatch("OnSpawn")]
    class Test
    {
        private static string spriteName = "space_00";

        public static void Postfix(TerrainBG __instance)
        {
            if (Utilities.IsOnWorld(WorldAdds.G_NAME))
            {
                __instance.starsMaterial.SetTexture("_Tex0", Utilities.CreateTextureDXT5(Assembly.GetExecutingAssembly().GetManifestResourceStream("SolarSystemWorlds" + "." + spriteName + ".dds"), 1024, 1024));
            }
        }
    }*/

    [HarmonyPatch(typeof(BackgroundEarthConfig))]
    [HarmonyPatch("CreatePrefab")]
    class EarthConfigPatch
    {
        public static KBatchedAnimController earthAnimController;
        public static void Postfix(GameObject __result)
        {
            earthAnimController = __result.AddOrGet<KBatchedAnimController>();
        }
    }


    [HarmonyPatch(typeof(Game))]
    [HarmonyPatch("OnSpawn")]
    public class AfterGameLoad_Patch
    {
        private static KAnimFile getWorldAnim()
        {
            if (Utilities.IsOnWorld(WorldAdds.TitanId)) return Assets.GetAnim("saturn_kanim");
            if (Utilities.IsOnWorld(WorldAdds.GanymedeId)) return Assets.GetAnim("jupiter_kanim");
            if (Utilities.IsOnWorld(WorldAdds.EarthId)) return Assets.GetAnim("moon_kanim");
            return null;
        }

        private const int sizeScale = 7;
        private static KAnimFile[] originalAnim = null;
        private static float normalSize = 0;


        // incomprehensible code but... it works
        public static void Postfix()
        {
            if (Utilities.IsOnWorld(WorldAdds.GanymedeId) || Utilities.IsOnWorld(WorldAdds.TitanId) || Utilities.IsOnWorld(WorldAdds.EarthId))
            {
                // Patch the moon
                if (EarthConfigPatch.earthAnimController != null)
                {
                    // save original anim
                    originalAnim = EarthConfigPatch.earthAnimController.AnimFiles;
                    // replace the anim
                    EarthConfigPatch.earthAnimController.AnimFiles = new KAnimFile[]
                    {
                        getWorldAnim()
                    };
                    if (normalSize == 0 || EarthConfigPatch.earthAnimController.animScale < normalSize)
                    {
                        normalSize = EarthConfigPatch.earthAnimController.animScale;
                        EarthConfigPatch.earthAnimController.animScale = EarthConfigPatch.earthAnimController.animScale * sizeScale;
                    }
                }
            }
            else
            {
                // if someone load a other game from a Ganymede game -> reset changes
                if (EarthConfigPatch.earthAnimController.AnimFiles[0] == Assets.GetAnim("jupiter_kanim") || 
                    EarthConfigPatch.earthAnimController.AnimFiles[0] == Assets.GetAnim("saturn_kanim") ||
                    EarthConfigPatch.earthAnimController.AnimFiles[0] == Assets.GetAnim("moon_kanim"))
                {
                    // reset the moon
                    Debug.Assert(originalAnim != null, "Original anim should not be null.");
                    EarthConfigPatch.earthAnimController.AnimFiles = originalAnim;
                    EarthConfigPatch.earthAnimController.animScale = EarthConfigPatch.earthAnimController.animScale / sizeScale;
                }
            }
        }
    }


    [HarmonyPatch(typeof(Localization))]
    [HarmonyPatch("Initialize")]
    class LocalizationPatch
    {
        public static string modPath;

        public static void OnLoad(string modPath)
        {
            LocalizationPatch.modPath = modPath;
        }
        public static void Postfix()
        {
            Utilities.LoadTranslations(typeof(WorldAdds), modPath);
        }
    }
    
    [HarmonyPatch(typeof(Db))]
    [HarmonyPatch("Initialize")]
    public class WorldAdds
    {

        public static LocString G_NAME = "Ganymede";
        public static LocString G_DESC= "Ganymede is a moon of Jupiter, the largest moon in the entire solar system. It contains a lot of water under its surface.\n\nGanymede will be a difficult experience, to help you in your planetary conquest, you have your habitable rocket that will provide you with valuable resources\n\n";

        public static LocString T_NAME = "Titan";
        public static LocString T_DESC= "Titan is one of Saturn's moons, the second largest moon in the solar system and the only planet other than Earth that has liquid oceans. Oceans... of methane\n\nTitan is an extremely cold planet, to help you in your planetary conquest, you have your habitable rocket that will provide you with valuable resources\n\n";


        public static LocString E_NAME = "Earth";
        public static LocString E_DESC = "The Earth is the cradle of humanity, it is where the distant ancestors of the duplicants lived, they should have a little trouble finding their place here.\n\n";

        public static LocString M_NAME = "Moon";
        public static LocString M_DESC = "";


        public static LocString IRON_CORE_NAME = "Iron Core";
        public static LocString IRON_CORE_DESC = "This world has a core of liquid iron";


        // strings that will not be override by local translation
        public static string GanymedeId = G_NAME;
        public static string TitanId = T_NAME;
        public static string EarthId = E_NAME;
        public static string MoonId = M_NAME;


        public static void Postfix()
        {
            Utilities.AddWorldYaml(G_NAME, G_DESC, "Asteroid_Ganymede", typeof(WorldAdds));
            Utilities.AddWorldYaml(T_NAME, T_DESC, "Asteroid_Titan", typeof(WorldAdds));
            Utilities.AddWorldYaml(E_NAME, E_DESC, "Asteroid_Earth", typeof(WorldAdds));
        }
    }
}