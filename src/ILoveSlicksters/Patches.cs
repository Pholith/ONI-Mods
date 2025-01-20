using HarmonyLib;
using Klei.AI;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Database;
using PeterHan.PLib.Options;
using Pholib;
using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

namespace ILoveSlicksters
{

    public class ILoveSlicksters : UserMod2
    {

        //public static string modPath;
        public static SlicksterOptions Settings { get; private set; }


        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            new POptions().RegisterOptions(this, typeof(SlicksterOptions));

            // Init PLib and settings
            PUtil.InitLibrary();
            Utilities.GenerateStringsTemplate(typeof(PHO_STRINGS));

            Settings = POptions.ReadSettings<SlicksterOptions>();
            if (Settings == null)
            {
                Settings = new SlicksterOptions();
            }
            new PLocalization().Register();

            WorldGenPatches.VeryHot2_patch.OnLoad();

            // Egg Chance modifier
            Type[] parameters_type = new Type[] { typeof(string), typeof(Tag), typeof(float), typeof(float), typeof(float), typeof(bool) };
            object[] paramaters = new object[] { EthanolOilfloaterConfig.ID, EthanolOilfloaterConfig.EGG_ID.ToTag(), (-20f).CelciusToKelvin(), 10f.CelciusToKelvin(), PHO_TUNING.EGG_MODIFIER_PER_SECOND.NORMAL, false };

            CREATURES.EGG_CHANCE_MODIFIERS.MODIFIER_CREATORS.Add(
                Traverse.Create<CREATURES.EGG_CHANCE_MODIFIERS>().Method("CreateTemperatureModifier", parameters_type).GetValue<System.Action>(paramaters)
            );

            object[] paramaters2 = new object[] { RobotOilfloaterConfig.ID, RobotOilfloaterConfig.EGG_ID.ToTag(), 180f.CelciusToKelvin(), 450f.CelciusToKelvin(), PHO_TUNING.EGG_MODIFIER_PER_SECOND.NORMAL, false };

            CREATURES.EGG_CHANCE_MODIFIERS.MODIFIER_CREATORS.Add(
                Traverse.Create<CREATURES.EGG_CHANCE_MODIFIERS>().Method("CreateTemperatureModifier", parameters_type).GetValue<System.Action>(paramaters2)
            );

            object[] paramaters3 = new object[] { FrozenOilfloaterConfig.ID, FrozenOilfloaterConfig.EGG_ID.ToTag(), (-80f).CelciusToKelvin(), 0f.CelciusToKelvin(), PHO_TUNING.EGG_MODIFIER_PER_SECOND.FAST, false };

            CREATURES.EGG_CHANCE_MODIFIERS.MODIFIER_CREATORS.Add(
                Traverse.Create<CREATURES.EGG_CHANCE_MODIFIERS>().Method("CreateTemperatureModifier", parameters_type).GetValue<System.Action>(paramaters3)
            );

            CREATURES.EGG_CHANCE_MODIFIERS.MODIFIER_CREATORS.Add(
                PHO_TUNING.CreateLightModifier(LeafyOilfloaterConfig.ID, LeafyOilfloaterConfig.EGG_ID.ToTag(), PHO_TUNING.EGG_MODIFIER_PER_SECOND.FAST , true)
            );
            CREATURES.EGG_CHANCE_MODIFIERS.MODIFIER_CREATORS.Add(
                PHO_TUNING.CreateElementModifier(OwO_OilfloaterConfig.ID, OwO_OilfloaterConfig.EGG_ID.ToTag(), SimHashes.Hydrogen, PHO_TUNING.EGG_MODIFIER_PER_SECOND.FAST3, false)
            );
            CREATURES.EGG_CHANCE_MODIFIERS.MODIFIER_CREATORS.Add(
                PHO_TUNING.CreateElementModifier(AquaOilfloaterConfig.ID, AquaOilfloaterConfig.EGG_ID.ToTag(), SimHashes.Water, PHO_TUNING.EGG_MODIFIER_PER_SECOND.FAST2, false)
            );

