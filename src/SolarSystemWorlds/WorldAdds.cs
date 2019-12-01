using Harmony;
using PeterHan.PLib;
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

                GameObject g = UnityEngine.Object.Instantiate(EarthConfigPatch.earthAnimController.gameObject);
                g.transform.position = new Vector2(g.transform.position.x / 2, g.transform.position.y);
                Component c = g.GetComponent<KBatchedAnimController>();
                Logs.Log("c = " + c);
                Logs.Log("g = " + g);
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
            } else 
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


    public class WorldAdds
    {
        public static LocString A_NAME = "Aquaria";
        public static LocString A_DESCRIPTION = "Test \n\n";

        public static LocString G_NAME = "Ganymede";
        public static LocString G_DESCRIPTION = "Test \n\n";

        public static void OnLoad()
        {
            PPatchTools.LogAllFailedAsserts();

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