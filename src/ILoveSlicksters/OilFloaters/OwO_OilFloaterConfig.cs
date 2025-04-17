using Klei.AI;
using Pholib;
using System.Collections.Generic;
using TUNING;
using UnityEngine;
using static TUNING.CREATURES;

namespace ILoveSlicksters
{
    public class OwO_OilfloaterConfig : IEntityConfig, IHasDlcRestrictions
    {

        public GameObject CreatePrefab()
        {
            GameObject gameObject = CreateOilFloater(ID, PHO_STRINGS.VARIANT_OWO.NAME, PHO_STRINGS.VARIANT_OWO.DESC, base_kanim_id, false);

            DecorProvider decorProvider = gameObject.AddOrGet<DecorProvider>();
            decorProvider.SetValues(TUNING.DECOR.BONUS.TIER5);


            EffectArea owoEffect = gameObject.AddComponent<EffectArea>();
            owoEffect.EffectName = "OwOEffect";
            owoEffect.Area = 5;


            EntityTemplates.ExtendEntityToFertileCreature(
                gameObject, this as IHasDlcRestrictions,
                EGG_ID,
                PHO_STRINGS.VARIANT_OWO.EGG_NAME,
                PHO_STRINGS.VARIANT_OWO.DESC,
                egg_kanim_id,
                OilFloaterTuning.EGG_MASS,
                ID + "Baby",
                45, 20f,
                EGG_CHANCES_OWO,
                EGG_SORT_ORDER,
                true, false);
            return gameObject;
        }

        public static GameObject CreateOilFloater(string id, string name, string desc, string anim_file, bool is_baby)
        {
            GameObject prefab = BaseOilFloaterConfig.BaseOilFloater(id, name, desc, anim_file, BASE_TRAIT_ID, 0f.CelciusToKelvin(), 45f.CelciusToKelvin(), (-15f).CelciusToKelvin(), 60f.CelciusToKelvin(), is_baby, variantSprite);
            EntityTemplates.ExtendEntityToWildCreature(prefab, CREATURES.SPACE_REQUIREMENTS.TIER4);
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

        public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_OWO = new List<FertilityMonitor.BreedingChance>
        {
            new FertilityMonitor.BreedingChance
            {
                egg = OilFloaterDecorConfig.EGG_ID.ToTag(),
                weight = 0.01f
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
                egg = LeafyOilfloaterConfig.EGG_ID.ToTag(),
                weight = 0.02f
            },
            new FertilityMonitor.BreedingChance
            {
                egg = FrozenOilfloaterConfig.EGG_ID.ToTag(),
                weight = 0.02f
            },
            new FertilityMonitor.BreedingChance
            {
                egg = EthanolOilfloaterConfig.EGG_ID.ToTag(),
                weight = 0.02f
            },
            new FertilityMonitor.BreedingChance
            {
                egg = AquaOilfloaterConfig.EGG_ID.ToTag(),
                weight = 0.02f
            },

        };
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
        public const string base_kanim_id = "custom_oilfloater_kanim";
        public const string egg_kanim_id = "custom_egg_oilfloater_kanim";
        public const string variantSprite = null;


        public const string ID = "OwOOilfloater";

        public const string BASE_TRAIT_ID = OwO_OilfloaterConfig.ID+"BaseTrait";

        public const string EGG_ID = OwO_OilfloaterConfig.ID+"Egg";

        public const SimHashes CONSUME_ELEMENT = SimHashes.Oxygen;

        public const SimHashes EMIT_ELEMENT = SimHashes.Hydrogen;

        private static readonly float KG_ORE_EATEN_PER_CYCLE = PHO_TUNING.OILFLOATER.KG_ORE_EATEN_PER_CYCLE.HIGH2 * ILoveSlicksters.Settings.ConsumptionMultiplier;

        private static readonly float CALORIES_PER_KG_OF_ORE = PHO_TUNING.OILFLOATER.STANDARD_CALORIES_PER_CYCLE / KG_ORE_EATEN_PER_CYCLE;

        private static readonly float MIN_POOP_SIZE_IN_KG = 0.5f;

        public static int EGG_SORT_ORDER = OilFloaterConfig.EGG_SORT_ORDER + 2;

    }
}
