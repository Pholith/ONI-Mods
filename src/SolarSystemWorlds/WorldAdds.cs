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
            if (Utilities.IsOnWorld(WorldAdds.T_NAME)) return Assets.GetAnim("saturn_kanim");
            if (Utilities.IsOnWorld(WorldAdds.G_NAME)) return Assets.GetAnim("jupiter_kanim");
            return null;
        }

        private const int sizeScale = 7;
        private static KAnimFile[] originalAnim = null;
        private static float normalSize = 0;

        public static void Postfix()
        {
            if (Utilities.IsOnWorld(WorldAdds.G_NAME) || Utilities.IsOnWorld(WorldAdds.T_NAME))
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
                    EarthConfigPatch.earthAnimController.AnimFiles[0] == Assets.GetAnim("saturn_kanim"))
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
        public static LocString A_NAME = "Aquaria";
        public static LocString A_DESC= "Test \n\n";

        public static LocString G_NAME = "Ganymede";
        public static LocString G_DESC= "Ganymede is a moon of Jupiter, the largest moon in the entire solar system. It contains a lot of water under its surface.\n\nGanymede will be a difficult experience, to help you in your planetary conquest, you have your habitable rocket that will provide you with valuable resources\n\n";

        public static LocString E_NAME = "Earth";
        public static LocString E_DESC= "\n";

        public static LocString T_NAME = "Titan";
        public static LocString T_DESC= "Titan is one of Saturn's moons, the second largest moon in the solar system and the only planet other than Earth that has liquid oceans. Oceans... of methane\n\nTitan is an extremely cold planet, to help you in your planetary conquest, you have your habitable rocket that will provide you with valuable resources\n\n";


        public static void Postfix()
        {
            Utilities.AddWorldYaml(A_NAME, A_DESC, null, typeof(WorldAdds));
            Utilities.AddWorldYaml(G_NAME, G_DESC, "Asteroid_Ganymede", typeof(WorldAdds));
            Utilities.AddWorldYaml(T_NAME, T_DESC, "Asteroid_Titan", typeof(WorldAdds));
            Utilities.AddWorldYaml(E_NAME, E_DESC, "Asteroid_Earth", typeof(WorldAdds));
        }
    }
}