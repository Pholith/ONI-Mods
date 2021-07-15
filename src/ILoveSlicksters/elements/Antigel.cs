using HarmonyLib;
using Pholib;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Pholib.Utilities;

namespace ILoveSlicksters
{
    public class Antigel
    {



        public const string Id = "Antigel";
        public static readonly SimHashes SimHash = (SimHashes)Hash.SDBMLower(Id);
        public static string Name = PHO_STRINGS.ELEMENTS.ANTIGEL.NAME;
        public static string Description = PHO_STRINGS.ELEMENTS.ANTIGEL.DESC;
        public static Color32 color = new Color32(190, 220, 160, 255);

        public static Substance CreateSubstance(Substance source)
        {
            return ModUtil.CreateSubstance(
              name: Id,
              state: Element.State.Liquid,
              kanim: source.anim,
              material: source.material,
              colour: color,
              ui_colour: color,
              conduit_colour: color
            );
        }

        public static void RegisterSubstance()
        {
            CreateRegisteredSubstance(
              name: Id,
              state: Element.State.Liquid,
              kanim: FindAnim("liquid_tank_kanim"),
              material: Assets.instance.substanceTable.liquidMaterial,
              colour: color
            );
        }

        [HarmonyPatch(typeof(Enum), "ToString", new Type[] { })]
        internal static class Patch_Enum_ToString
        {
            private static bool Prefix(ref Enum __instance, ref string __result)
            {
                if (__instance is SimHashes)
                {
                    return !SimHashUtil.SimHashNameLookup.ContainsKey((SimHashes)__instance);
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(Enum), "Parse", new Type[] { typeof(Type), typeof(string), typeof(bool) })]
        internal static class Patch_Enum_Parse
        {
            private static bool Prefix(Type enumType, string value, ref object __result)
            {
                if (enumType.Equals(typeof(SimHashes)))
                {
                    return !SimHashUtil.ReverseSimHashNameLookup.ContainsKey(value);
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(Enum), "GetValues", new Type[] { typeof(Type) })]
        private static class Patch_Enum_GetValues
        {
            private static void Postfix(Type enumType, ref Array __result)
            {
                if (enumType.Equals(typeof(SimHashes)))
                {
                    var res = new List<SimHashes>();
                    res.AddRange((SimHashes[])__result);
                    res.AddRange(SimHashUtil.SimHashNameLookup.Keys);
                    __result = res.ToArray();
                }
            }
        }


        /*[HarmonyPatch(typeof(ElementLoader), "Load")]
        private static class Patch_ElementLoader_Load
        {

            private static void Prefix(ref Hashtable substanceList, SubstanceTable substanceTable)
            {
                ElementManager.substanceList = substanceList;
                ElementManager.substanceTable = substanceTable;
            }

            private static void Postfix()
            {
                ElementManager.RegisterAttributes();
            }
        }*/


    }
}
