using Database;
using Harmony;
using Klei.AI;
using Pholib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using TUNING;
using UnityEngine;

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
            var custom4 = config.GetProperty<int>("pointsWhenMoreThan3Interest", 0);

            var customAttributes = new int[] { custom1, custom2, custom3, custom4, custom4, custom4, custom4, custom4, custom4, custom4 , custom4};
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
    [HarmonyPatch(typeof(MinionStartingStats))]
    [HarmonyPatch("GenerateTraits")]
    public class TraitPatch
    {
        public static bool Prefix(MinionStartingStats __instance, List<ChoreGroup> disabled_chore_groups)
        {
            DUPLICANTSTATS.MAX_TRAITS = 10;

            List<string> selectedTraits = new List<string>();
            System.Random randSeed = new System.Random();
            Trait trait = Db.Get().traits.Get(__instance.personality.stresstrait);
            __instance.stressTrait = trait;
            Trait trait2 = Db.Get().traits.Get(__instance.personality.congenitaltrait);
            if (trait2.Name == "None")
            {
                __instance.congenitaltrait = null;
            }
            else
            {
                __instance.congenitaltrait = trait2;
            }
            Func<List<DUPLICANTSTATS.TraitVal>, bool> func = delegate (List<DUPLICANTSTATS.TraitVal> traitPossibilities)
            {
                if (__instance.Traits.Count > DUPLICANTSTATS.MAX_TRAITS)
                {
                    return false;
                }
                float num2 = Util.GaussianRandom(0f, 1f);
                List<DUPLICANTSTATS.TraitVal> list = new List<DUPLICANTSTATS.TraitVal>(traitPossibilities);
                list.ShuffleSeeded(randSeed);
                list.Sort((DUPLICANTSTATS.TraitVal t1, DUPLICANTSTATS.TraitVal t2) => -t1.probability.CompareTo(t2.probability));
                foreach (DUPLICANTSTATS.TraitVal traitVal in list)
                {
                    if (!selectedTraits.Contains(traitVal.id))
                    {
                        if (traitVal.requiredNonPositiveAptitudes != null)
                        {
                            bool flag2 = false;
                            foreach (KeyValuePair<SkillGroup, float> keyValuePair in __instance.skillAptitudes)
                            {
                                if (flag2)
                                {
                                    break;
                                }
                                foreach (HashedString x in traitVal.requiredNonPositiveAptitudes)
                                {
                                    if (x == keyValuePair.Key.IdHash && keyValuePair.Value > 0f)
                                    {
                                        flag2 = true;
                                        break;
                                    }
                                }
                            }
                            if (flag2)
                            {
                                continue;
                            }
                        }
                        if (traitVal.mutuallyExclusiveTraits != null)
                        {
                            bool flag3 = false;
                            foreach (string item in selectedTraits)
                            {
                                flag3 = traitVal.mutuallyExclusiveTraits.Contains(item);
                                if (flag3)
                                {
                                    break;
                                }
                            }
                            if (flag3)
                            {
                                continue;
                            }
                        }
                        if (num2 > traitVal.probability)
                        {
                            Trait trait3 = Db.Get().traits.TryGet(traitVal.id);
                            if (trait3 == null)
                            {
                                global::Debug.LogWarning("Trying to add nonexistent trait: " + traitVal.id);
                            }
                            else if (trait3.ValidStarterTrait)
                            {
                                selectedTraits.Add(traitVal.id);
                                __instance.Traits.Add(trait3);
                                if (trait3.disabledChoreGroups != null)
                                {
                                    for (int k = 0; k < trait3.disabledChoreGroups.Length; k++)
                                    {
                                        disabled_chore_groups.Add(trait3.disabledChoreGroups[k]);
                                    }
                                }
                                return true;
                            }

                        }
                    }
                }
                return false;
            };


            int numberOfGoodTraits = TuningConfigPatch.config.GetProperty<int>("numberOfGoodTraits", 1);
            int numberOfBadTraits = TuningConfigPatch.config.GetProperty<int>("numberOfBadTraits", 1);

            if (numberOfGoodTraits > 5)
            {
                numberOfGoodTraits = 5;
            }
            if (numberOfBadTraits > 5)
            {
                numberOfBadTraits = 5;
            }

            for (int i = 0; i < numberOfBadTraits; i++)
            {
                bool isTraitAdded = false;
                while (!isTraitAdded)
                {
                    isTraitAdded = func(DUPLICANTSTATS.BADTRAITS);
                }
            }
            for (int i = 0; i < numberOfGoodTraits; i++)
            {
                bool isTraitAdded = false;
                while (!isTraitAdded)
                {
                    isTraitAdded = func(DUPLICANTSTATS.GOODTRAITS);
                }
            }
            return false;

        }
    }
    [HarmonyPatch(typeof(HeadquartersConfig))]
    [HarmonyPatch("ConfigureBuildingTemplate")]
    public class SkillPointPatch
    {
        public static void Postfix(GameObject go)
        {
            int startingLevel = TuningConfigPatch.config.GetProperty<int>("startingLevelOnPrintingPod", 1);

            Telepad telepad = go.AddOrGet<Telepad>();
            telepad.startingSkillPoints = startingLevel;

            Logs.Log("test lib 2");

        }
    }

}