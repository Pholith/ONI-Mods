using Klei.AI;
using System.Collections.Generic;
using UnityEngine;
using static TUNING.CREATURES;

namespace ILoveSlicksters
{
    public class RobotOilfloaterConfig : IEntityConfig
    {
        public GameObject CreatePrefab()
        {
            GameObject gameObject = CreateOilfloater(ID, StringsPatch.VARIANT_ROBOT.NAME, StringsPatch.VARIANT_ROBOT.DESC, "custom_oilfloater_kanim", false);

            EntityTemplates.ExtendEntityToFertileCreature(
                gameObject, 
                EGG_ID, 
                StringsPatch.VARIANT_ROBOT.EGG_NAME, 
                StringsPatch.VARIANT_ROBOT.DESC, 
                "egg_oilfloater_kanim", 
                OilFloaterTuning.EGG_MASS,
                "RobotOilfloaterBaby",
                60.0000038f, 20f,
                EGG_CHANCES_ROBOT, 
                EGG_SORT_ORDER);

            return gameObject;
        }

        public static GameObject CreateOilfloater(string id, string name, string desc, string anim_file, bool is_baby)
        {
            GameObject prefab = BaseOilFloaterConfig.BaseOilFloater(id, name, desc, anim_file, BASE_TRAIT_ID, 523.15f, 743.15f, is_baby, "hot_");
            EntityTemplates.ExtendEntityToWildCreature(prefab, OilFloaterTuning.PEN_SIZE_PER_CREATURE, LIFESPAN.TIER3);
            Trait trait = Db.Get().CreateTrait(BASE_TRAIT_ID, name, name, null, false, null, true, true);
            trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, OilFloaterTuning.STANDARD_STOMACH_SIZE, name, false, false, true));
            trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -OilFloaterTuning.STANDARD_CALORIES_PER_CYCLE / 600f, name, false, false, true));
            trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, HITPOINTS.TIER1, name, false, false, true));
            trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, LIFESPAN.TIER3, name, false, false, true));
            return BaseOilFloaterConfig.SetupDiet(prefab, CONSUME_ELEMENT.CreateTag(), EMIT_ELEMENT.CreateTag(), CALORIES_PER_KG_OF_ORE, CONVERSION_EFFICIENCY.NORMAL, null, 0f, MIN_POOP_SIZE_IN_KG);
        }

        public void OnPrefabInit(GameObject inst)
        {
        }

        public void OnSpawn(GameObject inst)
        {
        }

        public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_ROBOT = new List<FertilityMonitor.BreedingChance>
        {
            new FertilityMonitor.BreedingChance
            {
                egg = "OilfloaterHighTempEgg".ToTag(),
                weight = 0.33f
            },
            new FertilityMonitor.BreedingChance
            {
                egg = "RobotOilfloaterEgg".ToTag(),
                weight = 0.66f
            },
            new FertilityMonitor.BreedingChance
            {
                egg = "OilfloaterEgg".ToTag(),
                weight = 0.02f
            }
        };


        public const string ID = "RobotOilfloater";

        public const string BASE_TRAIT_ID = "RobotOilfloaterBaseTrait";

        public const string EGG_ID = "RobotOilfloaterEgg";

        public const SimHashes CONSUME_ELEMENT = SimHashes.CarbonDioxide;

        public const SimHashes EMIT_ELEMENT = SimHashes.MoltenSteel;

        private static float KG_ORE_EATEN_PER_CYCLE = PHO_TUNING.OILFLOATER.KG_ORE_EATEN_PER_CYCLE.HIGH2;

        private static float CALORIES_PER_KG_OF_ORE = OilFloaterTuning.STANDARD_CALORIES_PER_CYCLE / KG_ORE_EATEN_PER_CYCLE;

        private static float MIN_POOP_SIZE_IN_KG = 0.5f;

        public static int EGG_SORT_ORDER = OilFloaterConfig.EGG_SORT_ORDER + 2;

    }
}
