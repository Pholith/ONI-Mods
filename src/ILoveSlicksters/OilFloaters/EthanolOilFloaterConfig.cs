using Klei.AI;
using System.Collections.Generic;
using UnityEngine;
using static TUNING.CREATURES;
using Pholib;

namespace ILoveSlicksters
{
    public class EthanolOilfloaterConfig : IEntityConfig, IHasDlcRestrictions
    {

        public string[] GetDlcIds()
        {
            return DlcManager.AVAILABLE_ALL_VERSIONS;
        }

        public string[] GetRequiredDlcIds()
        {
            return new string[0];
        }

        public string[] GetForbiddenDlcIds()
        {
            return new string[0];
        }

        public GameObject CreatePrefab()
        {
            GameObject gameObject = CreateOilfloater(ID, PHO_STRINGS.VARIANT_ETHANOL.NAME, PHO_STRINGS.VARIANT_ETHANOL.DESC, base_kanim_id, false);
            EntityTemplates.ExtendEntityToFertileCreature(
                gameObject, this as IHasDlcRestrictions,
                EGG_ID,
                PHO_STRINGS.VARIANT_ETHANOL.EGG_NAME,
                PHO_STRINGS.VARIANT_ETHANOL.DESC,
                egg_kanim_id,
                OilFloaterTuning.EGG_MASS,
                ID + "Baby",
                45f, 20,
                EGG_CHANCES_ETHANOL,
                EGG_SORT_ORDER,
                true, false);

            return gameObject;
        }

        public static GameObject CreateOilfloater(string id, string name, string desc, string anim_file, bool is_baby)
        {
            GameObject prefab = BaseOilFloaterConfig.BaseOilFloater(id, name, desc, anim_file, BASE_TRAIT_ID, (-10f).CelciusToKelvin(), 10f.CelciusToKelvin(), (-30f).CelciusToKelvin(), 20f.CelciusToKelvin(), is_baby, variantSprite);
            EntityTemplates.ExtendEntityToWildCreature(prefab, OilFloaterTuning.PEN_SIZE_PER_CREATURE);
            Trait trait = Db.Get().CreateTrait(BASE_TRAIT_ID, name, name, null, false, null, true, true);
            trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, OilFloaterTuning.STANDARD_STOMACH_SIZE, name, false, false, true));
            trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -OilFloaterTuning.STANDARD_CALORIES_PER_CYCLE / 600f, name, false, false, true));
            trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, HITPOINTS.TIER1, name, false, false, true));
            trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, LIFESPAN.TIER2, name, false, false, true));
            return BaseOilFloaterConfig.SetupDiet(prefab, CONSUME_ELEMENT.CreateTag(), EMIT_ELEMENT.CreateTag(), CALORIES_PER_KG_OF_ORE,
                CONVERSION_EFFICIENCY.GOOD_2, null, 0f, MIN_POOP_SIZE_IN_KG);
        }

        public void OnPrefabInit(GameObject inst)
        {
        }

        public void OnSpawn(GameObject inst)
        {
        }
        public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_ETHANOL = new List<FertilityMonitor.BreedingChance>
        {
            new FertilityMonitor.BreedingChance
            {
                egg = OilFloaterDecorConfig.EGG_ID.ToTag(),
                weight = 0.1f
            },
            new FertilityMonitor.BreedingChance
            {
                egg = EGG_ID.ToTag(),
                weight = 0.66f
            },
            new FertilityMonitor.BreedingChance
            {
                egg = OilFloaterConfig.EGG_ID.ToTag(),
                weight = 0.02f
            },
            new FertilityMonitor.BreedingChance
            {
                egg = FrozenOilfloaterConfig.EGG_ID.ToTag(),
                weight = 0.02f
            },
            new FertilityMonitor.BreedingChance
            {
                egg = AquaOilfloaterConfig.EGG_ID.ToTag(),
                weight = 0.02f
            },
            new FertilityMonitor.BreedingChance
            {
                egg = OwO_OilfloaterConfig.EGG_ID.ToTag(),
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

        private static float KG_ORE_EATEN_PER_CYCLE = PHO_TUNING.OILFLOATER.KG_ORE_EATEN_PER_CYCLE.HIGH2 * ILoveSlicksters.Settings.ConsumptionMultiplier;

        private static float CALORIES_PER_KG_OF_ORE = PHO_TUNING.OILFLOATER.STANDARD_CALORIES_PER_CYCLE / KG_ORE_EATEN_PER_CYCLE;

        private static float MIN_POOP_SIZE_IN_KG = 0.5f;

        public static int EGG_SORT_ORDER = OilFloaterConfig.EGG_SORT_ORDER + 2;

    }
}
