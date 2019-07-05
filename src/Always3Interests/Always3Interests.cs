﻿using Database;
using Harmony;
using Klei.AI;
using Pholib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using TUNING;
using UnityEngine;
using static TUNING.DUPLICANTSTATS;

namespace Always3Interests
{
    
    [HarmonyPatch(typeof(Db), "Initialize")]
    class TuningConfigPatch
    {
        public static JsonReader config;

        public static void Prefix()
        {

            config = new JsonReader();

            var custom1 = config.GetProperty<int>("pointsWhen1Interest", 7);
            var custom2 = config.GetProperty<int>("pointsWhen2Interest", 3);
            var custom3 = config.GetProperty<int>("pointsWhen3Interest", 1);

            var customAttributes = new int[] { custom1, custom2, custom3 };
            Traverse.Create<DUPLICANTSTATS>().Field("APTITUDE_ATTRIBUTE_BONUSES").SetValue(customAttributes);
        }
    }

    [HarmonyPatch(typeof(MinionStartingStats), "GenerateAptitudes")]
    public class GenerateAptitudesPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            Logs.InitIfNot();
            var codes = new List<CodeInstruction>(instructions);
            
            JsonReader config = new JsonReader();

            var numberOfInterests = config.GetProperty<int>("numberOfInterests", 3);
            bool randomInterests = config.GetProperty<bool>("randomNumberOfInterests", false);

            if (!randomInterests)
            {
                for (int i = 0; i < 5; i++)
                {
                    if (codes[i].opcode == OpCodes.Ldc_I4_1)
                    {
                        codes[i].opcode = OpCodes.Ldc_I4;
                        codes[i].operand = numberOfInterests;
                    }
                    if (codes[i].opcode == OpCodes.Ldc_I4_4)
                    {
                        codes[i].opcode = OpCodes.Ldc_I4;
                        codes[i].operand = numberOfInterests;
                    }
                    //Debug.Log("test= " + codes[i].ToString());
                }
            }
            return codes.AsEnumerable();
        }
    }

    [HarmonyPatch(typeof(MinionStartingStats), "ApplyTraits")]
    public class ApplyPatch
    {
        public static void Postfix(ref MinionStartingStats __instance, GameObject go)
        {
            JsonReader config = new JsonReader();

            int numberOfGoodTraits = config.GetProperty<int>("numberOfGoodTraits", 1);
            int numberOfBadTraits = config.GetProperty<int>("numberOfBadTraits", 1);

            Traits component = go.GetComponent<Traits>();
            component.Clear();

            if (numberOfBadTraits <= 0)
            {
                foreach (var trait in __instance.Traits)
                {
                    if (trait.PositiveTrait)
                    {
                        component.Add(trait);
                    }
                }
            }
            else if (numberOfGoodTraits <= 0)
            {
                foreach (var trait in __instance.Traits)
                {
                    if (!trait.PositiveTrait)
                    {
                        component.Add(trait);
                    }
                }
            } else
            {
                foreach (var trait in __instance.Traits)
                {
                    component.Add(trait);
                }
            }

            
            for (int i = 0; i < numberOfGoodTraits - 1; i++)
            {
                string id = DUPLICANTSTATS.GOODTRAITS.GetRandom<TraitVal>().id;
                component.Add(Db.Get().traits.TryGet(id));
            }


            for (int i = 0; i < numberOfBadTraits - 1; i++)
            {
                string id = DUPLICANTSTATS.BADTRAITS.GetRandom<TraitVal>().id;
                component.Add(Db.Get().traits.TryGet(id));
            }

        }
    }
}