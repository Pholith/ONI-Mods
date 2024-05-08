using Database;
using HarmonyLib;
using Klei.AI;
using KMod;
using Newtonsoft.Json;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

namespace Always3Interests
{
    [RestartRequired]
    [JsonObject(MemberSerialization.OptIn)]
    public class Always3InterestsSettings
    {
        [Option("Number of interests", "The number of interests.")]
        [Limit(0, 6)]
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

        [Option("Disable joy trait", "")]
        [JsonProperty]
        public bool disableJoyTrait { get; set; }

        [Option("Disable stress trait", "")]
        [JsonProperty]
        public bool disableStressTrait { get; set; }

        [Option("Starting level on printing pod", "Set the experience of in game printed dups.")]
        [Limit(0, 5)]
        [JsonProperty]
        public int startingLevelOnPrintingPod { get; set; }



        public Always3InterestsSettings()
        {
            pointsWhen1Interest = 9;
            pointsWhen2Interest = 5;
            pointsWhen3Interest = 1;

            pointsWhenMoreThan3Interest = 1;

            numberOfInterests = 3;
            randomNumberOfInterests = false;

            numberOfGoodTraits = 1;
            numberOfBadTraits = 1;
            disableJoyTrait = false;
            disableStressTrait = false;

            startingLevelOnPrintingPod = 1;
        }
    }

    public class Always3Interests : UserMod2
    {

        public static Always3InterestsSettings Settings;

        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            new POptions().RegisterOptions(this, typeof(Always3InterestsSettings));


            // Init PLib and settings
            PUtil.InitLibrary();

            Settings = POptions.ReadSettings<Always3InterestsSettings>();
            if (Settings == null)
            {
                Settings = new Always3InterestsSettings();
            }

            int[] customAttributes = new int[] {
                Settings.pointsWhen1Interest,
                Settings.pointsWhen2Interest,
                Settings.pointsWhen3Interest,
                Settings.pointsWhenMoreThan3Interest,
                Settings.pointsWhenMoreThan3Interest,
                Settings.pointsWhenMoreThan3Interest,
                Settings.pointsWhenMoreThan3Interest,
                Settings.pointsWhenMoreThan3Interest,
                Settings.pointsWhenMoreThan3Interest,
                Settings.pointsWhenMoreThan3Interest,
                Settings.pointsWhenMoreThan3Interest
            };

            Traverse.Create<DUPLICANTSTATS>().Field<int[]>("APTITUDE_ATTRIBUTE_BONUSES").Value = customAttributes;
        }

