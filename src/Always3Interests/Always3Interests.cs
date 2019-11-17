using Database;
using Harmony;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using TUNING;
using UnityEngine;
using PeterHan.PLib.Options;
using Newtonsoft.Json;
using PeterHan.PLib;

namespace Always3Interests
{
    public class Always3InterestsSettings
    {
        [Option("Number of interests", "The number of interests.")]
        [Limit(0, 10)]
        [JsonProperty]
        public int numberOfInterests { get; set; }

        [Option("Random number of interests", "Active it to disable the interest modification.")]
        [JsonProperty]
        public bool randomNumberOfInterests { get; set; }


        [Option("Points when 1 interest", "")]
        [Limit(0, 50)]
        [JsonProperty]
        public int pointsWhen1Interest { get; set; }

        [Option("Points when 2 interest", "")]
        [Limit(0, 50)]
        [JsonProperty]
        public int pointsWhen2Interest { get; set; }

        [Option("Points when 3 interest", "")]
        [Limit(0, 50)]
        [JsonProperty]
        public int pointsWhen3Interest { get; set; }

        [Option("Points when more than 3 interest", "")]
        [Limit(0, 50)]
        [JsonProperty]
        public int pointsWhenMoreThan3Interest { get; set; }

        [Option("Number of Good traits", "")]
        [Limit(0, 5)]
        [JsonProperty]
        public int numberOfGoodTraits { get; set; }

        [Option("Number of Bad traits", "")]
        [Limit(0, 5)]
        [JsonProperty]
        public int numberOfBadTraits { get; set; }

        [Option("Starting level on printing pod", "Set the experience of in game printed dups.")]
        [Limit(0, 5)]
        [JsonProperty]
        public int startingLevelOnPrintingPod { get; set; }

        

        public Always3InterestsSettings()
        {
            pointsWhen1Interest = 7;
            pointsWhen2Interest = 3;
            pointsWhen3Interest = 1;

            pointsWhenMoreThan3Interest = 1;

            numberOfInterests = 3;
            randomNumberOfInterests = false;

            numberOfGoodTraits = 1;
            numberOfBadTraits = 1;

            startingLevelOnPrintingPod = 1;
        }
    }

    class TuningConfigPatch
    {

        public static Always3InterestsSettings settings;

        public static void OnLoad()
        {
            PUtil.InitLibrary();
            POptions.RegisterOptions(typeof(Always3InterestsSettings));

            settings = POptions.ReadSettings<Always3InterestsSettings>();
            if (settings == null)
            {
                settings = new Always3InterestsSettings();
            }

            var customAttributes = new int[] {
                settings.pointsWhen1Interest ,
                settings.pointsWhen2Interest,
                settings.pointsWhen3Interest,
                settings.pointsWhenMoreThan3Interest,
                settings.pointsWhenMoreThan3Interest, 
                settings.pointsWhenMoreThan3Interest, 
                settings.pointsWhenMoreThan3Interest, 
                settings.pointsWhenMoreThan3Interest, 
                settings.pointsWhenMoreThan3Interest, 
                settings.pointsWhenMoreThan3Interest,
                settings.pointsWhenMoreThan3Interest
            };
            Traverse.Create<DUPLICANTSTATS>().Field("APTITUDE_ATTRIBUTE_BONUSES").SetValue(customAttributes);
        }
    }

    [HarmonyPatch(typeof(MinionStartingStats), "GenerateAptitudes")]
    public class GenerateAptitudesPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            Always3InterestsSettings settings = POptions.ReadSettings<Always3InterestsSettings>();
            if (settings == null)
            {
                settings = new Always3InterestsSettings();
            }

            var numberOfInterests = settings.numberOfInterests;
            bool randomInterests = settings.randomNumberOfInterests;

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
            Trait joytrait = Db.Get().traits.Get(__instance.personality.joyTrait);
            __instance.joyTrait = joytrait;
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


            int numberOfGoodTraits = TuningConfigPatch.settings.numberOfGoodTraits; //TuningConfigPatch.config.GetProperty<int>("numberOfGoodTraits", 1);
            int numberOfBadTraits = TuningConfigPatch.settings.numberOfBadTraits; //TuningConfigPatch.config.GetProperty<int>("numberOfBadTraits", 1);

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
            int startingLevel = TuningConfigPatch.settings.startingLevelOnPrintingPod;// .GetProperty<int>("startingLevelOnPrintingPod", 1);

            Telepad telepad = go.AddOrGet<Telepad>();
            telepad.startingSkillPoints = startingLevel;

        }
    }
}