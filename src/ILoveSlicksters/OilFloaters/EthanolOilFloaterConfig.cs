using Klei.AI;
using System.Collections.Generic;
using UnityEngine;
using static TUNING.CREATURES;

namespace ILoveSlicksters
{
    public class EthanolOilfloaterConfig : IEntityConfig
    {
        public GameObject CreatePrefab()
        {
            GameObject gameObject = CreateOilfloater(ID, PHO_STRINGS.VARIANT_ETHANOL.NAME, PHO_STRINGS.VARIANT_ETHANOL.DESC, base_kanim_id, false);
            EntityTemplates.ExtendEntityToFertileCreature(gameObject,
                EGG_ID,
                PHO_STRINGS.VARIANT_ETHANOL.EGG_NAME,
                PHO_STRINGS.VARIANT_ETHANOL.DESC,
                egg_kanim_id,
                OilFloaterTuning.EGG_MASS,
                ID + "Baby",
                30f, 10f,
                EGG_CHANCES_ETHANOL,
                EGG_SORT_ORDER,
                true, false, true, 1f);

            return gameObject;
        }

        public static GameObject CreateOilfloater(string id, string name, string desc, string anim_file, bool is_baby)
        {
            GameObject prefab = BaseOilFloaterConfig.BaseOilFloater(id, name, desc, anim_file, BASE_TRAIT_ID, 243.15f, 293.15f, is_baby, variantSprite);
            EntityTemplates.ExtendEntityToWildCreature(prefab, OilFloaterTuning.PEN_SIZE_PER_CREATURE);
            Trait trait = Db.Get().CreateTrait(BASE_TRAIT_ID, name, name, null, false, null, true, true);
            trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, OilFloaterTuning.STANDARD_STOMACH_SIZE, name, false, false, true));
            trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -OilFloaterTuning.STANDARD_CALORIES_PER_CYCLE / 600f, name, false, false, true));
            trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, HITPOINTS.TIER1, name, false, false, true));
            trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, LIFESPAN.TIER2, name, false, false, true));
            return BaseOilFloaterConfig.SetupDiet(prefab, CONSUME_ELEMENT.CreateTag(), EMIT_ELEMENT.CreateTag(), CALORIES_PER_KG_OF_ORE, PHO_TUNING.CONVERSION_EFFICIENCY.NORMAL_LOW, null, 0f, MIN_POOP_SIZE_IN_KG);
        }

        public void OnPrefabInit(GameObject inst)
        {
        }

        public void OnSpawn(GameObject inst)
        {
        }
        public string GetDlcId()
        {
            return "";
        }

        public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_ETHANOL = new List<FertilityMonitor.BreedingChance>
        {
            new FertilityMonitor.BreedingChance
            {
                egg = "OilfloaterDecorEgg".ToTag(),
                weight = 0.33f
            },
            new FertilityMonitor.BreedingChance
            {
                egg = "EthanolOilfloaterEgg".ToTag(),
                weight = 0.66f
            },
            new FertilityMonitor.BreedingChance
            {
                egg = "OilfloaterEgg".ToTag(),
                weight = 0.02f
            },
            new FertilityMonitor.BreedingChance
            {
                egg = "FrozenOilfloaterEgg".ToTag(),
                weight = 0.02f
            },
            new FertilityMonitor.BreedingChance
            {
                egg = "AquaOilfloaterEgg".ToTag(),
                weight = 0.02f
            }

        };

        public const string base_kanim_id = "custom_oilfloater_kanim";
        public const string egg_kanim_id = "custom_egg_oilfloater_kanim";
        public const string variantSprite = "oxy_";


        public const string ID = "EthanolOilfloater";

        public const string BASE_TRAIT_ID = "EthanolOilfloaterBaseTrait";

        public const string EGG_ID = "EthanolOilfloaterEgg";

        public const SimHashes CONSUME_ELEMENT = SimHashes.CarbonDioxide;

        public const SimHashes EMIT_ELEMENT = SimHashes.Ethanol;

        private static float KG_ORE_EATEN_PER_CYCLE = PHO_TUNING.OILFLOATER.KG_ORE_EATEN_PER_CYCLE.HIGH2 * Patches.Settings.ConsumptionMultiplier;

        private static float CALORIES_PER_KG_OF_ORE = PHO_TUNING.OILFLOATER.STANDARD_CALORIES_PER_CYCLE / KG_ORE_EATEN_PER_CYCLE;

        private static float MIN_POOP_SIZE_IN_KG = 0.5f;

        public static int EGG_SORT_ORDER = OilFloaterConfig.EGG_SORT_ORDER + 2;

    }
}