        public static void ReadSettings()
        {
            Debug.Log("Loading settings");

            Settings = POptions.ReadSettings<Always3InterestsSettings>();
            if (Settings == null)
            {
                Settings = new Always3InterestsSettings();
            }
        }
    }

    [HarmonyPatch(typeof(MinionStartingStats), "GenerateAptitudes")]
    public class GenerateAptitudesPatch
    {
        public static bool Prefix(MinionStartingStats __instance, string guaranteedAptitudeID)
        {
            if (Always3Interests.Settings.randomNumberOfInterests)
            {
                return true;
            }

            int num = Always3Interests.Settings.numberOfInterests;

            List<SkillGroup> list = new List<SkillGroup>(Db.Get().SkillGroups.resources);
            list.Shuffle<SkillGroup>();
            if (guaranteedAptitudeID != null)
            {
                __instance.skillAptitudes.Add(Db.Get().SkillGroups.Get(guaranteedAptitudeID), DUPLICANTSTATS.APTITUDE_BONUS);
                list.Remove(Db.Get().SkillGroups.Get(guaranteedAptitudeID));
                num--;
            }
            for (int i = 0; i < num; i++)
            {
                __instance.skillAptitudes.Add(list[i], DUPLICANTSTATS.APTITUDE_BONUS);
            }
            return false;
        }
    }


    [HarmonyPatch(typeof(MinionStartingStats))]
    [HarmonyPatch("GenerateTraits")]
    public class TraitPatch
    {
        public static bool Prefix(MinionStartingStats __instance, bool is_starter_minion, List<ChoreGroup> disabled_chore_groups, string guaranteedAptitudeID = null, string guaranteedTraitID = null, bool isDebugMinion = false)
        {
            DUPLICANTSTATS.MAX_TRAITS = 10;

            int statDelta = 0;
            List<string> selectedTraits = new List<string>();
            KRandom randSeed = new KRandom();

            Trait trait = Db.Get().traits.Get(__instance.personality.stresstrait);
            __instance.stressTrait = trait;

            if (Always3Interests.Settings.disableStressTrait)
            {
                __instance.stressTrait = Db.Get().traits.Get("None");
            }

            // set joy trait if it is not disable
            Trait joytrait = Db.Get().traits.Get(__instance.personality.joyTrait);
            __instance.joyTrait = joytrait;
            if (Always3Interests.Settings.disableJoyTrait)
            {
                __instance.joyTrait = Db.Get().traits.Get("None");
            }
            __instance.stickerType = __instance.personality.stickerType;

            
            Trait trait3 = Db.Get().traits.TryGet(__instance.personality.congenitaltrait);
            if (trait3 == null || trait3.Name == "None")
            {
                __instance.congenitaltrait = null;
            }
            else
            {
                __instance.congenitaltrait = trait3;
            }

            Func<List<DUPLICANTSTATS.TraitVal>, bool, bool> func = delegate (List<DUPLICANTSTATS.TraitVal> traitPossibilities, bool positiveTrait)
            {
                if (__instance.Traits.Count > DUPLICANTSTATS.MAX_TRAITS)
                {
                    return false;
                }
                Mathf.Abs(Util.GaussianRandom(0f, 1f));
                int num6 = traitPossibilities.Count;
                int num7;
                if (!positiveTrait)
                {
                    if (DUPLICANTSTATS.rarityDeckActive.Count < 1)
                    {
                        DUPLICANTSTATS.rarityDeckActive.AddRange(DUPLICANTSTATS.RARITY_DECK);
                    }
                    if (DUPLICANTSTATS.rarityDeckActive.Count == DUPLICANTSTATS.RARITY_DECK.Count)
                    {
                        DUPLICANTSTATS.rarityDeckActive.ShuffleSeeded(new KRandom());
                    }
                    num7 = DUPLICANTSTATS.rarityDeckActive[DUPLICANTSTATS.rarityDeckActive.Count - 1];
                    DUPLICANTSTATS.rarityDeckActive.RemoveAt(DUPLICANTSTATS.rarityDeckActive.Count - 1);
                }
                else
                {
                    List<int> list = new List<int>();
                    if (is_starter_minion)
                    {
                        list.Add(__instance.rarityBalance - 1);
                        list.Add(__instance.rarityBalance);
                        list.Add(__instance.rarityBalance);
                        list.Add(__instance.rarityBalance + 1);
                    }
                    else
                    {
                        list.Add(__instance.rarityBalance - 2);
                        list.Add(__instance.rarityBalance - 1);
                        list.Add(__instance.rarityBalance);
                        list.Add(__instance.rarityBalance + 1);
                        list.Add(__instance.rarityBalance + 2);
                    }
                    list.ShuffleSeeded(new KRandom());
                    num7 = list[0];
                    num7 = Mathf.Max(DUPLICANTSTATS.RARITY_COMMON, num7);
                    num7 = Mathf.Min(DUPLICANTSTATS.RARITY_LEGENDARY, num7);
                }

                List<DUPLICANTSTATS.TraitVal> list2 = new List<DUPLICANTSTATS.TraitVal>(traitPossibilities);
                for (int i = list2.Count - 1; i > -1; i--)
                {
                    if (list2[i].rarity != num7)
                    {
                        list2.RemoveAt(i);
                        num6--;
                    }
                }
                list2.ShuffleSeeded(new KRandom());
                foreach (DUPLICANTSTATS.TraitVal traitVal in list2)
                {
                    if (!DlcManager.IsContentActive(traitVal.dlcId))
                    {
                        num6--;
                    }
                    else if (selectedTraits.Contains(traitVal.id))
                    {
                        num6--;
                    }
                    else
                    {
                        Trait trait4 = Db.Get().traits.TryGet(traitVal.id);
                        if (trait4 == null)
                        {
                            global::Debug.LogWarning("Trying to add nonexistent trait: " + traitVal.id);
                            num6--;
                        }
                        else if (!isDebugMinion || trait4.disabledChoreGroups == null || trait4.disabledChoreGroups.Length == 0)
                        {
                            if (is_starter_minion && !trait4.ValidStarterTrait)
                            {
                                num6--;
                            }
                            else if (traitVal.doNotGenerateTrait)
                            {
                                num6--;
                            }
                            else if ((bool)Traverse.Create(__instance).Method("AreTraitAndAptitudesExclusive", new object[] { traitVal, __instance.skillAptitudes }).GetValue())
                            {
                                num6--;
                            }
                            else if (is_starter_minion && guaranteedAptitudeID != null &&
                                (bool)Traverse.Create(__instance).Method("AreTraitAndArchetypeExclusive", new object[] { traitVal, guaranteedAptitudeID }).GetValue())
                            {
                                num6--;
                            }
                            else
                            {
                                if (!(bool)Traverse.Create(__instance).Method("AreTraitsMutuallyExclusive", new object[] { traitVal, selectedTraits }).GetValue())
                                {
                                    selectedTraits.Add(traitVal.id);
                                    statDelta += traitVal.statBonus;
                                    __instance.rarityBalance += (positiveTrait ? (-traitVal.rarity) : traitVal.rarity);
                                    __instance.Traits.Add(trait4);
                                    if (trait4.disabledChoreGroups != null)
                                    {
                                        for (int j = 0; j < trait4.disabledChoreGroups.Length; j++)
                                        {
                                            disabled_chore_groups.Add(trait4.disabledChoreGroups[j]);
                                        }
                                    }
                                    return true;
                                }
                                num6--;
                            }
                        }
                    }
                }
                return false;
            };

            int numberOfGoodTraits = Always3Interests.Settings.numberOfGoodTraits;
            int numberOfBadTraits = Always3Interests.Settings.numberOfBadTraits;

            if (numberOfGoodTraits > 5) numberOfGoodTraits = 5;
            if (numberOfBadTraits > 5) numberOfBadTraits = 5;


            if (!string.IsNullOrEmpty(guaranteedTraitID))
            {
                DUPLICANTSTATS.TraitVal traitVal = DUPLICANTSTATS.GetTraitVal(guaranteedTraitID);
                if (traitVal.id == guaranteedTraitID)
                {
                    Trait trait4 = Db.Get().traits.TryGet(traitVal.id);
                    bool positiveTrait2 = trait4.PositiveTrait;
                    selectedTraits.Add(traitVal.id);
                    statDelta += traitVal.statBonus;
                    
                    __instance.rarityBalance += (positiveTrait2 ? (-traitVal.rarity) : traitVal.rarity);
                    __instance.Traits.Add(trait4);
                    if (trait4.disabledChoreGroups != null)
                    {
                        for (int i = 0; i < trait4.disabledChoreGroups.Length; i++)
                        {
                            disabled_chore_groups.Add(trait4.disabledChoreGroups[i]);
                        }
                    }
                }
            }


            for (int i = 0; i < numberOfBadTraits; i++)
            {
                bool isTraitAdded = false;
                while (!isTraitAdded)
                {
                    isTraitAdded = func(DUPLICANTSTATS.BADTRAITS, false);
                }
            }
            for (int i = 0; i < numberOfGoodTraits; i++)
            {
                bool isTraitAdded = false;
                while (!isTraitAdded)
                {
                    isTraitAdded = func(DUPLICANTSTATS.GOODTRAITS, true);
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
            Always3Interests.ReadSettings();
            //Debug.Log("telepad");
            int startingLevel = Always3Interests.Settings.startingLevelOnPrintingPod;

            Telepad telepad = go.AddOrGet<Telepad>();
            telepad.startingSkillPoints = startingLevel;

        }
    }
}