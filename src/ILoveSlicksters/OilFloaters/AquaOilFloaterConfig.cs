using Klei.AI;
using System.Collections.Generic;
using UnityEngine;
using static TUNING.CREATURES;

namespace ILoveSlicksters
{
    public class AquaOilfloaterConfig : IEntityConfig
    {
        public GameObject CreatePrefab()
        {
            GameObject gameObject = CreateOilfloater(ID, PHO_STRINGS.VARIANT_AQUA.NAME, PHO_STRINGS.VARIANT_AQUA.DESC, base_kanim_id, false);

            EntityTemplates.ExtendEntityToFertileCreature(
                gameObject,
                EGG_ID,
                PHO_STRINGS.VARIANT_AQUA.EGG_NAME,
                PHO_STRINGS.VARIANT_AQUA.DESC,
                egg_kanim_id,
                OilFloaterTuning.EGG_MASS,
                ID + "Baby",
                60.0000038f, 20f,
                EGG_CHANCES_AQUA,
                EGG_SORT_ORDER);

            return gameObject;
        }

        public static GameObject CreateOilfloater(string id, string name, string desc, string anim_file, bool is_baby)
        {
            GameObject prefab = AquaticOilFloater(id, name, desc, anim_file, BASE_TRAIT_ID, 263.15f, 323.15f, is_baby, variantSprite);
            //prefab.RemoveDef<SubmergedMonitor.Def>();
            //prefab.AddOrGetDef<CreatureFallMonitor.Def>().canSwim = false;

            EntityTemplates.ExtendEntityToWildCreature(prefab, OilFloaterTuning.PEN_SIZE_PER_CREATURE, LIFESPAN.TIER3);
            Trait trait = Db.Get().CreateTrait(BASE_TRAIT_ID, name, name, null, false, null, true, true);
            trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, OilFloaterTuning.STANDARD_STOMACH_SIZE, name, false, false, true));
            trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -OilFloaterTuning.STANDARD_CALORIES_PER_CYCLE / 600f, name, false, false, true));
            trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, HITPOINTS.TIER1, name, false, false, true));
            trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, LIFESPAN.TIER3, name, false, false, true));
            List<Diet.Info> diet_infos = DietInfo(GameTags.AnyWater, CALORIES_PER_KG_OF_ORE, CONVERSION_EFFICIENCY.GOOD_1, null, 0f);
            return OilFloaters.SetupDiet(prefab, diet_infos, CALORIES_PER_KG_OF_ORE, MIN_POOP_SIZE_IN_KG, 15f * Patches.Settings.ConsumptionMultiplier);

        }
        public static List<Diet.Info> DietInfo(Tag poopTag, float caloriesPerKg, float producedConversionRate, string diseaseId, float diseasePerKgProduced)
        {
            return new List<Diet.Info>
            {
                new Diet.Info(new HashSet<Tag>(new Tag[]
                {
                    SimHashes.Water.CreateTag()
                }), SimHashes.Methane.CreateTag(), caloriesPerKg * 10, producedConversionRate, diseaseId, diseasePerKgProduced, false, false),
                new Diet.Info(new HashSet<Tag>(new Tag[]
                {
                    SimHashes.SaltWater.CreateTag()
                }), SimHashes.SedimentaryRock.CreateTag(), caloriesPerKg / 2, producedConversionRate, diseaseId, diseasePerKgProduced, false, false),
                new Diet.Info(new HashSet<Tag>(new Tag[]
                {
                    SimHashes.DirtyWater.CreateTag()
                }), SimHashes.Clay.CreateTag(), caloriesPerKg, producedConversionRate, diseaseId, diseasePerKgProduced, false, false),
                new Diet.Info(new HashSet<Tag>(new Tag[]
                {
                    Antigel.SimHash.CreateTag()
                }), SimHashes.Wolframite.CreateTag(), caloriesPerKg, producedConversionRate, diseaseId, diseasePerKgProduced, false, false)

            };
        }
        public static GameObject AquaticOilFloater(string id, string name, string desc, string anim_file, string traitId, float warnLowTemp, float warnHighTemp, bool is_baby, string symbolOverridePrefix = null)
        {
            float mass = 50f;
            EffectorValues tier = TUNING.DECOR.BONUS.TIER1;
            GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, desc, mass, Assets.GetAnim(anim_file), "idle_loop", Grid.SceneLayer.Creatures, 1, 1, tier, default(EffectorValues), SimHashes.Creature, null, (warnLowTemp + warnHighTemp) / 2f);
            gameObject.GetComponent<KPrefabID>().AddTag(GameTags.Creatures.Swimmer);
            gameObject.GetComponent<KPrefabID>().AddTag(GameTags.SwimmingCreature);

            EntityTemplates.ExtendEntityToBasicCreature(gameObject, FactionManager.FactionID.Pest, traitId, "SwimmerNavGrid", NavType.Swim, 32, 3f, TUNING.FOOD.FOOD_TYPES.FISH_MEAT.Id, 2, false, false, warnLowTemp, warnHighTemp, warnLowTemp - 15f, warnHighTemp + 20f);
            if (!string.IsNullOrEmpty(symbolOverridePrefix))
            {
                gameObject.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByAffix(Assets.GetAnim(anim_file), symbolOverridePrefix, null, 0);
            }
            gameObject.AddOrGet<ElementVulnerable>();
            gameObject.AddOrGet<LoopingSounds>();
            gameObject.AddOrGetDef<ThreatMonitor.Def>();
            gameObject.AddOrGet<GasDrowningMonitor>();
            gameObject.AddOrGetDef<CreatureAquaticGroomingMonitor.Def>();

            gameObject.AddOrGetDef<CreatureFallMonitor.Def>().canSwim = true;
            gameObject.AddWeapon(1f, 1f, AttackProperties.DamageType.Standard, AttackProperties.TargetType.Single, 1, 0f);
            EntityTemplates.CreateAndRegisterBaggedCreature(gameObject, false, true, false);
            string inhaleSound = "OilFloater_intake_air";
            if (is_baby)
            {
                inhaleSound = "OilFloaterBaby_intake_air";
            }
            ChoreTable.Builder chore_table = new ChoreTable.Builder().Add(new DeathStates.Def()).Add(new AnimInterruptStates.Def())
                .Add(new GrowUpStates.Def()).Add(new TrappedStates.Def()).Add(new IncubatingStates.Def())
                .Add(new BaggedStates.Def()).Add(new FallStates.Def()).Add(new StunnedStates.Def())
                /*.Add(new DrowningStates.Def())*/.Add(new DebugGoToStates.Def()).PushInterruptGroup()//.Add(new GasDrowningStates.Def())
                .Add(new CreatureSleepStates.Def()).Add(new FixedCaptureStates.Def())
                .Add(new RanchedStates.Def()).Add(new LayEggStates.Def()).Add(new InhaleStates.Def
                {
                    inhaleSound = inhaleSound
                }).Add(new SameSpotPoopStates.Def()).Add(new CallAdultStates.Def()).PopInterruptGroup().Add(new IdleStates.Def());

            EntityTemplates.AddCreatureBrain(gameObject, chore_table, GameTags.Creatures.Species.OilFloaterSpecies, symbolOverridePrefix);
            //string sound = "OilFloater_move_LP";
            //if (is_baby)
            //{
            //    sound = "OilFloaterBaby_move_LP";
            //}
            //gameObject.AddOrGet<OilFloaterMovementSound>().sound = sound;
            return gameObject;
        }

        public void OnPrefabInit(GameObject inst)
        {
        }

        public void OnSpawn(GameObject inst)
        {

        }

        public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_AQUA = new List<FertilityMonitor.BreedingChance>
        {
            new FertilityMonitor.BreedingChance
            {
                egg = "AquaOilfloaterEgg".ToTag(),
                weight = 0.66f
            },
            new FertilityMonitor.BreedingChance
            {
                egg = "EthanolOilfloaterEgg".ToTag(),
                weight = 0.33f
            }
        };
        public const string base_kanim_id = "custom_oilfloater_kanim";
        public const string egg_kanim_id = "custom_egg_oilfloater_kanim";
        public const string variantSprite = "oxy_";


        public const string ID = "AquaOilfloater";

        public const string BASE_TRAIT_ID = "AquaOilfloaterBaseTrait";

        public const string EGG_ID = "AquaOilfloaterEgg";

        private static float KG_ORE_EATEN_PER_CYCLE = PHO_TUNING.OILFLOATER.KG_ORE_EATEN_PER_CYCLE.MEGA * Patches.Settings.ConsumptionMultiplier;

        private static float CALORIES_PER_KG_OF_ORE = PHO_TUNING.OILFLOATER.STANDARD_CALORIES_PER_CYCLE / KG_ORE_EATEN_PER_CYCLE;

        private static float MIN_POOP_SIZE_IN_KG = 0.5f;

        public static int EGG_SORT_ORDER = OilFloaterConfig.EGG_SORT_ORDER + 2;

    }
}
