using Harmony;
using Pholib;
using System;
using System.Reflection;
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
    class BgEarthConfig_Prefab_Patch
    {
        public static KBatchedAnimController earthAnimController;
        public static void Postfix(GameObject __result)
        {
            earthAnimController = __result.AddOrGet<KBatchedAnimController>();
        }
    }


    [HarmonyPatch(typeof(Game))]
    [HarmonyPatch("Load")]
    public class AfterGameLoad_Patch
    {
        private const int sizeScale = 6;
        private static KAnimFile[] originalAnim = null;

        public static void Postfix()
        {
            if (Utilities.IsOnWorld(WorldAdds.G_NAME))
            {

                GameObject g = GameObject.Instantiate(BgEarthConfig_Prefab_Patch.earthAnimController.gameObject);
                g.transform.position = new Vector2(g.transform.position.x / 2, g.transform.position.y);
                Component c = g.GetComponent<KBatchedAnimController>();
                Logs.Log("c = " + c);
                Logs.Log("g = " + g);
                // Patch the moon
                if (BgEarthConfig_Prefab_Patch.earthAnimController != null)
                {
                    // save original anim
                    originalAnim = BgEarthConfig_Prefab_Patch.earthAnimController.AnimFiles;
                    // replace the anim
                    BgEarthConfig_Prefab_Patch.earthAnimController.AnimFiles = new KAnimFile[]
                    {
                        Assets.GetAnim("saturn_kanim")
                    };
                    BgEarthConfig_Prefab_Patch.earthAnimController.animScale = BgEarthConfig_Prefab_Patch.earthAnimController.animScale * sizeScale;
                }
            } else 
            {
                // if someone load a other game from a Ganymede game -> reset changes
                if (BgEarthConfig_Prefab_Patch.earthAnimController.AnimFiles[0] == Assets.GetAnim("saturn_kanim"))
                {
                    // reset the moon
                    Debug.Assert(originalAnim != null, "Original anim should not be null.");
                    BgEarthConfig_Prefab_Patch.earthAnimController.AnimFiles = originalAnim;
                    BgEarthConfig_Prefab_Patch.earthAnimController.animScale = BgEarthConfig_Prefab_Patch.earthAnimController.animScale / sizeScale;
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