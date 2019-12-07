using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Pholib.Utilities;

namespace SoundAlarm
{
    [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
    public static class DupRoomSensorStringsPatch
    {
        public static void Prefix()
        {
            AddBuilding("Automation", SoundAlarmConfig.ID, SoundAlarmConfig.NAME, SoundAlarmConfig.DESC, SoundAlarmConfig.EFFECT);
        }
    }

    [HarmonyPatch(typeof(Db), "Initialize")]
    public static class DupRoomSensorTechPatch
    {
        public static void Prefix()
        {
            AddBuildingTech("LogicControl", SoundAlarmConfig.ID);
        }
    }

}
