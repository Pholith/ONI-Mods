using Harmony;
using Pholib;
using UnityEngine;

namespace WorldgenPack
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
        private const int sizeScale = 7;
        private static KAnimFile[] originalAnim = null;
        private static float normalSize = 0;

        public static void Postfix()
        {
            if (Utilities.IsOnWorld(WorldAdds.G_NAME))
            {
                // Patch the moon
                if (EarthConfigPatch.earthAnimController != null)
                {
                    // save original anim
                    originalAnim = EarthConfigPatch.earthAnimController.AnimFiles;
                    // replace the anim
                    EarthConfigPatch.earthAnimController.AnimFiles = new KAnimFile[]
                    {
                        Assets.GetAnim("saturn_kanim")
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
                if (EarthConfigPatch.earthAnimController.AnimFiles[0] == Assets.GetAnim("saturn_kanim"))
                {
                    // reset the moon
                    Debug.Assert(originalAnim != null, "Original anim should not be null.");
                    EarthConfigPatch.earthAnimController.AnimFiles = originalAnim;
                    EarthConfigPatch.earthAnimController.animScale = EarthConfigPatch.earthAnimController.animScale / sizeScale;
                }
            }
        }
    }

    [HarmonyPatch(typeof(Db))]
    [HarmonyPatch("Initialize")]
    public class WorldAdds
    {
        public static LocString A_NAME = "Aquaria";
        public static LocString A_DESCRIPTION = "Test \n\n";

        public static LocString G_NAME = "Ganymede";
        public static LocString G_DESCRIPTION = "Ganymede is the 2nd moon of Jupiter, and the largest moon in the entire solar system. It contains a lot of water under its surface.\n\nGanymede will be the most difficult experience you have ever had, to help you in your planetary conquest, you have your habitable rocket that will provide you with valuable resources\n";
        
        public static LocString T_NAME = "Titan";
        public static LocString T_DESCRIPTION = "\n";


        public static void Postfix()
        {
            Utilities.AddWorldYaml(A_NAME, A_DESCRIPTION, null, typeof(WorldAdds));
            Utilities.AddWorldYaml(G_NAME, G_DESCRIPTION, "Asteroid_Ganymede", typeof(WorldAdds));
            Utilities.AddWorldYaml(T_NAME, T_DESCRIPTION, "Asteroid_Titan", typeof(WorldAdds));
        }
    }
}