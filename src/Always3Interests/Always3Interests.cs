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
using static TUNING.DUPLICANTSTATS;

namespace Always3Interests
{
    [RestartRequired]
    [JsonObject(MemberSerialization.OptIn)]
    public class Always3InterestsSettings
    {
        [Option("Number of interests", "The number of interests.")]
        [Limit(0, 6)]
        [JsonProperty]
        public int NumberOfInterests { get; set; }

        [Option("Random number of interests", "Active it to disable the interest modification.")]
        [JsonProperty]
        public bool RandomNumberOfInterests { get; set; }


        [Option("Points when 1 interest", "")]
        [Limit(0, 50)]
        [JsonProperty]
        public int PointsWhen1Interest { get; set; }

        [Option("Points when 2 interest", "")]
        [Limit(0, 50)]
        [JsonProperty]
        public int PointsWhen2Interest { get; set; }

        [Option("Points when 3 interest", "")]
        [Limit(0, 50)]
        [JsonProperty]
        public int PointsWhen3Interest { get; set; }

        [Option("Points when more than 3 interest", "")]
        [Limit(0, 50)]
        [JsonProperty]
        public int PointsWhenMoreThan3Interest { get; set; }

        [Option("Number of Good traits", "")]
        [Limit(0, 5)]
        [JsonProperty]
        public int NumberOfGoodTraits { get; set; }

        [Option("Number of Bad traits", "")]
        [Limit(0, 5)]
        [JsonProperty]
        public int NumberOfBadTraits { get; set; }

        [Option("Disable joy trait", "")]
        [JsonProperty]
        public bool DisableJoyTrait { get; set; }

        [Option("Disable stress trait", "")]
        [JsonProperty]
        public bool DisableStressTrait { get; set; }

        [Option("Starting level on printing pod", "Set the experience of in game printed dups.")]
        [Limit(0, 5)]
        [JsonProperty]
        public int StartingLevelOnPrintingPod { get; set; }

        [Option("Don't print biologic duplicants", "The printing pod will never show you biologic duplicants if this option is checked.\nWill not do anything if Bionic Booster Pack is not owned or activated on your save.", "Bionic Booster Pack")]
        [JsonProperty]
        public bool DisableBiologicDuplicants { get; set; }

        [Option("Don't print bionic duplicants", "The printing pod will never show you bionic duplicants if this option is checked.\nWill not do anything if Bionic Booster Pack is not owned or activated on your save.", "Bionic Booster Pack")]
        [JsonProperty]
        public bool DisableBionicDuplicants { get; set; }

        public Always3InterestsSettings()
        {
            PointsWhen1Interest = 9;
            PointsWhen2Interest = 5;
            PointsWhen3Interest = 1;

            PointsWhenMoreThan3Interest = 1;

            NumberOfInterests = 3;
            RandomNumberOfInterests = true;

            NumberOfGoodTraits = 1;
            NumberOfBadTraits = 1;
            DisableJoyTrait = false;
            DisableStressTrait = false;

            StartingLevelOnPrintingPod = 1;

            DisableBionicDuplicants = false;
            DisableBiologicDuplicants = false;
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
                Settings.PointsWhen1Interest,
                Settings.PointsWhen2Interest,
                Settings.PointsWhen3Interest,
                Settings.PointsWhenMoreThan3Interest,
                Settings.PointsWhenMoreThan3Interest,
                Settings.PointsWhenMoreThan3Interest,
                Settings.PointsWhenMoreThan3Interest,
                Settings.PointsWhenMoreThan3Interest,
                Settings.PointsWhenMoreThan3Interest,
                Settings.PointsWhenMoreThan3Interest,
                Settings.PointsWhenMoreThan3Interest
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
            if (!(__instance.personality.model == BionicMinionConfig.MODEL))
            {

                if (Always3Interests.Settings.RandomNumberOfInterests)
                {
                    return true;
                }

                int num = Always3Interests.Settings.NumberOfInterests;

                List<SkillGroup> list = new List<SkillGroup>(Db.Get().SkillGroups.resources);
                list.RemoveAll((SkillGroup match) => !match.allowAsAptitude);
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
            }
            return false;
        }
    }


