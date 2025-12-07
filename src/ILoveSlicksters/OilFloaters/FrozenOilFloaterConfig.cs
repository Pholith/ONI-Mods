using Klei.AI;
using Pholib;
using System.Collections.Generic;
using UnityEngine;
using static TUNING.CREATURES;

namespace ILoveSlicksters
{
    public class FrozenOilfloaterConfig : IEntityConfig, IHasDlcRestrictions
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
        public string[] GetAnyRequiredDlcIds()
        {
            return null;
        }

        public GameObject CreatePrefab()
        {
            GameObject gameObject = CreateOilfloater(ID, PHO_STRINGS.VARIANT_FROZEN.NAME, PHO_STRINGS.VARIANT_FROZEN.DESC, base_kanim_id, false);

            EntityTemplates.ExtendEntityToFertileCreature(
                gameObject, this,
                EGG_ID,
                PHO_STRINGS.VARIANT_FROZEN.EGG_NAME,
                PHO_STRINGS.VARIANT_FROZEN.DESC,
                egg_kanim_id,
                OilFloaterTuning.EGG_MASS,
                ID + "Baby",
                50, 20 + 5,
                EGG_CHANCES_FROZEN,
                EGG_SORT_ORDER);

            return gameObject;
        }

        public static GameObject CreateOilfloater(string id, string name, string desc, string anim_file, bool is_baby)
        {
            GameObject prefab = BaseOilFloaterConfig.BaseOilFloater(id, name, desc, anim_file, BASE_TRAIT_ID, (-50f).CelciusToKelvin(), (-20f).CelciusToKelvin(), (-90f).CelciusToKelvin(), 0f.CelciusToKelvin(), is_baby, variantSprite);
            EntityTemplates.ExtendEntityToWildCreature(prefab, OilFloaterTuning.PEN_SIZE_PER_CREATURE);
            Trait trait = Db.Get().CreateTrait(BASE_TRAIT_ID, name, name, null, false, null, true, true);
            trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, OilFloaterTuning.STANDARD_STOMACH_SIZE, name, false, false, true));
            trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -OilFloaterTuning.STANDARD_CALORIES_PER_CYCLE / 600f, name, false, false, true));
            trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, HITPOINTS.TIER1, name, false, false, true));
            trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, LIFESPAN.TIER3, name, false, false, true));
            List<Diet.Info> diet_infos = DietInfo(GameTags.Steel, CALORIES_PER_KG_OF_ORE, CONVERSION_EFFICIENCY.GOOD_2, null, 0f);

            if (!ILoveSlicksters.Settings.DisableFrozenSlicksterCold)
            {
                var primaryElement = prefab.GetComponent<PrimaryElement>();
                primaryElement.setTemperatureCallback = (PrimaryElement primary_element, float temperature) =>
                {
                    Debug.Assert(!float.IsNaN(temperature));
                    if (temperature <= 0f)
                    {
                        DebugUtil.LogErrorArgs(primary_element.gameObject, primary_element.gameObject.name + " has a temperature of zero which has always been an error in my experience.");
                    }

                    primary_element.Temperature = Mathf.Max((-40f).CelciusToKelvin(), temperature - ILoveSlicksters.Settings.FrozenSlicksterColdEffectDelta);
                };
            }
            return OilFloaters.SetupDiet(prefab, diet_infos, CALORIES_PER_KG_OF_ORE, MIN_POOP_SIZE_IN_KG, 5 * ILoveSlicksters.Settings.ConsumptionMultiplier);
        }


        public static List<Diet.Info> DietInfo(Tag poopTag, float caloriesPerKg, float producedConversionRate, string diseaseId, float diseasePerKgProduced)
        {
            return new List<Diet.Info>
            {
                new Diet.Info(new HashSet<Tag>(new Tag[]
                {
                    SimHashes.CarbonDioxide.CreateTag()
                }), Antigel.SimHash.CreateTag(), caloriesPerKg, producedConversionRate, diseaseId, diseasePerKgProduced, false, Diet.Info.FoodType.EatSolid, false),
                new Diet.Info(new HashSet<Tag>(new Tag[]
                {
                    SimHashes.Oxygen.CreateTag()
                }), SimHashes.OxyRock.CreateTag(), caloriesPerKg, producedConversionRate, diseaseId, diseasePerKgProduced, false, Diet.Info.FoodType.EatSolid, false)
            };
        }


        public void OnPrefabInit(GameObject inst)
        {
            if (!ILoveSlicksters.Settings.DisableFrozenSlicksterCold)
                inst.AddOrGet<EntityCellVisualizer>().AddPort(EntityCellVisualizer.Ports.HeatSink, default(CellOffset));
        }

        public void OnSpawn(GameObject inst)
        {
        }

        public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_FROZEN = new List<FertilityMonitor.BreedingChance>
        {
            new FertilityMonitor.BreedingChance
            {
                egg = EGG_ID.ToTag(),
                weight = 0.66f
            },
            new FertilityMonitor.BreedingChance
            {
                egg = EthanolOilfloaterConfig.EGG_ID.ToTag(),
                weight = 0.34f
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
            },
        };

        public const string base_kanim_id = "custom_oilfloater2_kanim";
        public const string egg_kanim_id = "custom_egg_oilfloater2_kanim";
        public const string variantSprite = "oxy_";


        public const string ID = "FrozenOilfloater";

        public const string BASE_TRAIT_ID = "FrozenOilfloaterBaseTrait";

        public const string EGG_ID = "FrozenOilfloaterEgg";

        private static float KG_ORE_EATEN_PER_CYCLE = PHO_TUNING.OILFLOATER.KG_ORE_EATEN_PER_CYCLE.HIGH * ILoveSlicksters.Settings.ConsumptionMultiplier;

        private static float CALORIES_PER_KG_OF_ORE = PHO_TUNING.OILFLOATER.STANDARD_CALORIES_PER_CYCLE / KG_ORE_EATEN_PER_CYCLE;

        private static float MIN_POOP_SIZE_IN_KG = 0.5f;

        public static int EGG_SORT_ORDER = OilFloaterConfig.EGG_SORT_ORDER + 2;

    }
}
