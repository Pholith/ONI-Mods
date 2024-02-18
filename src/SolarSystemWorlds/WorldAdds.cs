using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.UI;
using Pholib;
using ProcGenGame;
using System;
using System.IO;
using UnityEngine;

namespace SolarSystemWorlds
{
    public class SolarSystemWorld : UserMod2
    {

        public static string modPath;
        public static Sprite IronCoreTraitSprite;

        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            modPath = path;
            //Logs.DebugLog = true;
            WorldGenPatches.ExtremelyCold2_patch.OnLoad();

            IronCoreTraitSprite = PUIUtils.LoadSpriteFile(Path.Combine(Utilities.ModPath(), "IronCore.png"));
            if (IronCoreTraitSprite == null)
            {
                Logs.Error($"Sprite {Path.Combine(Utilities.ModPath(), "IronCore.png")} could not be loaded. Sprit will remain default.");
            }

        }
    }

    [HarmonyPatch(typeof(BackgroundEarthConfig))]
    [HarmonyPatch("CreatePrefab")]
    internal class EarthConfigPatch
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
    internal class ReportWorldGenError_patch
    {
        public static void Prefix(Exception e)
        {
            Debug.LogException(e);
        }
    }

    // Change the name of the planet on the space map
    [HarmonyPatch(typeof(AsteroidGridEntity), nameof(AsteroidGridEntity.Init))]
    public class AsteroidGridEntityPatch
    {
        private static void Postfix(AsteroidGridEntity __instance, string asteroidTypeId)
        {
            if (asteroidTypeId.Contains("planet"))
            {
                Traverse.Create(__instance).Field<string>("m_name").Value = asteroidTypeId.Replace("planet_", "").Replace("_kanim", "");
            }
        }
    }



    [HarmonyPatch(typeof(Game))]
    [HarmonyPatch("OnSpawn")]
    public class AfterGameLoad_Patch
    {
        private static KAnimFile GetWorldAnim()
        {
            if (Utilities.IsOnCluster(SOLAR_STRINGS.TitanId)) return Assets.GetAnim("saturn_kanim");
            if (Utilities.IsOnCluster(SOLAR_STRINGS.GanymedeId)) return Assets.GetAnim("jupiter_kanim");
            if (Utilities.IsOnCluster(SOLAR_STRINGS.IOId)) return Assets.GetAnim("jupiter_kanim");
            if (Utilities.IsOnCluster(SOLAR_STRINGS.EarthId)) return Assets.GetAnim("moon_kanim");
            if (Utilities.IsOnCluster(SOLAR_STRINGS.MoonId)) return Assets.GetAnim("earth2_kanim");
            return null;
        }

        private const int sizeScale = 5;
        private static KAnimFile[] originalAnim = null;
        private static float normalSize = 0;


        // incomprehensible code but... it works
        public static void Postfix()
        {
            if (Utilities.IsOnCluster(SOLAR_STRINGS.GanymedeId) ||
                Utilities.IsOnCluster(SOLAR_STRINGS.TitanId) ||
                Utilities.IsOnCluster(SOLAR_STRINGS.EarthId) ||
                Utilities.IsOnCluster(SOLAR_STRINGS.MoonId) ||
                Utilities.IsOnCluster(SOLAR_STRINGS.IOId))
            {
                // Patch the moon
                if (EarthConfigPatch.earthAnimController != null)
                {
                    // save original anim
                    if (originalAnim == null) originalAnim = new KAnimFile[] { Assets.GetAnim("earth_kanim") };
                    // replace the anim
                    EarthConfigPatch.earthAnimController.AnimFiles = new KAnimFile[]
                    {
                        GetWorldAnim()
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
        public static void Postfix()
        {
            Utilities.AddWorldYaml(typeof(SOLAR_STRINGS));
            Strings.Add(new string[]
            {
                "STRINGS.SUBWORLDS.EARTH.NAME", SOLAR_STRINGS.E_NAME
            });
            Strings.Add(new string[]
            {
                "STRINGS.SUBWORLDS.EARTH.DESC", SOLAR_STRINGS.E_DESC
            });
            Strings.Add(new string[]
            {
                "STRINGS.SUBWORLDS.EARTH.UTILITY", ""
            });
            Strings.Add(new string[]
            {
                "STRINGS.SUBWORLDS.GANYMEDE.NAME", SOLAR_STRINGS.G_NAME
            });
            Strings.Add(new string[]
            {
                "STRINGS.SUBWORLDS.GANYMEDE.DESC", SOLAR_STRINGS.G_DESC
            });
            Strings.Add(new string[]
            {
                "STRINGS.SUBWORLDS.GANYMEDE.UTILITY", ""
            });
            Strings.Add(new string[]
            {
                "STRINGS.SUBWORLDS.TITAN.NAME", SOLAR_STRINGS.T_NAME
            });
            Strings.Add(new string[]
            {
                "STRINGS.SUBWORLDS.TITAN.DESC", SOLAR_STRINGS.T_DESC
            });
            Strings.Add(new string[]
            {
                "STRINGS.SUBWORLDS.TITAN.UTILITY", ""
            });
            Strings.Add(new string[]
            {
                "STRINGS.SUBWORLDS.MOON.NAME", SOLAR_STRINGS.M_NAME
            });
            Strings.Add(new string[]
            {
                "STRINGS.SUBWORLDS.MOON.DESC", SOLAR_STRINGS.M_DESC
            });
            Strings.Add(new string[]
            {
                "STRINGS.SUBWORLDS.MOON.UTILITY", ""
            });

        }
    }

    [HarmonyPatch(typeof(Localization))]
    [HarmonyPatch("Initialize")]
    public static class Localization_Initialize_Patch
    {
        public static void Postfix()
        {
            Utilities.LoadTranslations(typeof(SOLAR_STRINGS), SolarSystemWorld.modPath);
            //LocString.CreateLocStringKeys(typeof(WALLPUMP_STRINGS.UI_ADD));
            Utilities.GenerateStringsTemplate(typeof(SOLAR_STRINGS));

        }
    }
}