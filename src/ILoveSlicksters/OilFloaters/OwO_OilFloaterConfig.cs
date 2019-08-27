using Harmony;
using Klei.AI;
using System.Collections.Generic;
using UnityEngine;
using static StateMachine;
using static TUNING.CREATURES;

namespace ILoveSlicksters
{
    class OwO_OilFloaterConfig : IEntityConfig
    {

        public GameObject CreatePrefab()
        {
            GameObject gameObject = CreateOilFloater(ID, StringsPatch.VARIANT_OWO.NAME, StringsPatch.VARIANT_OWO.DESC, "custom_oilfloater_anim", false);

            DecorProvider decorProvider = gameObject.AddOrGet<DecorProvider>();
            decorProvider.SetValues(TUNING.DECOR.BONUS.TIER5);


            EffectArea owoEffect = gameObject.AddComponent<EffectArea>();
            owoEffect.EffectName = "OwO_effect";
            owoEffect.Area = 5;


            EntityTemplates.ExtendEntityToFertileCreature(gameObject, 
                EGG_ID, 
                StringsPatch.VARIANT_OWO.EGG_NAME, 
                StringsPatch.VARIANT_OWO.DESC, 
                "egg_oilfloater", 
                OilFloaterTuning.EGG_MASS, 
                "OwO_OilfloaterBaby", 
                60.0000038f, 20f, 
                EGG_CHANCES_OWO, 
                EGG_SORT_ORDER, 
                true, false, true, 1f);


            return gameObject;
        }

        public static GameObject CreateOilFloater(string id, string name, string desc, string anim_file, bool is_baby)
        {
            GameObject prefab = BaseOilFloaterConfig.BaseOilFloater(id, name, desc, anim_file, BASE_TRAIT_ID, 263.15f, 313.15f, is_baby, null);
            EntityTemplates.ExtendEntityToWildCreature(prefab, OilFloaterTuning.PEN_SIZE_PER_CREATURE, LIFESPAN.TIER3);
            Trait trait = Db.Get().CreateTrait(BASE_TRAIT_ID, name, name, null, false, null, true, true);
            trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, OilFloaterTuning.STANDARD_STOMACH_SIZE, name, false, false, true));
            trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -OilFloaterTuning.STANDARD_CALORIES_PER_CYCLE / 600f, name, false, false, true));
            trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, HITPOINTS.TIER1, name, false, false, true));
            trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, LIFESPAN.TIER3, name, false, false, true));
            return BaseOilFloaterConfig.SetupDiet(prefab, CONSUME_ELEMENT.CreateTag(), EMIT_ELEMENT.CreateTag(), CALORIES_PER_KG_OF_ORE, TUNING.CREATURES.CONVERSION_EFFICIENCY.NORMAL, null, 0f, MIN_POOP_SIZE_IN_KG);
        }

        public void OnPrefabInit(GameObject inst)
        {
        }

        public void OnSpawn(GameObject inst)
        {
        }

        public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_OWO = new List<FertilityMonitor.BreedingChance>
        {
            new FertilityMonitor.BreedingChance
            {
                egg = "OilfloaterDecorEgg".ToTag(),
                weight = 0.33f
            },
            new FertilityMonitor.BreedingChance
            {
                egg = "OwO_OilfloaterEgg".ToTag(),
                weight = 0.66f
            },
            new FertilityMonitor.BreedingChance
            {
                egg = "OilfloaterEgg".ToTag(),
                weight = 0.02f
            }
        };

        public const string ID = "OwO_Oilfloater";

        public const string BASE_TRAIT_ID = "OwO_OilfloaterBaseTrait";

        public const string EGG_ID = "OwO_OilfloaterEgg";

        public const SimHashes CONSUME_ELEMENT = SimHashes.Oxygen;

        public const SimHashes EMIT_ELEMENT = SimHashes.LiquidHydrogen;

        private static float KG_ORE_EATEN_PER_CYCLE = 20f;

        private static float CALORIES_PER_KG_OF_ORE = OilFloaterTuning.STANDARD_CALORIES_PER_CYCLE / KG_ORE_EATEN_PER_CYCLE;

        private static float MIN_POOP_SIZE_IN_KG = 0.5f;

        public static int EGG_SORT_ORDER = OilFloaterConfig.EGG_SORT_ORDER + 2;

    }
}
