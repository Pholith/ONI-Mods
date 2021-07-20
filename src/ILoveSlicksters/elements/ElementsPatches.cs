﻿using HarmonyLib;
using Pholib;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ILoveSlicksters.elements
{
    //// Utils
    public class SubstanceUtils
    {
        internal static Hashtable substanceList;
        internal static Dictionary<string, SubstanceTable> substanceTablesByDlc;

        public static void AddSubstance(Substance substance)
        {
            Assets.instance.substanceTable.GetList().Add(substance);
        }

        public static Substance CreateSubstance(string name, Element.State state, KAnimFile kanim, Material material, Color32 colour)
        {
            return ModUtil.CreateSubstance(name, state, kanim, material, colour, colour, colour);
        }

        public static void RegisterElementStrings(string elementId, string name, string description)
        {
            string upperElemId = elementId.ToUpper();
            Strings.Add($"STRINGS.ELEMENTS.{upperElemId}.NAME", STRINGS.UI.FormatAsLink(name, upperElemId));
            Strings.Add($"STRINGS.ELEMENTS.{upperElemId}.DESC", description);
        }

        // Note: As of 2021-03-14
        // Needed for vanilla as it does not map anims into a dictionary until after elements have been loaded
        // Therefore don't use Assets.GetAnim here.
        public static KAnimFile FindAnim(string name)
        {
            KAnimFile result = Assets.Anims.Find((anim) => anim.name == name);
            if (result == null)
            {
                Debug.LogError($"Failed to find KAnim: {name}");
            }

            return result;
        }
        public static Substance CreateRegisteredSubstance(string name, Element.State state, KAnimFile kanim, Material material, Color32 colour)
        {
            Substance result = CreateSubstance(name, state, kanim, material, colour);
            SimHashUtil.RegisterSimHash(result.elementID, name);
            if (!substanceList.ContainsKey(result.elementID))
            { substanceList.Add(result.elementID, result); }
            return result;
        }
    }

    public static class SimHashUtil
    {
        public static Dictionary<SimHashes, string> SimHashNameLookup = new Dictionary<SimHashes, string>();
        public static readonly Dictionary<string, object> ReverseSimHashNameLookup = new Dictionary<string, object>();

        public static void RegisterSimHash(SimHashes hash, string name)
        {
            SimHashNameLookup.Add(hash, name);
            ReverseSimHashNameLookup.Add(name, hash);
        }
    }

    //// Patches
    [HarmonyPatch(typeof(Enum), nameof(Enum.GetValues), new Type[] { typeof(Type) })]
    internal static class Patch_Enum_GetValues
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

    [HarmonyPatch(typeof(ElementLoader), nameof(ElementLoader.Load))]
    internal static class Patch_ElementLoader_Load
    {
        private static void Prefix(ref Hashtable substanceList, Dictionary<string, SubstanceTable> substanceTablesByDlc)
        {
            SubstanceUtils.RegisterElementStrings(Antigel.Id, PHO_STRINGS.ELEMENTS.ANTIGEL.NAME, PHO_STRINGS.ELEMENTS.ANTIGEL.DESC);
            SubstanceUtils.substanceList = substanceList;
            SubstanceUtils.substanceTablesByDlc = substanceTablesByDlc;
            Antigel.RegisterSubstance();
        }
    }

    [HarmonyPatch(typeof(Enum), nameof(Enum.Parse), new Type[] { typeof(Type), typeof(string), typeof(bool) })]
    internal class SimHashes_Parse_Patch
    {
        private static bool Prefix(Type enumType, string value, ref object __result)
        {
            if (enumType.Equals(typeof(SimHashes)))
            {
                return !SimHashUtil.ReverseSimHashNameLookup.TryGetValue(value, out __result);
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Enum), nameof(ToString), new Type[] { })]
    internal class SimHashes_ToString_Patch
    {
        private static bool Prefix(ref Enum __instance, ref string __result)
        {
            if (__instance is SimHashes)
            {
                return !SimHashUtil.SimHashNameLookup.TryGetValue((SimHashes)__instance, out __result);
            }
            return true;
        }
    }


}
