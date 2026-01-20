using Klei.AI;
using Pholib;
using System.Collections.Generic;
using UnityEngine;
using UtilLibs;
using static TUNING.CREATURES;

namespace ILoveSlicksters
{
    [EntityConfigOrder(2)]
    public class RobotOilfloaterConfig : IEntityConfig, IHasDlcRestrictions
    {
        public GameObject CreatePrefab()
        {
            GameObject gameObject = CreateOilfloater(ID, PHO_STRINGS.VARIANT_ROBOT.NAME, PHO_STRINGS.VARIANT_ROBOT.DESC, base_kanim_id, false);

            EntityTemplates.ExtendEntityToFertileCreature(
                gameObject, this as IHasDlcRestrictions,
                EGG_ID,
                PHO_STRINGS.VARIANT_ROBOT.EGG_NAME,
                PHO_STRINGS.VARIANT_ROBOT.DESC,
                egg_kanim_id,
                OilFloaterTuning.EGG_MASS,
                ID + "Baby",
                55, 20 + 5,
                EGG_CHANCES_ROBOT,
                EGG_SORT_ORDER);

            return gameObject;
        }
        public string[] GetDlcIds()
        {
            return DlcManager.AVAILABLE_ALL_VERSIONS;
        }
        public string[] GetAnyRequiredDlcIds()
        {
            return null;
        }

        public string[] GetRequiredDlcIds()
        {
            return new string[0];
        }

        public string[] GetForbiddenDlcIds()
        {
            return new string[0];
        }
        public static GameObject CreateOilfloater(string id, string name, string desc, string anim_file, bool is_baby)
        {
            ///"anim" - kanim should be the vanilla one, except for babies atm
            string vanillaAnimationKanimId = is_baby ? anim_file : "oilfloater_kanim";
            string symbol_override_prefix = is_baby ? "hot_" : variantSprite;


            GameObject prefab = BaseOilFloaterConfig.BaseOilFloater(id, name, desc, anim_file, BASE_TRAIT_ID, 230f.CelciusToKelvin(), 500f.CelciusToKelvin(), 200f.CelciusToKelvin(), 600f.CelciusToKelvin(), is_baby, symbol_override_prefix);

            GameObject wildCreature = EntityTemplates.ExtendEntityToWildCreature(prefab, OilFloaterTuning.PEN_SIZE_PER_CREATURE);

            if (is_baby == false)
                NewCritterSystemUtility.FixCritterAnimationOverrides(wildCreature, anim_file, vanillaAnimationKanimId, symbol_override_prefix);


            int count = (int)prefab.AddOrGet<PrimaryElement>().Mass;

            string[] loot = new string[count];
            for (int i = 0; i < count; i++)
            {
                loot[i] = "Steel";
            }
            prefab.AddOrGet<Butcherable>().SetDrops(loot);

            if (!ILoveSlicksters.Settings.DisableRobotSlicksterSpore)
            {
                DiseaseDropper.Def def = prefab.AddOrGetDef<DiseaseDropper.Def>();
                def.diseaseIdx = Db.Get().Diseases.GetIndex(Db.Get().Diseases.ZombieSpores.id);
                def.emitFrequency = 1f;
                def.averageEmitPerSecond = 5;
                def.singleEmitQuantity = 50;
            }


            Trait trait = Db.Get().CreateTrait(BASE_TRAIT_ID, name, name, null, false, null, true, true);
            trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, OilFloaterTuning.STANDARD_STOMACH_SIZE, name, false, false, true));
            trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -OilFloaterTuning.STANDARD_CALORIES_PER_CYCLE / 600f, name, false, false, true));
            trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, HITPOINTS.TIER2, name, false, false, true));
            trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, LIFESPAN.TIER3, name, false, false, true));
            List<Diet.Info> diet_infos = DietInfo(GameTags.Steel, CALORIES_PER_KG_OF_ORE, CONVERSION_EFFICIENCY.GOOD_1, null, 0f);
            return OilFloaters.SetupDiet(prefab, diet_infos, CALORIES_PER_KG_OF_ORE, MIN_POOP_SIZE_IN_KG, 5 * ILoveSlicksters.Settings.ConsumptionMultiplier);
        }
        public static List<Diet.Info> DietInfo(Tag poopTag, float caloriesPerKg, float producedConversionRate, string diseaseId, float diseasePerKgProduced)
        {
            HashSet<Tag> hashSet = new HashSet<Tag>
            {
                SimHashes.Steam.CreateTag(),
                SimHashes.CarbonDioxide.CreateTag(),
                SimHashes.SourGas.CreateTag()
            };
            return new List<Diet.Info>
        {
            new Diet.Info(hashSet, poopTag, caloriesPerKg, producedConversionRate, diseaseId, diseasePerKgProduced, false, Diet.Info.FoodType.EatSolid, false)
        };
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
                egg = OilFloaterHighTempConfig.EGG_ID.ToTag(),
                weight = 0.33f
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
            }
        };
        public const string base_kanim_id = "custom_oilfloater_kanim";
        public const string egg_kanim_id = "custom_egg_oilfloater_kanim";
        public const string variantSprite = "robo_";


        public const string ID = "RobotOilfloater";

        public const string BASE_TRAIT_ID = "RobotOilfloaterBaseTrait";

        public const string EGG_ID = ID+"Egg";

        private static float KG_ORE_EATEN_PER_CYCLE = PHO_TUNING.OILFLOATER.KG_ORE_EATEN_PER_CYCLE.HIGH2 * ILoveSlicksters.Settings.ConsumptionMultiplier;

        private static float CALORIES_PER_KG_OF_ORE = PHO_TUNING.OILFLOATER.STANDARD_CALORIES_PER_CYCLE / KG_ORE_EATEN_PER_CYCLE;

        private static float MIN_POOP_SIZE_IN_KG = 0.5f;

        public static int EGG_SORT_ORDER = OilFloaterConfig.EGG_SORT_ORDER + 2;

    }
}