    [HarmonyPatch(typeof(MinionStartingStats))]
    [HarmonyPatch("GenerateTraits")]
    public class TraitPatch
    {
        public static bool Prefix(ref int __result, MinionStartingStats __instance, bool is_starter_minion, List<ChoreGroup> disabled_chore_groups, string guaranteedAptitudeID = null, string guaranteedTraitID = null, bool isDebugMinion = false)
        {

            DUPLICANTSTATS.MAX_TRAITS = 10;
            int statDelta = 0;
            __result = statDelta;
            List<string> selectedTraits = new List<string>();
            KRandom randSeed = new KRandom();

            Trait trait = Db.Get().traits.Get(__instance.personality.stresstrait);
            __instance.stressTrait = trait;

            if (Always3Interests.Settings.DisableStressTrait)
            {
                __instance.stressTrait = Db.Get().traits.Get("None");
            }

            // set joy trait if it is not disable
            Trait joytrait = Db.Get().traits.Get(__instance.personality.joyTrait);
            __instance.joyTrait = joytrait;
            if (Always3Interests.Settings.DisableJoyTrait)
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

            if (__instance.personality.model == GameTags.Minions.Models.Bionic)
            {
                DUPLICANTSTATS.TraitVal random = DUPLICANTSTATS.BIONICBUGTRAITS.GetRandom();
                SelectTrait(random, Db.Get().traits.Get(random.id), isPositiveTrait: false);
                DUPLICANTSTATS.TraitVal traitVal2 = (guaranteedAptitudeID == null) ? DUPLICANTSTATS.BIONICUPGRADETRAITS.GetRandom() : Traverse.Create(__instance).Method("GetBionicTraitsCompatibleWithArchetype", new object[] { guaranteedAptitudeID }).GetValue<List<DUPLICANTSTATS.TraitVal>>().GetRandom();
                SelectTrait(traitVal2, Db.Get().traits.Get(traitVal2.id), isPositiveTrait: true);

                __result = statDelta;
                return false;
            }

            Func<List<TraitVal>, bool, bool> func = delegate (List<TraitVal> traitPossibilities, bool positiveTrait)
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
                    global::Debug.Assert(SaveLoader.Instance != null, "IsDLCActiveForCurrentSave should not be called from the front end");
                    if (!Game.IsCorrectDlcActiveForCurrentSave(traitVal))
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
                                    SelectTrait(traitVal, trait4, positiveTrait);
                                    return true;
                                }
                                num6--;
                            }
                        }
                    }
                }
                return false;
            };

            int numberOfGoodTraits = Always3Interests.Settings.NumberOfGoodTraits;
            int numberOfBadTraits = Always3Interests.Settings.NumberOfBadTraits;

            if (numberOfGoodTraits > 5) numberOfGoodTraits = 5;
            if (numberOfBadTraits > 5) numberOfBadTraits = 5;

            if (!is_starter_minion)
            {
                if (DUPLICANTSTATS.podTraitConfigurationsActive.Count < 1)
                {
                    DUPLICANTSTATS.podTraitConfigurationsActive.AddRange(DUPLICANTSTATS.POD_TRAIT_CONFIGURATIONS_DECK);
                }

                if (DUPLICANTSTATS.podTraitConfigurationsActive.Count == DUPLICANTSTATS.POD_TRAIT_CONFIGURATIONS_DECK.Count)
                {
                    DUPLICANTSTATS.podTraitConfigurationsActive.ShuffleSeeded(randSeed);
                }

                DUPLICANTSTATS.podTraitConfigurationsActive.RemoveAt(DUPLICANTSTATS.podTraitConfigurationsActive.Count - 1);
            }

            if (!string.IsNullOrEmpty(guaranteedTraitID))
            {
                DUPLICANTSTATS.TraitVal traitVal = DUPLICANTSTATS.GetTraitVal(guaranteedTraitID);
                if (traitVal.id == guaranteedTraitID)
                {
                    Trait trait4 = Db.Get().traits.TryGet(traitVal.id);
                    bool positiveTrait2 = trait4.PositiveTrait;
                    selectedTraits.Add(traitVal.id);
                    statDelta += traitVal.statBonus;

                    __instance.rarityBalance += positiveTrait2 ? (-traitVal.rarity) : traitVal.rarity;
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

            if (__instance.congenitaltrait != null)
            {
                DUPLICANTSTATS.TraitVal traitVal4;
                if (__instance.congenitaltrait.PositiveTrait)
                {
                    traitVal4 = DUPLICANTSTATS.GOODTRAITS.Find((DUPLICANTSTATS.TraitVal match) => match.id == __instance.congenitaltrait.Id);
                }
                else
                {
                    traitVal4 = DUPLICANTSTATS.BADTRAITS.Find((DUPLICANTSTATS.TraitVal match) => match.id == __instance.congenitaltrait.Id);
                }
                SelectTrait(traitVal4, __instance.congenitaltrait, __instance.congenitaltrait.PositiveTrait);
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
            __instance.IsValid = true;
            __result = statDelta;

            return false;

            void SelectTrait(DUPLICANTSTATS.TraitVal traitVal, Trait trait2, bool isPositiveTrait)
            {
                selectedTraits.Add(traitVal.id);
                statDelta += traitVal.statBonus;
                __instance.rarityBalance += isPositiveTrait ? (-traitVal.rarity) : traitVal.rarity;
                __instance.Traits.Add(trait2);
                if (trait2.disabledChoreGroups != null)
                {
                    for (int k = 0; k < trait2.disabledChoreGroups.Length; k++)
                    {
                        disabled_chore_groups.Add(trait2.disabledChoreGroups[k]);
                    }
                }
            }
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
            int startingLevel = Always3Interests.Settings.StartingLevelOnPrintingPod;

            Telepad telepad = go.AddOrGet<Telepad>();
            telepad.startingSkillPoints = startingLevel;

        }
    }

    [HarmonyPatch(typeof(CharacterContainer), "GenerateCharacter")]
    public class CharacterContainer_GenerateCharacter_Patch
    {
        public static void Prefix(bool is_starter, ref List<Tag> ___permittedModels)
        {
            if (!is_starter && Game.IsDlcActiveForCurrentSave("DLC3_ID"))
            {
                if (Always3Interests.Settings.DisableBionicDuplicants) ___permittedModels.Remove(GameTags.Minions.Models.Bionic);
                if (Always3Interests.Settings.DisableBiologicDuplicants) ___permittedModels.Remove(GameTags.Minions.Models.Standard);
            };

        }
    }
}