            if (!Settings.DisableNewEggs)
            {

                // Add custom slicksters eggs into vanilla slicksters
                OilFloaterTuning.EGG_CHANCES_BASE.Add(
                new FertilityMonitor.BreedingChance
                {
                    egg = LeafyOilfloaterConfig.EGG_ID.ToTag(),
                    weight = 0.02f
                });
                OilFloaterTuning.EGG_CHANCES_BASE.Add(
                new FertilityMonitor.BreedingChance
                {
                    egg = RobotOilfloaterConfig.EGG_ID.ToTag(),
                    weight = 0.02f
                });
                OilFloaterTuning.EGG_CHANCES_BASE.Add(
                new FertilityMonitor.BreedingChance
                {
                    egg = EthanolOilfloaterConfig.EGG_ID.ToTag(),
                    weight = 0.02f
                });
                OilFloaterTuning.EGG_CHANCES_BASE.Add(
                new FertilityMonitor.BreedingChance
                {
                    egg = AquaOilfloaterConfig.EGG_ID.ToTag(),
                    weight = 0.02f
                });


                OilFloaterTuning.EGG_CHANCES_HIGHTEMP.Add(
                new FertilityMonitor.BreedingChance
                {
                    egg = RobotOilfloaterConfig.EGG_ID.ToTag(),
                    weight = 0.02f
                });


                OilFloaterTuning.EGG_CHANCES_DECOR[1] = new FertilityMonitor.BreedingChance
                {
                    egg = EthanolOilfloaterConfig.EGG_ID.ToTag(),
                    weight = 0.02f
                };
                OilFloaterTuning.EGG_CHANCES_DECOR.Add(new FertilityMonitor.BreedingChance
                {
                    egg = OwO_OilfloaterConfig.EGG_ID.ToTag(),
                    weight = 0.02f
                });
                OilFloaterTuning.EGG_CHANCES_DECOR.Add(new FertilityMonitor.BreedingChance
                {
                    egg = AquaOilfloaterConfig.EGG_ID.ToTag(),
                    weight = 0.02f
                });
                OilFloaterTuning.EGG_CHANCES_DECOR.Add(new FertilityMonitor.BreedingChance
                {
                    egg = LeafyOilfloaterConfig.EGG_ID.ToTag(),
                    weight = 0.02f
                }); 
                OilFloaterTuning.EGG_CHANCES_DECOR.Add(new FertilityMonitor.BreedingChance
                {
                    egg = FrozenOilfloaterConfig.EGG_ID.ToTag(),
                    weight = 0.02f
                });

            }
            // Kg eaten patch
            if (Settings.IncreasesVanillaSlickstersConsumption)
            {
                Traverse.Create<OilFloaterConfig>().Field<float>("KG_ORE_EATEN_PER_CYCLE").Value = PHO_TUNING.OILFLOATER.KG_ORE_EATEN_PER_CYCLE.HIGH2;

                float CALORIES_PER_KG_OF_ORE = PHO_TUNING.OILFLOATER.STANDARD_CALORIES_PER_CYCLE / PHO_TUNING.OILFLOATER.KG_ORE_EATEN_PER_CYCLE.HIGH2;
                Traverse.Create<OilFloaterConfig>().Field<float>("CALORIES_PER_KG_OF_ORE").Value = CALORIES_PER_KG_OF_ORE;
                Traverse.Create<OilFloaterHighTempConfig>().Field<float>("CALORIES_PER_KG_OF_ORE").Value = CALORIES_PER_KG_OF_ORE;
            }
        }
    }

    [HarmonyPatch(typeof(BaseOilFloaterConfig))]
    [HarmonyPatch("SetupDiet")]
    internal class BiggerConsumptionRatePatch
    {
        public static void Postfix(GameObject __result)
        {
            GasAndLiquidConsumerMonitor.Def def = __result.AddOrGetDef<GasAndLiquidConsumerMonitor.Def>();
            List<string> vanillasSlickstersIds = new List<string>
            {
                "Oilfloater",
                "OilfloaterBaby",
                "OilfloaterDecor",
                "OilfloaterDecorBaby",
                "OilfloaterHighTemp",
                "OilfloaterHighTempBaby"
            };

            KPrefabID kId = __result.AddOrGet<KPrefabID>();

            // If the slickster is a vanilla slickster, I check the options
            if (vanillasSlickstersIds.Contains(kId.PrefabTag.Name))
            {
                if (ILoveSlicksters.Settings.IncreasesVanillaSlickstersConsumption)
                {
                    def.consumptionRate = 5f;
                }
            }
            else
            {
                if (def.consumptionRate <= 0.5f)
                {
                    def.consumptionRate = 5f * ILoveSlicksters.Settings.ConsumptionMultiplier;
                }
            }

        }
    }

    // add some slickster egg recipes to make them easier to unlock
    [HarmonyPatch(typeof(SupermaterialRefineryConfig))]
    [HarmonyPatch("ConfigureBuildingTemplate")]
    public class SuperRefineryConfig_ConfigureBuildingTemplateAddSlickstersPatch
    {
        public static void Postfix()
        {
            if (!ILoveSlicksters.Settings.DisableSlickstersRecipes)
            {
                Utilities.AddComplexRecipe(
                    input: new[] {
                        new ComplexRecipe.RecipeElement(SimHashes.Ethanol.CreateTag(), 50f),
                        new ComplexRecipe.RecipeElement(SimHashes.Lime.CreateTag(), 3f),
                        new ComplexRecipe.RecipeElement(SimHashes.CarbonDioxide.CreateTag(), 10f),
                    },
                    output: new[] { new ComplexRecipe.RecipeElement(TagManager.Create(EthanolOilfloaterConfig.EGG_ID), 1f) },
                    fabricatorId: SupermaterialRefineryConfig.ID,
                    productionTime: 20f,
                    recipeDescription: PHO_STRINGS.VARIANT_ETHANOL.DESC,
                    nameDisplayType: ComplexRecipe.RecipeNameDisplay.Result,
                    sortOrder: 975
                );
                Utilities.AddComplexRecipe(
                    input: new[] {
                        new ComplexRecipe.RecipeElement(EthanolOilfloaterConfig.EGG_ID.ToTag(), 2f),
                        new ComplexRecipe.RecipeElement(SimHashes.Algae.CreateTag(), 50f),
                        new ComplexRecipe.RecipeElement(SimHashes.Water.CreateTag(), 10f),
                        new ComplexRecipe.RecipeElement(SimHashes.SaltWater.CreateTag(), 10f),
                    },
                    output: new[] { new ComplexRecipe.RecipeElement(TagManager.Create(AquaOilfloaterConfig.EGG_ID), 1f) },
                    fabricatorId: SupermaterialRefineryConfig.ID,
                    productionTime: 20f,
                    recipeDescription: PHO_STRINGS.VARIANT_AQUA.DESC,
                    nameDisplayType: ComplexRecipe.RecipeNameDisplay.Result,
                    sortOrder: 980
                );
                Utilities.AddComplexRecipe(
                    input: new[] {
                        new ComplexRecipe.RecipeElement(PrickleFlowerConfig.SEED_ID.ToTag(), 10f),
                        new ComplexRecipe.RecipeElement(SimHashes.Algae.CreateTag(), 50f),
                        new ComplexRecipe.RecipeElement(SimHashes.SlimeMold.CreateTag(), 50f),
                    },
                    output: new[] { new ComplexRecipe.RecipeElement(TagManager.Create(LeafyOilfloaterConfig.EGG_ID), 1f) },
                    fabricatorId: SupermaterialRefineryConfig.ID,
                    productionTime: 20f,
                    recipeDescription: PHO_STRINGS.VARIANT_LEAFY.DESC,
                    nameDisplayType: ComplexRecipe.RecipeNameDisplay.Result,
                    sortOrder: 985
                );
                Utilities.AddComplexRecipe(
                    input: new[] {
                        new ComplexRecipe.RecipeElement(OilFloaterConfig.EGG_ID.ToTag(), 2f),
                        new ComplexRecipe.RecipeElement(SimHashes.Steel.CreateTag(), 20f),
                        new ComplexRecipe.RecipeElement(SimHashes.Iron.CreateTag(), 50f),
                    },
                    output: new[] { new ComplexRecipe.RecipeElement(TagManager.Create(RobotOilfloaterConfig.EGG_ID), 1f) },
                    fabricatorId: SupermaterialRefineryConfig.ID,
                    productionTime: 20f,
                    recipeDescription: PHO_STRINGS.VARIANT_ROBOT.DESC,
                    nameDisplayType: ComplexRecipe.RecipeNameDisplay.Result,
                    sortOrder: 990
                );
                Utilities.AddComplexRecipe(
                    input: new[] {
                        new ComplexRecipe.RecipeElement(OilFloaterDecorConfig.EGG_ID.ToTag(), 2f),
                        new ComplexRecipe.RecipeElement(SimHashes.Ice.CreateTag(), 20f),
                        new ComplexRecipe.RecipeElement(SimHashes.CarbonDioxide.CreateTag(), 50f),
                    },
                    output: new[] { new ComplexRecipe.RecipeElement(TagManager.Create(FrozenOilfloaterConfig.EGG_ID), 1f) },
                    fabricatorId: SupermaterialRefineryConfig.ID,
                    productionTime: 20f,
                    recipeDescription: PHO_STRINGS.VARIANT_ROBOT.DESC,
                    nameDisplayType: ComplexRecipe.RecipeNameDisplay.Result,
                    sortOrder: 986
                );
            }
        }
    }

    // add the OwO effect
    [HarmonyPatch(typeof(Db))]
    [HarmonyPatch("Initialize")]
    public class OwO_EffectPatch
    {
        public static void Postfix(Db __instance)
        {
            //Utilities.AddWorldYaml("Asteroid_Slicksteria", typeof(PHO_STRINGS));


            Effect OwO_Effect = new Effect("OwOEffect", "OwO Effect", "This duplicant saw something so cute that he can't think of anything else.", 300f, true, true, false, null, 10f, null, "");
            OwO_Effect.Add(new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, -DUPLICANTSTATS.QOL_STRESS.BELOW_EXPECTATIONS.HARD, "OwO Effect"));
            __instance.effects.Add(OwO_Effect);

        }
    }

    // add Light sensibility for all slickers
    [HarmonyPatch(typeof(BaseOilFloaterConfig))]
    [HarmonyPatch("BaseOilFloater")]
    public class AddLightSensibilityPatch
    {
        public static void Postfix(GameObject __result)
        {
            __result.AddOrGet<LightVulnerable>();
            __result.AddOrGet<SimplePressureVulnerable>();
            __result.AddOrGet<ElementVulnerable>();
            __result.AddOrGetDef<SlicksterDance.Def>();
        }
    }

    [HarmonyPatch(typeof(Immigration))]
    [HarmonyPatch("ConfigureCarePackages")]
    public static class Immigration_ConfigureCarePackages_Patch
    {
        public static void Postfix(ref Immigration __instance)
        {
            if (!ILoveSlicksters.Settings.DisableSlickstersCarePackages)
            {
                Utilities.AddCarePackage(ref __instance, LeafyOilfloaterBabyConfig.ID, 2f, () => Utilities.CycleInRange(20, 700));
                Utilities.AddCarePackage(ref __instance, EthanolOilfloaterConfig.EGG_ID, 2f, () => Utilities.CycleInRange(50, 900) && Utilities.IsOilFieldDiscovered() && Utilities.IsSimHashesDiscovered(SimHashes.Ethanol) && Utilities.IsTagDiscovered(EthanolOilfloaterConfig.ID));
                Utilities.AddCarePackage(ref __instance, OwO_OilfloaterConfig.EGG_ID, 2f, () => Utilities.CycleInRange(150, 700) && Utilities.IsOilFieldDiscovered() && Utilities.IsTagDiscovered(OwO_OilfloaterConfig.ID));
                Utilities.AddCarePackage(ref __instance, FrozenOilfloaterConfig.EGG_ID, 1f, () => Utilities.CycleCondition(500) && Utilities.IsOilFieldDiscovered() && Utilities.IsTagDiscovered(FrozenOilfloaterConfig.ID));
                Utilities.AddCarePackage(ref __instance, AquaOilfloaterConfig.EGG_ID, 2f, () => Utilities.CycleCondition(500) && Utilities.IsTagDiscovered(AquaOilfloaterConfig.ID));
                Utilities.AddCarePackage(ref __instance, RobotOilfloaterConfig.EGG_ID, 1f, () => Utilities.CycleCondition(500) &&
                    Utilities.IsOilFieldDiscovered() && Utilities.IsSimHashesDiscovered(SimHashes.Steel) && Utilities.IsTagDiscovered(OilFloaterHighTempConfig.ID));

            }
        }
    }



    [HarmonyPatch(typeof(OilFloaterDecorConfig))]
    [HarmonyPatch("CreateOilFloater")]
    public class OilFloaterDecorConfigDietPatch
    {
        public static void Postfix(GameObject __result)
        {
            if (!ILoveSlicksters.Settings.DisableLonghairSlicksters)
            {

                // Patch the diet
                Diet diet = new Diet(new Diet.Info[]
                {
                    new Diet.Info(new HashSet<Tag>
                    {
                        SimHashes.Oxygen.CreateTag()
                    }, ((SimHashes) ILoveSlicksters.Settings.LonghairElement).CreateTag(), Traverse.Create<OilFloaterDecorConfig>().Field<float>("CALORIES_PER_KG_OF_ORE").Value, TUNING.CREATURES.CONVERSION_EFFICIENCY.GOOD_1, null, 0, false, Diet.Info.FoodType.EatSolid, false)
                });
                CreatureCalorieMonitor.Def def = __result.AddOrGetDef<CreatureCalorieMonitor.Def>();
                def.diet = diet;
                __result.AddOrGetDef<GasAndLiquidConsumerMonitor.Def>().diet = diet;


                // Patch the dead drop
                string[] loot = new string[5];
                loot[0] = BasicFabricConfig.ID;
                loot[1] = BasicFabricConfig.ID;
                loot[2] = BasicFabricConfig.ID;
                loot[3] = MeatConfig.ID;
                loot[4] = MeatConfig.ID;

                __result.AddOrGet<Butcherable>().SetDrops(loot);


            }
        }
    }

    public enum LonghairElementList
    {
        CO2 = SimHashes.CarbonDioxide,
        Oxygen = SimHashes.Oxygen,
        OxyRock = SimHashes.OxyRock,
        Chlorine = SimHashes.ChlorineGas,
        Helium = SimHashes.Helium,
        Water = SimHashes.Water,
        SaltWater = SimHashes.SaltWater,
        Brine = SimHashes.Brine,
        Dirt = SimHashes.Dirt,
        Snow = SimHashes.Snow,
        Carbon = SimHashes.Carbon,
        Phosphorite = SimHashes.Phosphorite,
        Rust = SimHashes.Rust
    }

}
