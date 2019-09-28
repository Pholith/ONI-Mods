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