using Harmony;
using Klei;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ILoveSlicksters
{
    class SubstancePatches
    {

        private static string ToUpperSnakeCase(string camelCase)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < camelCase.Length; i++)
            {
                if (i > 0 && char.IsUpper(camelCase[i]))
                {
                    builder.Append("_" + camelCase[i].ToString());
                }
                else
                {
                    builder.Append(camelCase[i].ToString());
                }
            }
            return builder.ToString().ToUpperInvariant();
        }

        public static Dictionary<SimHashes, string> SimHashTable = new Dictionary<SimHashes, string>();
        public static Dictionary<string, object> SimHashReverseTable = new Dictionary<string, object>();

        private static void AddHashToTable(SimHashes hash, string id)
        {
            SimHashTable.Add(hash, id);
            SimHashReverseTable.Add(id, hash);
        }

        public static void OnLoad()
        {
            AddHashToTable(Antigel.SimHash, Antigel.Id);
        }

        [HarmonyPatch(typeof(Enum), nameof(Enum.ToString), new Type[] { })]
        public static class SimHashes_ToString_Patch
        {
            public static bool Prefix(ref Enum __instance, ref string __result)
            {
                if (!(__instance is SimHashes)) return true;
                return !SimHashTable.TryGetValue((SimHashes)__instance, out __result);
            }
        }

        [HarmonyPatch(typeof(Enum), nameof(Enum.Parse), new Type[] { typeof(Type), typeof(string), typeof(bool) })]
        public static class SimHashes_Parse_Patch
        {
            public static bool Prefix(Type enumType, string value, ref object __result)
            {
                if (!enumType.Equals(typeof(SimHashes))) return true;
                return !SimHashReverseTable.TryGetValue(value, out __result);
            }
        }

        [HarmonyPatch(typeof(ElementLoader), "CollectElementsFromYAML")]
        public static class ElementLoader_CollectElementsFromYAML_Patch
        {
            public static void Postfix(ref List<ElementLoader.ElementEntry> __result)
            {
                Strings.Add($"STRINGS.ELEMENTS.{ToUpperSnakeCase(Antigel.Id)}.NAME", Antigel.Name);
                Strings.Add($"STRINGS.ELEMENTS.{ToUpperSnakeCase(Antigel.Id)}.DESC", Antigel.Description);

                __result.AddRange(YamlIO.Parse<ElementLoader.ElementEntryCollection>(Antigel.Data, null).elements);
            }
        }
        [HarmonyPatch(typeof(ElementLoader), "Load")]
        public class ElementLoader_Load
        {
            public static void Prefix(ref Hashtable substanceList, SubstanceTable substanceTable)
            {
                var water = substanceTable.GetSubstance(SimHashes.Water);
                
                substanceList[Antigel.SimHash] = Antigel.CreateSubstance(water);
            }
        }

    }
}
