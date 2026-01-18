using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace UtilLibs
{
    // Thanks Sgt_Imalas and Romen for this amazing script 
    internal class NewCritterSystemUtility
    {
        /// ## New Critter anim system:
        /// Critter anims now consist of two components, the "build" kanim and various "anim" kanims.
        /// This means that modded critters no longer require any own anims in their kanim, and can reuse the vanilla ones
        /// In their infinite wisdom however, Klei has hardcoded that "build" kanim into their "Base[Critter]" method, which we dont want.
        /// 
        /// for that reason, call FixCritterAnimationOverrides for the critter gameobject, for example after ExtendEntityToWildCreature. 
        /// ONLY DO THIS FOR GROWN UP CRITTERS ATM!! babies still use the old system.
        /// Also make sure that the kanimId given to the Base(Critter) function is the vanilla "anim" kanim, for example hatch_kanim for hatches
        /// See the code under "CritterExample" for reference
        /// 
        /// 
        /// Furthermore, we need to move our modded kanim into the same batch group (kanim grouping stuff) as the vanilla anims of that critter
        /// To do so, copy (or just use) the KAnimGroupFile_Load_Patch below and adjust its variables so "animInTargetGroupId" is one of the vanilla critter kanims, 
        /// and the "swaps" list contains all your modded "build" kanims
        ///
        /// On these modded kanims:
        /// Modded critter kanims now only serve as symbol override storages, they shouldnt contain any actual animations anymore
        /// also make sure that all the critter symbols have the same SymbolOverridePrefix, which must be unique for this modded morph, without overlaps with other morphs of this critter 
        /// If you migrate from an older critter anim, I can recommend exporting the vanilla critter "(critter)_build_kanim" and draw/paste over its existing symbols
        /// afterwards rename the symbols to include your SymbolOverridePrefix

        #region CritterExample

        static string SymbolOverridePrefix = "somethingUnique_";
        static string BaseTraitId = "SomeCritterBaseTrait";

        public static GameObject CreateHatch(
            string id,
            string name,
            string desc,
            string anim_file,
            bool is_baby)
        {

            ///"anim" - kanim should be the vanilla one, except for babies atm
            string vanillaAnimationKanimId = is_baby ? anim_file : "hatch_kanim";
            string symbol_override_prefix = is_baby ? null : SymbolOverridePrefix;


            GameObject wildCreature = EntityTemplates.ExtendEntityToWildCreature(
                BaseHatchConfig.BaseHatch(
                    id,
                    name,
                    desc,
                    vanillaAnimationKanimId,
                    BaseTraitId,
                    is_baby,
                    symbol_override_prefix
                ),
                HatchTuning.PEN_SIZE_PER_CREATURE);

            ///fix symbol overrides for adult critter
            ///
            if (is_baby == false)
                NewCritterSystemUtility.FixCritterAnimationOverrides(wildCreature, anim_file, vanillaAnimationKanimId, symbol_override_prefix);

            ///[...] more critter stuff goes here
            ///

            return wildCreature;
        }

        #endregion





        /// <summary>
        /// this patch puts the critter anims into the correct anim group
        /// </summary>
        [HarmonyPatch(typeof(KAnimGroupFile), "Load")]
        public class KAnimGroupFile_Load_Patch
        {
            public static void Prefix(KAnimGroupFile __instance)
            {
                NewCritterSystemUtility.MoveModKanimsIntoCorrectCritterAnimGroup(
                    __instance,
                    new HashSet<HashedString>()
                    {
						///all the adult animations, should only contain symbol overrides with a prefix, no animations
						/*"diamond_hatch_adult_kanim",
                        "floral_hatch_adult_kanim",
                        "wooden_hatch_adult_kanim"*/					
                        "custom_oilfloater2_kanim",
                        "custom_oilfloater_kanim",
                    },
                    ///vanilla critter "animation" kanim
                    "oilfloater_kanim");
            }
        }

        /// <summary>
        /// call this method after EntityTemplates.ExtendEntityToWildCreature if the critter is not a baby(!)
        /// </summary>
        /// <param name="wildCreature"></param>
        /// <param name="modCritterKanimId"></param>
        /// <param name="vanillaCritterMovementKanimId"></param>
        /// <param name="symbol_override_prefix"></param>
        public static void FixCritterAnimationOverrides(GameObject wildCreature, string modCritterKanimId, string vanillaCritterMovementKanimId, string symbol_override_prefix)
        {
            var buildKanim = Assets.GetAnim(modCritterKanimId);
            var animKanim = Assets.GetAnim(vanillaCritterMovementKanimId);
            ///place the proper build anim in the kbac because klei hardcoded "hatch_build" in BaseHatch...
            if (wildCreature.TryGetComponent<KBatchedAnimController>(out var kbac))
            {
                kbac.AnimFiles = new KAnimFile[] {
                    buildKanim //build file - custom modded critter icons
					,animKanim //anim file - default vanilla critter animations
					};
            }
            ///apply proper symbol overrides
            if (wildCreature.TryGetComponent<SymbolOverrideController>(out var soc))
            {
                soc.ApplySymbolOverridesByAffix((buildKanim == null) ? animKanim : buildKanim, symbol_override_prefix);
            }
            Debug.Log("Fixed animation override for critter: " + wildCreature.name);
        }

        /// <summary>
        /// Required to move all animations "swaps" into the animation group of animation "animInTargetGroupId"
        /// </summary>
        /// <param name="kAnimGroupFile"></param>
        /// <param name="groupIdHash"></param>
        /// <param name="swaps"></param>
        public static void MoveModKanimsIntoCorrectCritterAnimGroup(KAnimGroupFile kAnimGroupFile, HashSet<HashedString> swaps, string animInTargetGroupId)
        {
            var groups = kAnimGroupFile.GetData();
            if (!global::Assets.TryGetAnim(animInTargetGroupId, out var animInTargetGroup))
            {
                Debug.LogWarning($"Could not find anim {animInTargetGroupId} in asset list!");
                return;
            }
            KAnimGroupFile.Group targetGroup = null;
            foreach (KAnimGroupFile.Group group in groups)
            {
                if (group.animFiles.Contains(animInTargetGroup))
                {
                    targetGroup = group;
                    break;
                }
            }
            if (targetGroup == null)
            {
                Debug.LogWarning($"Could not find anim group for {animInTargetGroupId}!");
                return;
            }
            MoveKanimsToNewGroup(kAnimGroupFile, targetGroup.id.HashValue, swaps);
        }


        /// <summary>
        /// Required to register the correct anim group for custom made interact anims
        /// </summary>
        /// <param name="kAnimGroupFile"></param>
        /// <param name="groupIdHash"></param>
        /// <param name="swaps"></param>
        public static void MoveKanimsToNewGroup(KAnimGroupFile kAnimGroupFile, int groupIdHash, HashSet<HashedString> swaps)
        {
            var groups = kAnimGroupFile.GetData();
            var swapAnimsGroup = KAnimGroupFile.GetGroup(new HashedString(groupIdHash));

            // remove the wrong group
            groups.RemoveAll(g => swaps.Contains(g.animNames[0]));

            foreach (var swap in swaps)
            {
                // readd to correct group
                var anim = global::Assets.GetAnim(swap);

                if (anim == null)

                {
                    Debug.LogWarning("anim " + swap + " not found");
                    continue;
                }
                if (swapAnimsGroup.animFiles.Contains(anim) || swapAnimsGroup.animNames.Contains(anim.name))
                {

                    Debug.LogWarning("anim " + swap + " already in group");
                    continue;
                }

                swapAnimsGroup.animFiles.Add(anim);
                swapAnimsGroup.animNames.Add(anim.name);

                var data = swapAnimsGroup;

                Debug.Log(anim + "; " + anim.name + " added to group");
            }
        }
    }
}