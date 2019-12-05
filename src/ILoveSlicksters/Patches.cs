using Harmony;
using TUNING;
using System;
using Klei.AI;
using UnityEngine;
using System.Reflection;
using static Pholib.Utilities;

namespace ILoveSlicksters
{

    class Patches
    {
        // Egg chance patches
        public static void OnLoad()
        {

            // Add the temperature modifier for ethanol oilfloater
            Type[] parameters_type = new Type[] { typeof(string), typeof(Tag), typeof(float), typeof(float), typeof(float), typeof(bool) };
            object[] paramaters = new object[] { "EthanolOilfloater", "EthanolOilfloaterEgg".ToTag(), 253.15f, 313.15f, PHO_TUNING.EGG_MODIFIER_PER_SECOND.NORMAL, false };

            CREATURES.EGG_CHANCE_MODIFIERS.MODIFIER_CREATORS.Add(
                Traverse.Create<CREATURES.EGG_CHANCE_MODIFIERS>().Method("CreateTemperatureModifier", parameters_type).GetValue<System.Action>(paramaters)
            );

            // Add the temperature modifier for robot oilfloater
            object[] paramaters2 = new object[] { "RobotOilfloater", "RobotOilfloaterEgg".ToTag(), 523.15f, 743.15f, PHO_TUNING.EGG_MODIFIER_PER_SECOND.NORMAL, false };

            CREATURES.EGG_CHANCE_MODIFIERS.MODIFIER_CREATORS.Add(
                Traverse.Create<CREATURES.EGG_CHANCE_MODIFIERS>().Method("CreateTemperatureModifier", parameters_type).GetValue<System.Action>(paramaters2)
            );

            // Add the temperature modifier for frozen oilfloater
            object[] paramaters3 = new object[] { "FrozenOilfloater", "FrozenOilfloaterEgg".ToTag(), 213.15f, 283.15f, PHO_TUNING.EGG_MODIFIER_PER_SECOND.NORMAL, false };

            CREATURES.EGG_CHANCE_MODIFIERS.MODIFIER_CREATORS.Add(
                Traverse.Create<CREATURES.EGG_CHANCE_MODIFIERS>().Method("CreateTemperatureModifier", parameters_type).GetValue<System.Action>(paramaters3)
            );

            // Add the light modifier for leafy oilfloater
            CREATURES.EGG_CHANCE_MODIFIERS.MODIFIER_CREATORS.Add(
                PHO_TUNING.CreateLightModifier("LeafyOilfloater", "LeafyOilfloaterEgg".ToTag(), PHO_TUNING.EGG_MODIFIER_PER_SECOND.NORMAL, true)
            );
            // Add the pressure modifier for leafy oilfloater
            //CREATURES.EGG_CHANCE_MODIFIERS.MODIFIER_CREATORS.Add(
            //    PHO_TUNING.CreatePressureModifier("LeafyOilfloater", "LeafyOilfloaterEgg".ToTag(), 500f, PHO_TUNING.EGG_MODIFIER_PER_SECOND.FAST, false)
            //);
            CREATURES.EGG_CHANCE_MODIFIERS.MODIFIER_CREATORS.Add(
                PHO_TUNING.CreateElementModifier("OwO_Oilfloater", "OwO_OilfloaterEgg".ToTag(), SimHashes.Hydrogen, PHO_TUNING.EGG_MODIFIER_PER_SECOND.FAST2, false)
            );



            // Add Leafy egg chance
            OilFloaterTuning.EGG_CHANCES_BASE.Add(
            new FertilityMonitor.BreedingChance
            {
                egg = "LeafyOilfloaterEgg".ToTag(),
                weight = 0.02f
            });
            // Add Robot egg chance
            OilFloaterTuning.EGG_CHANCES_HIGHTEMP.Add(
            new FertilityMonitor.BreedingChance
            {
                egg = "RobotOilfloaterEgg".ToTag(),
                weight = 0.02f
            });

            // replace the molten slicker egg chance to ethanol egg chance
            OilFloaterTuning.EGG_CHANCES_DECOR[1] = new FertilityMonitor.BreedingChance
            {
                egg = "EthanolOilfloaterEgg".ToTag(),
                weight = 0.02f
            };
            // Add the OwO slicker egg chance
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


    // add the OwO effect
    [HarmonyPatch(typeof(Db))]
    [HarmonyPatch("Initialize")]
    public class OwO_EffectPatch
    {
        public static void Postfix(Db __instance)
        {
            Pholib.Utilities.addWorldYaml(StringsPatch.WORLDGEN.NAME, StringsPatch.WORLDGEN.DESC, "Asteroid_Slicksteria", typeof(StringsPatch));


            Effect OwO_Effect = new Effect("OwO_effect", " OwO Effect", "This duplicant saw something so cute that he can't think of anything else.", 300f, true, true, false, null, 10f);
            OwO_Effect.Add(new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, - DUPLICANTSTATS.QOL_STRESS.BELOW_EXPECTATIONS.HARD, "OwO Effect"));
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
            AddCarePackage(ref __instance, EthanolOilfloaterBabyConfig.ID, 1f, () => CycleCondition(48));
            AddCarePackage(ref __instance, LeafyOilfloaterBabyConfig.ID, 1f, () => CycleCondition(48));
            AddCarePackage(ref __instance, OwO_OilFloaterBabyConfig.ID, 1f, () => CycleCondition(126));
            AddCarePackage(ref __instance, FrozenOilfloaterBabyConfig.ID, 1f, () => CycleCondition(126));
            AddCarePackage(ref __instance, RobotOilfloaterBabyConfig.ID, 1f, () => CycleCondition(256));

            AddCarePackage(ref __instance, EthanolOilfloaterConfig.EGG_ID, 3f, () => CycleCondition(48));
            AddCarePackage(ref __instance, LeafyOilfloaterConfig.EGG_ID, 3f, () => CycleCondition(48));
            AddCarePackage(ref __instance, OwO_OilFloaterConfig.EGG_ID, 2f, () => CycleCondition(48));
            AddCarePackage(ref __instance, FrozenOilfloaterConfig.EGG_ID, 1f, () => CycleCondition(96));
            AddCarePackage(ref __instance, RobotOilfloaterConfig.EGG_ID, 1f, () => CycleCondition(96));
        }
    }
}
