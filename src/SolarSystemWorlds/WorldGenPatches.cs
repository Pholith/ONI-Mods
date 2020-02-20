using Harmony;
using System;
using System.Collections.Generic;
using static ProcGen.Temperature;

namespace SolarSystemWorlds
{
    public class WorldGenPatches
    {

        public class ExtremelyCold2_patch
        {
            public static Dictionary<Range, string> TemperatureTable = new Dictionary<Range, string>();
            public static Dictionary<string, object> TemperatureReverseTable = new Dictionary<string, object>();

            private static void AddHashToTable(Range hash, string id)
            {
                TemperatureTable.Add(hash, id);
                TemperatureReverseTable.Add(id, hash);
            }

            public static void OnLoad()
            {                       // v this number must be unique
                AddHashToTable((Range)222, "ExtremelyCold2");
            }

            [HarmonyPatch(typeof(Enum), nameof(Enum.ToString), new Type[] { })]
            public static class Temperatures_ToString_Patch
            {
                public static bool Prefix(ref Enum __instance, ref string __result)
                {
                    if (!(__instance is Range)) return true;
                    return !TemperatureTable.TryGetValue((Range)__instance, out __result);
                }
            }

            [HarmonyPatch(typeof(Enum), nameof(Enum.Parse), new Type[] { typeof(Type), typeof(string), typeof(bool) })]
            public static class Temperatures_Parse_Patch
            {
                public static bool Prefix(Type enumType, string value, ref object __result)
                {
                    if (!enumType.Equals(typeof(Range))) return true;
                    return !TemperatureReverseTable.TryGetValue(value, out __result);
                }
            }
        }
    }
}
