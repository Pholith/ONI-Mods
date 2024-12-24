using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using static Pholib.Utilities;

namespace DuplicantRoomSensor
{
    [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
    public static class DupRoomSensorStringsPatch
    {
        public static void Prefix()
        {
            AddBuilding("Automation", DuplicantRoomSensorConfig.ID, DuplicantRoomSensorConfig.NAME, DuplicantRoomSensorConfig.DESC, DuplicantRoomSensorConfig.EFFECT);
        }
    }

    [HarmonyPatch(typeof(Db), "Initialize")]
    public static class DupRoomSensorTechPatch
    {
        public static void Postfix()
        {
            AddBuildingTech("LogicControl", DuplicantRoomSensorConfig.ID);
        }
    }

    public static class CavityInfoDuplicants
    {
        public static Dictionary<CavityInfo, List<KPrefabID>> map = new Dictionary<CavityInfo, List<KPrefabID>>();
    }

    [HarmonyPatch(typeof(BaseMinionConfig))]
    [HarmonyPatch("BaseMinion")]
    public static class DupRoomSensor_BaseMinionConfigPatch
    {
        public static void Postfix(GameObject __result)
        {
            __result.AddOrGetDef<DuplicantMonitor.Def>();
        }
    }
}
