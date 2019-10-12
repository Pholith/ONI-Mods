using Harmony;
using Pholib;
using System;
using System.Reflection;
using UnityEngine;

namespace WorldgenPack
{
    [HarmonyPatch(typeof(TerrainBG))]
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
    }
    [HarmonyPatch(typeof(BackgroundEarthConfig))]
    [HarmonyPatch("CreatePrefab")]
    class BgEarthConfig_CreatePrefab_Patch
    {
        public static KBatchedAnimController earthAnimController;
        public static void Postfix(GameObject __result)
        {
            earthAnimController = __result.AddOrGet<KBatchedAnimController>();

        }
    }


    [HarmonyPatch(typeof(Game))]
    [HarmonyPatch("Load")]
    class AfterGameLoad_Patch
    {
        public static void Postfix()
        {
            if (Utilities.IsOnWorld(WorldAdds.G_NAME))
            {
                // Patch the earth
                if (BgEarthConfig_CreatePrefab_Patch.earthAnimController != null)
                {
                    BgEarthConfig_CreatePrefab_Patch.earthAnimController.AnimFiles = new KAnimFile[]
                    {
                        Assets.GetAnim("saturn_kanim")
                    };
                    BgEarthConfig_CreatePrefab_Patch.earthAnimController.animScale = BgEarthConfig_CreatePrefab_Patch.earthAnimController.animScale * 3;
                }
            }
        }
    }


    public class WorldAdds
    {
        public static LocString A_NAME = "Aquaria";
        public static LocString A_DESCRIPTION = "Test \n\n";

        public static LocString G_NAME = "Ganymede";
        public static LocString G_DESCRIPTION = "Test \n\n";

        public static void OnLoad()
        {
            Logs.DebugLog = true;
            Utilities.addWorldYaml(A_NAME, A_DESCRIPTION, null, typeof(WorldAdds));
            Utilities.addWorldYaml(G_NAME, G_DESCRIPTION, null, typeof(WorldAdds));

            try
            {
                /*foreach (var item in Assets.Textures)
                {
                    Debug.Log("here " + item.name);
                }*/

            }
            catch (System.Exception e)
            {
                Debug.Log(e.ToString());
                throw;
            }

        }
    }
}