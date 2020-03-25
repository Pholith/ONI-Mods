﻿using Harmony;
using Klei.AI;
using Pholib;
using System;
using TUNING;
using UnityEngine;

namespace ILoveSlicksters
{

    class Patches
    {
        public static string modPath;

        // Egg chance patches
        public static void OnLoad(string modPath)
        {
            Patches.modPath = modPath;

            // Egg Chance modifier
            Type[] parameters_type = new Type[] { typeof(string), typeof(Tag), typeof(float), typeof(float), typeof(float), typeof(bool) };
            object[] paramaters = new object[] { "EthanolOilfloater", "EthanolOilfloaterEgg".ToTag(), 243.15f, 293.15f, PHO_TUNING.EGG_MODIFIER_PER_SECOND.NORMAL, false };

            CREATURES.EGG_CHANCE_MODIFIERS.MODIFIER_CREATORS.Add(
                Traverse.Create<CREATURES.EGG_CHANCE_MODIFIERS>().Method("CreateTemperatureModifier", parameters_type).GetValue<System.Action>(paramaters)
            );

            object[] paramaters2 = new object[] { "RobotOilfloater", "RobotOilfloaterEgg".ToTag(), 523.15f, 743.15f, PHO_TUNING.EGG_MODIFIER_PER_SECOND.NORMAL, false };

            CREATURES.EGG_CHANCE_MODIFIERS.MODIFIER_CREATORS.Add(
                Traverse.Create<CREATURES.EGG_CHANCE_MODIFIERS>().Method("CreateTemperatureModifier", parameters_type).GetValue<System.Action>(paramaters2)
            );

            object[] paramaters3 = new object[] { "FrozenOilfloater", "FrozenOilfloaterEgg".ToTag(), 210.15f, 273.15f, PHO_TUNING.EGG_MODIFIER_PER_SECOND.NORMAL, false };

            CREATURES.EGG_CHANCE_MODIFIERS.MODIFIER_CREATORS.Add(
                Traverse.Create<CREATURES.EGG_CHANCE_MODIFIERS>().Method("CreateTemperatureModifier", parameters_type).GetValue<System.Action>(paramaters3)
            );

            CREATURES.EGG_CHANCE_MODIFIERS.MODIFIER_CREATORS.Add(
                PHO_TUNING.CreateLightModifier("LeafyOilfloater", "LeafyOilfloaterEgg".ToTag(), PHO_TUNING.EGG_MODIFIER_PER_SECOND.NORMAL, true)
            );
            CREATURES.EGG_CHANCE_MODIFIERS.MODIFIER_CREATORS.Add(
                PHO_TUNING.CreateElementModifier("OwO_Oilfloater", "OwO_OilfloaterEgg".ToTag(), SimHashes.Hydrogen, PHO_TUNING.EGG_MODIFIER_PER_SECOND.FAST2, false)
            );
            CREATURES.EGG_CHANCE_MODIFIERS.MODIFIER_CREATORS.Add(
                PHO_TUNING.CreateElementModifier("AquaOilFloater", "AquaOilFloaterEgg".ToTag(), SimHashes.Water, PHO_TUNING.EGG_MODIFIER_PER_SECOND.FAST * 10, false)
            );



            // Add custom slicksters eggs into vanilla slicksters
            OilFloaterTuning.EGG_CHANCES_BASE.Add(
            new FertilityMonitor.BreedingChance
            {
                egg = "LeafyOilfloaterEgg".ToTag(),
                weight = 0.02f
            });
            OilFloaterTuning.EGG_CHANCES_HIGHTEMP.Add(
            new FertilityMonitor.BreedingChance
            {
                egg = "RobotOilfloaterEgg".ToTag(),
                weight = 0.02f
            });

            OilFloaterTuning.EGG_CHANCES_DECOR[1] = new FertilityMonitor.BreedingChance
            {
                egg = "EthanolOilfloaterEgg".ToTag(),
                weight = 0.02f
            };
            OilFloaterTuning.EGG_CHANCES_DECOR.Add(new FertilityMonitor.BreedingChance
            {
                egg = "OwO_OilfloaterEgg".ToTag(),
                weight = 0.20f
            });
        }
    }
    // Kg eaten patch
    public class KG_Eaten_Patch
    {
        public static void OnLoad()
        {
            Traverse.Create<OilFloaterConfig>().Field<float>("KG_ORE_EATEN_PER_CYCLE").Value = PHO_TUNING.OILFLOATER.KG_ORE_EATEN_PER_CYCLE.HIGH2;

            float CALORIES_PER_KG_OF_ORE = PHO_TUNING.OILFLOATER.STANDARD_CALORIES_PER_CYCLE / PHO_TUNING.OILFLOATER.KG_ORE_EATEN_PER_CYCLE.HIGH2;

            Traverse.Create<OilFloaterConfig>().Field<float>("CALORIES_PER_KG_OF_ORE").Value = CALORIES_PER_KG_OF_ORE;
            Traverse.Create<OilFloaterHighTempConfig>().Field<float>("CALORIES_PER_KG_OF_ORE").Value = CALORIES_PER_KG_OF_ORE;


        }
    }

