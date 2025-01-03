﻿using Klei.AI;
using Pholib;
using System.Collections.Generic;
using UnityEngine;
using static TUNING.CREATURES;

namespace ILoveSlicksters
{
    public class LeafyOilfloaterConfig : IEntityConfig
    {
        public GameObject CreatePrefab()
        {
            GameObject gameObject = CreateOilfloater(ID, PHO_STRINGS.VARIANT_LEAFY.NAME, PHO_STRINGS.VARIANT_LEAFY.DESC, base_kanim_id, false);

            gameObject.AddOrGetDef<CreatureLightMonitor.Def>();

            EntityTemplates.ExtendEntityToFertileCreature(
                gameObject, 
                EGG_ID, 
                PHO_STRINGS.VARIANT_LEAFY.EGG_NAME, 
                PHO_STRINGS.VARIANT_LEAFY.DESC,
                egg_kanim_id, 
                OilFloaterTuning.EGG_MASS,
                ID + "Baby",
                40f, 10f, 
                EGG_CHANCES_LEAFY, new string[] { "" },
                EGG_SORT_ORDER);

            return gameObject;
        }

        public static GameObject CreateOilfloater(string id, string name, string desc, string anim_file, bool is_baby)
        {
            GameObject prefab = BaseOilFloaterConfig.BaseOilFloater(id, name, desc, anim_file, BASE_TRAIT_ID,  12f.CelciusToKelvin(), 35f.CelciusToKelvin(), 1f.CelciusToKelvin(), 45f.CelciusToKelvin(), is_baby, variantSprite);
            EntityTemplates.ExtendEntityToWildCreature(prefab, OilFloaterTuning.PEN_SIZE_PER_CREATURE);
            int count = 3;
            string[] loot = new string[count];
            for (int i = 0; i < count; i++)
            {
                loot[i] = TUNING.FOOD.FOOD_TYPES.PRICKLEFRUIT.Id;
            }
            prefab.AddOrGet<Butcherable>().SetDrops(loot);
            prefab.AddOrGet<Navigator>().defaultSpeed = 1.5f;
            DiseaseDropper.Def def = prefab.AddOrGetDef<DiseaseDropper.Def>();
            def.diseaseIdx = Db.Get().Diseases.GetIndex(Db.Get().Diseases.PollenGerms.id);
            def.emitFrequency = 1f;
            def.averageEmitPerSecond = 1000;
            def.singleEmitQuantity = 100000;
            Trait trait = Db.Get().CreateTrait(BASE_TRAIT_ID, name, name, null, false, null, true, true);
            trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, OilFloaterTuning.STANDARD_STOMACH_SIZE, name, false, false, true));
            trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -OilFloaterTuning.STANDARD_CALORIES_PER_CYCLE / 600f, name, false, false, true));
            trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, HITPOINTS.TIER1, name, false, false, true));
            trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, LIFESPAN.TIER2, name, false, false, true));
            List<Diet.Info> diet_infos = DietInfo(GameTags.Steel, CALORIES_PER_KG_OF_ORE, CONVERSION_EFFICIENCY.GOOD_2, null, 0f);
            return OilFloaters.SetupDiet(prefab, diet_infos, CALORIES_PER_KG_OF_ORE, MIN_POOP_SIZE_IN_KG, 5 * ILoveSlicksters.Settings.ConsumptionMultiplier);
        }
        public string[] GetDlcIds()
        {
            return DlcManager.AVAILABLE_ALL_VERSIONS;
        }

        public static List<Diet.Info> DietInfo(Tag poopTag, float caloriesPerKg, float producedConversionRate, string diseaseId, float diseasePerKgProduced)
        {
            return new List<Diet.Info>
            {
                new Diet.Info(new HashSet<Tag>(new Tag[]
                {
                    SimHashes.CarbonDioxide.CreateTag()
                }), SimHashes.Oxygen.CreateTag(), caloriesPerKg, producedConversionRate, diseaseId, diseasePerKgProduced, false, Diet.Info.FoodType.EatSolid, false),
                new Diet.Info(new HashSet<Tag>(new Tag[]
                {
                    SimHashes.ChlorineGas.CreateTag()
                }), SimHashes.Algae.CreateTag(), caloriesPerKg / 2, producedConversionRate, diseaseId, diseasePerKgProduced, false, Diet.Info.FoodType.EatSolid, false),
                new Diet.Info(new HashSet<Tag>(new Tag[]
                {
                    SimHashes.ContaminatedOxygen.CreateTag()
                }), SimHashes.Phosphorite.CreateTag(), caloriesPerKg / 2, producedConversionRate, diseaseId, diseasePerKgProduced, false, Diet.Info.FoodType.EatSolid, false),
                new Diet.Info(new HashSet<Tag>(new Tag[]
                {
                    SimHashes.Methane.CreateTag()
                }), SimHashes.Carbon.CreateTag(), caloriesPerKg / 2, producedConversionRate, diseaseId, diseasePerKgProduced, false, Diet.Info.FoodType.EatSolid, false)
            };
        }


        public void OnPrefabInit(GameObject inst)
        {
        }

        public void OnSpawn(GameObject inst)
        {
        }

        public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_LEAFY = new List<FertilityMonitor.BreedingChance>
        {
            new FertilityMonitor.BreedingChance
            {
                egg = EGG_ID.ToTag(),
                weight = 0.66f
            },
            new FertilityMonitor.BreedingChance
            {
                egg = OilFloaterConfig.EGG_ID.ToTag(),
                weight = 0.34f
            },
            new FertilityMonitor.BreedingChance
            {
                egg = EthanolOilfloaterConfig.EGG_ID.ToTag(),
                weight = 0.02f
            },
            new FertilityMonitor.BreedingChance
            {
                egg = OwO_OilfloaterConfig.EGG_ID.ToTag(),
                weight = 0.02f
            }
        };

        public const string base_kanim_id = "custom_oilfloater2_kanim";
        public const string egg_kanim_id = "custom_egg_oilfloater2_kanim";
        public const string variantSprite = "hot_";


        public const string ID = "LeafyOilfloater";

        public const string BASE_TRAIT_ID = "LeafyOilfloaterBaseTrait";

        public const string EGG_ID = ID + "Egg";

        private static float KG_ORE_EATEN_PER_CYCLE = PHO_TUNING.OILFLOATER.KG_ORE_EATEN_PER_CYCLE.HIGH * ILoveSlicksters.Settings.ConsumptionMultiplier;

        private static float CALORIES_PER_KG_OF_ORE = PHO_TUNING.OILFLOATER.STANDARD_CALORIES_PER_CYCLE / KG_ORE_EATEN_PER_CYCLE;

        private static float MIN_POOP_SIZE_IN_KG = 0.3f;

        public static int EGG_SORT_ORDER = OilFloaterConfig.EGG_SORT_ORDER + 2;

    }
}
