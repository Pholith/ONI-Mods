using HarmonyLib;
using KMod;
using Pholib;
using ProcGen;
using ProcGenGame;
using System;
using UnityEngine;

namespace SolarSystemWorlds
{
    public class AllBiomesWorld : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            Logs.DebugLog = true;
            WorldGenPatches.ExtremelyCold2_patch.OnLoad();
            //PUtil.InitLibrary();
            //new PLocalization().Register();
        }
    }

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

    // Theses patches are used to improve worldgen error logs.
    [HarmonyPatch(typeof(WorldGen))]
    [HarmonyPatch("ReportWorldGenError")]
    class ReportWorldGenError_patch
    {
        public static void Prefix(Exception e)
        {
            Debug.LogException(e);
        }
    }
    [HarmonyPatch(typeof(WorldGen))]
    [HarmonyPatch("GenerateNoiseData")]
    class GenerateNoiseData_patch
    {
        public static void Prefix()
        {
            Debug.Log("Starting generate noise");
        }
    }    
    [HarmonyPatch(typeof(WorldGen))]
    [HarmonyPatch("GenerateLayout")]
    class GenerateLayout_patch
    {
        public static void Prefix()
        {
            Debug.Log("Starting generate layout");
        }
    }
    [HarmonyPatch(typeof(WorldGen))]
    [HarmonyPatch("RenderOffline")]
    class RenderOffline_patch
    {
        public static void Prefix()
        {
            Debug.Log("Starting render offline");
        }
    }


    [HarmonyPatch(typeof(Game))]
    [HarmonyPatch("OnSpawn")]
    public class AfterGameLoad_Patch
    {
        private static KAnimFile getWorldAnim()
        {
            if (Utilities.IsOnCluster(WorldAdds.TitanId)) return Assets.GetAnim("saturn_kanim");
            if (Utilities.IsOnCluster(WorldAdds.GanymedeId)) return Assets.GetAnim("jupiter_kanim");
            if (Utilities.IsOnCluster(WorldAdds.IOId)) return Assets.GetAnim("jupiter_kanim");
            if (Utilities.IsOnCluster(WorldAdds.EarthId)) return Assets.GetAnim("moon_kanim");
            if (Utilities.IsOnCluster(WorldAdds.MoonId)) return Assets.GetAnim("earth2_kanim");
            return null;
        }

        private const int sizeScale = 5;
        private static KAnimFile[] originalAnim = null;
        private static float normalSize = 0;


        // incomprehensible code but... it works
        public static void Postfix()
        {
            if (Utilities.IsOnCluster(WorldAdds.GanymedeId) || 
                Utilities.IsOnCluster(WorldAdds.TitanId) || 
                Utilities.IsOnCluster(WorldAdds.EarthId) ||
                Utilities.IsOnCluster(WorldAdds.MoonId) ||
                Utilities.IsOnCluster(WorldAdds.IOId))
            {
                // Patch the moon
                if (EarthConfigPatch.earthAnimController != null)
                {
                    // save original anim
                    if (originalAnim == null) originalAnim = new KAnimFile[] { Assets.GetAnim("earth_kanim") };
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
                // if someone load a non solar system game from one -> reset changes
                if (EarthConfigPatch.earthAnimController.AnimFiles[0] == Assets.GetAnim("jupiter_kanim") || 
                    EarthConfigPatch.earthAnimController.AnimFiles[0] == Assets.GetAnim("saturn_kanim") ||
                    EarthConfigPatch.earthAnimController.AnimFiles[0] == Assets.GetAnim("earth2_kanim") ||
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
        public static LocString M_DESC = "The Moon is the Earth's only natural satellite, probably the result of a collision 4.4 billion years ago between our fledgling planet and a small celestial body called Theia.\n\nThe lunar surface is barren and without resources, your duplicants will not survive without extreme preparation.\n\n";

        public static LocString I_NAME = "IO";
        public static LocString I_DESC = "\n\n";


        public static LocString IRON_CORE_NAME = "Iron Core";
        public static LocString IRON_CORE_DESC = "This world has a core of liquid iron";


        // strings that will not be override by local translation
        public static string GanymedeId = G_NAME;
        public static string TitanId = T_NAME;
        public static string EarthId = E_NAME;
        public static string MoonId = M_NAME;
        public static string IOId = I_NAME;


        public static void Postfix()
        {
            Utilities.AddWorldYaml(typeof(WorldAdds));
            Utilities.AddWorldYaml(typeof(WorldAdds));
            Utilities.AddWorldYaml(typeof(WorldAdds));
            Utilities.AddWorldYaml(typeof(WorldAdds));
        }
    }
}