    // Load translations files
    [HarmonyPatch(typeof(Localization))]
    [HarmonyPatch("Initialize")]
    class StringLocalisationPatch
    {
        public static void Postfix()
        {
            Utilities.LoadTranslations(typeof(PHO_STRINGS), Patches.modPath);
        }
    }

    // Load translations files
    [HarmonyPatch(typeof(BaseOilFloaterConfig))]
    [HarmonyPatch("SetupDiet")]
    class BiggerConsumptionRatePatch
    {
        public static void Postfix(GameObject __result)
        {
            GasAndLiquidConsumerMonitor.Def def = __result.AddOrGetDef<GasAndLiquidConsumerMonitor.Def>();
            if (def.consumptionRate <= 0.5f) def.consumptionRate = 3f;
        }
    }



    // add the OwO effect
    [HarmonyPatch(typeof(Db))]
    [HarmonyPatch("Initialize")]
    public class OwO_EffectPatch
    {
        public static void Postfix(Db __instance)
        {
            Utilities.AddWorldYaml("Asteroid_Slicksteria", typeof(PHO_STRINGS));


            Effect OwO_Effect = new Effect("OwO_effect", " OwO Effect", "This duplicant saw something so cute that he can't think of anything else.", 300f, true, true, false, null, 10f);
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
        }
    }

    [HarmonyPatch(typeof(Immigration))]
    [HarmonyPatch("ConfigureCarePackages")]
    public static class Immigration_ConfigureCarePackages_Patch
    {
        public static void Postfix(ref Immigration __instance)
        {
            Utilities.AddCarePackage(ref __instance, LeafyOilfloaterConfig.EGG_ID, 2f, () => Utilities.CycleInRange(100, 400) && Utilities.IsTagDiscovered("LeafyOilfloater"));
            Utilities.AddCarePackage(ref __instance, EthanolOilfloaterConfig.EGG_ID, 2f, () => Utilities.CycleInRange(100, 400) && Utilities.IsOilFieldDiscovered() && Utilities.IsSimHashesDiscovered(SimHashes.Ethanol) && Utilities.IsTagDiscovered("EthanolOilfloater"));
            Utilities.AddCarePackage(ref __instance, OwO_OilFloaterConfig.EGG_ID, 2f, () => Utilities.CycleInRange(200, 600) && Utilities.IsOilFieldDiscovered() && Utilities.IsTagDiscovered("OwO_Oilfloater"));
            Utilities.AddCarePackage(ref __instance, FrozenOilfloaterConfig.EGG_ID, 1f, () => Utilities.CycleCondition(600) && Utilities.IsOilFieldDiscovered() && Utilities.IsTagDiscovered("FrozenOilfloater"));
            Utilities.AddCarePackage(ref __instance, RobotOilfloaterConfig.EGG_ID, 1f, () => Utilities.CycleCondition(600) &&
                Utilities.IsOilFieldDiscovered() && Utilities.IsSimHashesDiscovered(SimHashes.Steel) && Utilities.IsTagDiscovered("RobotOilfloater"));
        }
    }
}
