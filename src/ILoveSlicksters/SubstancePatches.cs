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

        [HarmonyPatch(typeof(Enum), "ToString", new Type[] { })]
        public static class SimHashes_ToString_Patch
        {
            public static Dictionary<SimHashes, string> SimHashNameLookup = new Dictionary<SimHashes, string>
            {
                { Ethylen_Glycol.SimHash, Ethylen_Glycol.Id },
            };

            public static bool Prefix(ref Enum __instance, ref string __result)
            {
                if (!(__instance is SimHashes)) return true;
                return !SimHashNameLookup.TryGetValue((SimHashes)__instance, out __result);
            }
        }


        [HarmonyPatch(typeof(ElementLoader), "CollectElementsFromYAML")]
        public static class ElementLoader_CollectElementsFromYAML_Patch
        {
            public static void Postfix(ref List<ElementLoader.ElementEntry> __result)
            {
                Strings.Add($"STRINGS.ELEMENTS.{ToUpperSnakeCase(Ethylen_Glycol.Id)}.NAME", Ethylen_Glycol.Name);

                __result.AddRange(YamlIO.Parse<ElementLoader.ElementEntryCollection>(Ethylen_Glycol.Data, null).elements);
            }
        }
        [HarmonyPatch(typeof(ElementLoader), "Load")]
        public class ElementLoader_Load
        {
            public static void Prefix(ref Hashtable substanceList, SubstanceTable substanceTable)
            {
                var water = substanceTable.GetSubstance(SimHashes.Water);
                substanceList[Ethylen_Glycol.SimHash] = Ethylen_Glycol.CreateSubstance(water);
                Debug.Log("substance here");
            }
        }

    }
}
