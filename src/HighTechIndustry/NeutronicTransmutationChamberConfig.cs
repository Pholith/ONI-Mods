using Klei.AI;
using Pholib;
using System.Collections.Generic;
using TUNING;
using UnityEngine;
using static ComplexRecipe;

namespace HighTechIndustry
{
    public class NeutronicTransmutationChamberConfig : IBuildingConfig
    {
        public override string[] GetRequiredDlcIds()
        {
            return DlcManager.EXPANSION1;
        }

        public const string ID = "NeutronicTransmutationChamber";
        public const string HEP_STORAGE_ID = "HEP_STORAGE";
        public const string OPERATING_PORT_ID = "OPERATING";

        private static readonly List<Storage.StoredItemModifier> storedItemModifiers = new List<Storage.StoredItemModifier>
        {
            Storage.StoredItemModifier.Hide,
            Storage.StoredItemModifier.Preserve,
            Storage.StoredItemModifier.Insulate,
            Storage.StoredItemModifier.Seal,
        };

        // Buildingdef from SupermaterialRefineryConfig example
        public override BuildingDef CreateBuildingDef()
        {
            string id = ID;
            int width = 5;
            int height = 3;
            string anim = "supermaterial_refinery_kanim";
            int hitpoints = 30;
            float construction_time = 240f;
            float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
            string[] all_METALS = MATERIALS.REFINED_METALS;
            float melting_point = 2400f;
            BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
            EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER6;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tier, all_METALS, melting_point, build_location_rule, BUILDINGS.DECOR.PENALTY.TIER2, tier2, 0.2f);
            buildingDef.RequiresPowerInput = true;
            buildingDef.EnergyConsumptionWhenActive = BUILDINGS.ENERGY_CONSUMPTION_WHEN_ACTIVE.TIER2;
            buildingDef.SelfHeatKilowattsWhenActive = BUILDINGS.EXHAUST_ENERGY_ACTIVE.TIER8;
            buildingDef.UseHighEnergyParticleInputPort = true;
            buildingDef.HighEnergyParticleInputOffset = new CellOffset(0, 2);
            buildingDef.ViewMode = OverlayModes.Power.ID;
            buildingDef.AudioCategory = "HollowMetal";
            buildingDef.AudioSize = "large";
            buildingDef.OutputConduitType = ConduitType.Liquid;
            buildingDef.UtilityInputOffset = new CellOffset(-1, 0);
            // buildingDef.RequiredSkillPerkID = Db.Get().SkillPerks.AllowNuclearResearch.Id; // To test
            buildingDef.DiseaseCellVisName = RadiationPoisoning.ID;
            buildingDef.LogicOutputPorts = new List<LogicPorts.Port>
            {
                LogicPorts.Port.OutputPort(HEP_STORAGE_ID, new CellOffset(0, 2), STRINGS.BUILDINGS.PREFABS.HEPENGINE.LOGIC_PORT_STORAGE, STRINGS.BUILDINGS.PREFABS.HEPENGINE.LOGIC_PORT_STORAGE_ACTIVE, STRINGS.BUILDINGS.PREFABS.HEPENGINE.LOGIC_PORT_STORAGE_INACTIVE),
                LogicPorts.Port.OutputPort(OPERATING_PORT_ID, new CellOffset(-2, 1), PHO_STRINGS.NEUTRONIC_TRANSMUTATION_CHAMBER.PORT_NAME, PHO_STRINGS.NEUTRONIC_TRANSMUTATION_CHAMBER.PORT_ACTIVE, PHO_STRINGS.NEUTRONIC_TRANSMUTATION_CHAMBER.PORT_INACTIVE)
            };
            buildingDef.Deprecated = !Sim.IsRadiationEnabled();

            return buildingDef;
        }

        //ConfigureBuildingTemplate from SupermaterialRefineryConfig example
        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
            go.AddTag(GameTags.CorrosionProof);

            HighEnergyParticleStorage highEnergyParticleStorage = go.AddOrGet<HighEnergyParticleStorage>();
            highEnergyParticleStorage.capacity = 3000f;
            highEnergyParticleStorage.autoStore = true;
            highEnergyParticleStorage.PORT_ID = HEP_STORAGE_ID;
            highEnergyParticleStorage.showCapacityStatusItem = true;

            RadiationEmitter radiationEmitter = go.AddComponent<RadiationEmitter>();
            radiationEmitter.emitType = RadiationEmitter.RadiationEmitterType.Constant;
            radiationEmitter.emitRadiusX = 5;
            radiationEmitter.emitRadiusY = 3;
            radiationEmitter.radiusProportionalToRads = false;
            radiationEmitter.emissionOffset = new Vector3(0f, 1f, 0f);
            radiationEmitter.emitRads = 100;

            DropAllWorkable dropper = go.AddOrGet<DropAllWorkable>();
            dropper.requiredSkillPerk = Db.Get().SkillPerks.AllowNuclearResearch.Id;

            NeutronicTransmutationChamber neutronicChamber = go.AddOrGet<NeutronicTransmutationChamber>();
            neutronicChamber.IsWorking = OPERATING_PORT_ID;

            neutronicChamber.inStorage?.SetDefaultStoredItemModifiers(storedItemModifiers);
            neutronicChamber.buildStorage?.SetDefaultStoredItemModifiers(storedItemModifiers);
            neutronicChamber.outStorage?.SetDefaultStoredItemModifiers(storedItemModifiers);


            BuildingTemplates.CreateComplexFabricatorStorage(go, neutronicChamber);
            neutronicChamber.heatedTemperature = 313.15f;
            neutronicChamber.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
            neutronicChamber.duplicantOperated = false;
            neutronicChamber.showProgressBar = true;
            neutronicChamber.outputOffset = new Vector3(3, 1);

            ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
            conduitDispenser.conduitType = ConduitType.Liquid;
            conduitDispenser.alwaysDispense = true;
            conduitDispenser.elementFilter = new SimHashes[] { SimHashes.NuclearWaste };
            conduitDispenser.storage = neutronicChamber.outStorage;

            go.AddOrGet<BuildingComplete>().isManuallyOperated = false;
            go.AddOrGet<FabricatorIngredientStatusManager>();
            go.AddOrGet<CopyBuildingSettings>();
            Prioritizable.AddRef(go);


            SetRecipes();
        }
        public override void DoPostConfigureComplete(GameObject go)
        {
            go.GetComponent<HighEnergyParticlePort>().requireOperational = false; // Radbolts don't enter the machine if true (diamond press don't have it so I don't really understand why)
            HighEnergyParticleStorage hepStorage = go.GetComponent<HighEnergyParticleStorage>();
            /*component.Subscribe(-1837862626, delegate (object data)
            {
                //meter.SetPositionPercent(hepStorage.Particles / hepStorage.Capacity());
            });*/
            //meter.SetPositionPercent(hepStorage.Particles / hepStorage.Capacity());

        }
        public void SetRecipes()
        {
            const float recipeBaseAmount = 200f;
            const float timeDecayMultiplier = 2f; // Added that for everything to longer the time a bit
            const float timeDecayCycle = timeDecayMultiplier * 600f; // 600 is a cycle
            const float timeDecayMinuteMedium = timeDecayMultiplier * 60f;
            const float timeDecayMinimum = timeDecayMultiplier * 20f;

            NeutronicTransmutationRecipe HydrogenToHelium = NeutronicTransmutationRecipe.AddRecipe(new RecipeElement(SimHashes.Hydrogen.CreateTag(), recipeBaseAmount),
            new RecipeElement[]
            {
                new RecipeElement(SimHashes.Helium.CreateTag(), 60f, RecipeElement.TemperatureOperation.Heated, false),
                new RecipeElement(SimHashes.NuclearWaste.CreateTag(), 140f, RecipeElement.TemperatureOperation.Heated, true)
            }, BUILDINGS.ENERGY_CONSUMPTION_WHEN_ACTIVE.TIER2);
            HydrogenToHelium.time = timeDecayCycle * 12; // time half-life is 12 years irl
            HydrogenToHelium.description = PHO_STRINGS.RECIPE.HYDROGEN_TO_HELIUM;
            HydrogenToHelium.nameDisplay = RecipeNameDisplay.IngredientToResult;
            HydrogenToHelium.consumedHEP = 200;

            // nuclear fusion
            //https://fr.wikipedia.org/wiki/R%C3%A9action_triple_alpha
            /*ColliderRecipe heliumToCarbon = ColliderRecipe.AddRecipe(new ColliderRecipe.RecipeElement(SimHashes.Helium.CreateTag(), recipeBaseAmount),
            new ColliderRecipe.RecipeElement[]
            {
                new ColliderRecipe.RecipeElement(SimHashes.RefinedCarbon.CreateTag(), 160f, ColliderRecipe.RecipeElement.TemperatureOperation.Heated, false),
                new ColliderRecipe.RecipeElement(SimHashes.Oxygen.CreateTag(), 40f, ColliderRecipe.RecipeElement.TemperatureOperation.Heated, false)
            }, 1200);
            heliumToCarbon.time = 60f;
            heliumToCarbon.description = PHO_STRINGS.RECIPE.;
            heliumToCarbon.nameDisplay = ColliderRecipe.RecipeNameDisplay.IngredientToResult;
            heliumToCarbon.consumedHEP = 50;*/


            NeutronicTransmutationRecipe carbonToOxygen = NeutronicTransmutationRecipe.AddRecipe(new RecipeElement(SimHashes.RefinedCarbon.CreateTag(), recipeBaseAmount),
            new RecipeElement[]
            {
                new RecipeElement(SimHashes.Oxygen.CreateTag(), 30f, RecipeElement.TemperatureOperation.Heated, false),
                new RecipeElement(SimHashes.NuclearWaste.CreateTag(), 170f, RecipeElement.TemperatureOperation.Heated, true)
            }, BUILDINGS.ENERGY_CONSUMPTION_WHEN_ACTIVE.TIER2);

            carbonToOxygen.time = timeDecayMinimum; // irl time is 7s
            carbonToOxygen.description = PHO_STRINGS.RECIPE.CARBON_TO_OXYGEN;
            carbonToOxygen.nameDisplay = RecipeNameDisplay.IngredientToResult;
            carbonToOxygen.consumedHEP = 300;

            if (HighTechIndustry_NuclearWasteRecyclePatch.nitrogen.ElementExistsAndIsActive())
            {
                NeutronicTransmutationRecipe carbonToNitrogen = NeutronicTransmutationRecipe.AddRecipe(new RecipeElement(SimHashes.RefinedCarbon.CreateTag(), recipeBaseAmount),
                new RecipeElement[]
                {
                    new RecipeElement(HighTechIndustry_NuclearWasteRecyclePatch.nitrogen.id.CreateTag(), 180f, RecipeElement.TemperatureOperation.Heated, false),
                    new RecipeElement(SimHashes.NuclearWaste.CreateTag(), 20f, RecipeElement.TemperatureOperation.Heated, true)
                }, BUILDINGS.ENERGY_CONSUMPTION_WHEN_ACTIVE.TIER2);
                carbonToNitrogen.time = timeDecayCycle * 57;  // irl time is 5700y
                carbonToNitrogen.description = PHO_STRINGS.RECIPE.CARBON_TO_NITROGEN;
                carbonToNitrogen.nameDisplay = RecipeNameDisplay.IngredientToResult;
                carbonToNitrogen.consumedHEP = 200;

                NeutronicTransmutationRecipe nitrogenToOxygen = NeutronicTransmutationRecipe.AddRecipe(new RecipeElement(HighTechIndustry_NuclearWasteRecyclePatch.nitrogen.id.CreateTag(), recipeBaseAmount),
                new RecipeElement[]
                {
                    new RecipeElement(SimHashes.Oxygen.CreateTag(), 180f, RecipeElement.TemperatureOperation.Heated, false),
                    new RecipeElement(SimHashes.NuclearWaste.CreateTag(), 20f, RecipeElement.TemperatureOperation.Heated, true)
                }, BUILDINGS.ENERGY_CONSUMPTION_WHEN_ACTIVE.TIER2);
                nitrogenToOxygen.time = timeDecayMinimum; // irl time is 7s
                nitrogenToOxygen.description = PHO_STRINGS.RECIPE.NITROGEN_TO_OXYGEN;
                nitrogenToOxygen.nameDisplay = RecipeNameDisplay.IngredientToResult;
                nitrogenToOxygen.consumedHEP = 200;
            }

            NeutronicTransmutationRecipe sodiumToAluminium = NeutronicTransmutationRecipe.AddRecipe(new RecipeElement(SimHashes.Salt.CreateTag(), recipeBaseAmount),
            new RecipeElement[]
            {
                new RecipeElement(SimHashes.Aluminum.CreateTag(), 90f, RecipeElement.TemperatureOperation.Heated, false),
                new RecipeElement(SimHashes.NuclearWaste.CreateTag(), 110f, RecipeElement.TemperatureOperation.Heated, true)
            }, BUILDINGS.ENERGY_CONSUMPTION_WHEN_ACTIVE.TIER2);
            sodiumToAluminium.time = timeDecayMinuteMedium; // irl time is 10min
            sodiumToAluminium.description = PHO_STRINGS.RECIPE.SODIUM_TO_ALUMINIUM;
            sodiumToAluminium.nameDisplay = RecipeNameDisplay.IngredientToResult;
            sodiumToAluminium.consumedHEP = 200;

            NeutronicTransmutationRecipe aluminiumToSand = NeutronicTransmutationRecipe.AddRecipe(new RecipeElement(SimHashes.Aluminum.CreateTag(), recipeBaseAmount),
            new RecipeElement[]
            {
                new RecipeElement(SimHashes.Sand.CreateTag(), 200f, RecipeElement.TemperatureOperation.Heated, false),
            }, BUILDINGS.ENERGY_CONSUMPTION_WHEN_ACTIVE.TIER2);
            aluminiumToSand.time = timeDecayMinuteMedium * 2; // 157min
            aluminiumToSand.description = PHO_STRINGS.RECIPE.ALUMINIUM_TO_SAND;
            aluminiumToSand.nameDisplay = RecipeNameDisplay.IngredientToResult;
            aluminiumToSand.consumedHEP = 200;

            NeutronicTransmutationRecipe siliconToPhosphorus = NeutronicTransmutationRecipe.AddRecipe(new RecipeElement(SimHashes.Sand.CreateTag(), recipeBaseAmount),
            new RecipeElement[]
            {
                new RecipeElement(SimHashes.Phosphorus.CreateTag(), 100f, RecipeElement.TemperatureOperation.Heated, false),
                new RecipeElement(SimHashes.Sulfur.CreateTag(), 60f, RecipeElement.TemperatureOperation.Heated, false),
                new RecipeElement(SimHashes.NuclearWaste.CreateTag(), 40f, RecipeElement.TemperatureOperation.Heated, true)
            }, BUILDINGS.ENERGY_CONSUMPTION_WHEN_ACTIVE.TIER2);
            siliconToPhosphorus.time = timeDecayMinuteMedium * 2; // 157min
            siliconToPhosphorus.description = PHO_STRINGS.RECIPE.SILICON_TO_PHOSPHORUS;
            siliconToPhosphorus.nameDisplay = RecipeNameDisplay.Composite;
            siliconToPhosphorus.consumedHEP = 200;


            NeutronicTransmutationRecipe sulfurToChroline = NeutronicTransmutationRecipe.AddRecipe(new RecipeElement(SimHashes.Sulfur.CreateTag(), recipeBaseAmount),
            new RecipeElement[]
            {
                new RecipeElement(SimHashes.ChlorineGas.CreateTag(), 180f, RecipeElement.TemperatureOperation.Heated, false),
                new RecipeElement(SimHashes.NuclearWaste.CreateTag(), 20f, RecipeElement.TemperatureOperation.Heated, true)
            }, BUILDINGS.ENERGY_CONSUMPTION_WHEN_ACTIVE.TIER2);
            sulfurToChroline.time = timeDecayCycle; // 87 day
            sulfurToChroline.description = PHO_STRINGS.RECIPE.SULFUR_TO_CHLORINE;
            sulfurToChroline.nameDisplay = RecipeNameDisplay.IngredientToResult;
            sulfurToChroline.consumedHEP = 200;


            NeutronicTransmutationRecipe ironToCobalt = NeutronicTransmutationRecipe.AddRecipe(new RecipeElement(SimHashes.Iron.CreateTag(), recipeBaseAmount),
            new RecipeElement[]
            {
                new RecipeElement(SimHashes.Cobalt.CreateTag(), 160f, RecipeElement.TemperatureOperation.Heated, false),
                new RecipeElement(SimHashes.Nickel.CreateTag(), 20f, RecipeElement.TemperatureOperation.Heated, false),
                new RecipeElement(SimHashes.NuclearWaste.CreateTag(), 20f, RecipeElement.TemperatureOperation.Heated, true)
            }, BUILDINGS.ENERGY_CONSUMPTION_WHEN_ACTIVE.TIER3);
            ironToCobalt.time = timeDecayCycle; // 44 day
            ironToCobalt.description = PHO_STRINGS.RECIPE.IRON_TO_COBALT;
            ironToCobalt.nameDisplay = RecipeNameDisplay.IngredientToResult;
            ironToCobalt.consumedHEP = 400;

            NeutronicTransmutationRecipe cobaltToNickel = NeutronicTransmutationRecipe.AddRecipe(new RecipeElement(SimHashes.Cobalt.CreateTag(), recipeBaseAmount),
            new RecipeElement[]
            {
                new RecipeElement(SimHashes.Nickel.CreateTag(), 150f, RecipeElement.TemperatureOperation.Heated, false),
                new RecipeElement(SimHashes.NuclearWaste.CreateTag(), 50f, RecipeElement.TemperatureOperation.Heated, true)
            }, BUILDINGS.ENERGY_CONSUMPTION_WHEN_ACTIVE.TIER3);
            cobaltToNickel.time = timeDecayCycle * 4; // 1925d
            cobaltToNickel.description = PHO_STRINGS.RECIPE.COBALT_TO_NICKEL;
            cobaltToNickel.nameDisplay = RecipeNameDisplay.IngredientToResult;
            cobaltToNickel.consumedHEP = 400;

            NeutronicTransmutationRecipe nickelToCopper = NeutronicTransmutationRecipe.AddRecipe(new RecipeElement(SimHashes.Nickel.CreateTag(), recipeBaseAmount),
            new RecipeElement[]
            {
                new RecipeElement(SimHashes.Copper.CreateTag(), 150f, RecipeElement.TemperatureOperation.Heated, false),
                new RecipeElement(SimHashes.Zinc.CreateTag(), 20f, RecipeElement.TemperatureOperation.Heated, false),
                new RecipeElement(SimHashes.NuclearWaste.CreateTag(), 30f, RecipeElement.TemperatureOperation.Heated, true)
            }, BUILDINGS.ENERGY_CONSUMPTION_WHEN_ACTIVE.TIER3);
            nickelToCopper.time = timeDecayCycle * 10; // 100y for one and 2h for the other nucleide
            nickelToCopper.description = PHO_STRINGS.RECIPE.NICKEL_TO_COPPER;
            nickelToCopper.nameDisplay = RecipeNameDisplay.Composite;
            nickelToCopper.consumedHEP = 400;

            NeutronicTransmutationRecipe copperToNickelAndZinc = NeutronicTransmutationRecipe.AddRecipe(new RecipeElement(SimHashes.Copper.CreateTag(), recipeBaseAmount),
            new RecipeElement[]
            {
                    new RecipeElement(SimHashes.Nickel.CreateTag(), 140f, RecipeElement.TemperatureOperation.Heated, false),
                    new RecipeElement(SimHashes.Zinc.CreateTag(), 60f, RecipeElement.TemperatureOperation.Heated, true),
            }, BUILDINGS.ENERGY_CONSUMPTION_WHEN_ACTIVE.TIER2);

            copperToNickelAndZinc.time = timeDecayMinuteMedium; // 12h and 5min
            copperToNickelAndZinc.description = PHO_STRINGS.RECIPE.COPPER_TO_NICKEL_AND_ZINC;
            copperToNickelAndZinc.nameDisplay = RecipeNameDisplay.Composite;
            copperToNickelAndZinc.consumedHEP = 200;


            NeutronicTransmutationRecipe zincToCopper = NeutronicTransmutationRecipe.AddRecipe(new RecipeElement(SimHashes.Zinc.CreateTag(), recipeBaseAmount),
            new RecipeElement[]
            {
                    new RecipeElement(SimHashes.Copper.CreateTag(), recipeBaseAmount / 2, RecipeElement.TemperatureOperation.Heated, false),
                    new RecipeElement(SimHashes.Zinc.CreateTag(), recipeBaseAmount / 2, RecipeElement.TemperatureOperation.Heated, false),
            }, BUILDINGS.ENERGY_CONSUMPTION_WHEN_ACTIVE.TIER2);
            zincToCopper.time = timeDecayCycle; // 243 d
            zincToCopper.description = PHO_STRINGS.RECIPE.ZINC_TO_COPPER;
            zincToCopper.nameDisplay = RecipeNameDisplay.Composite;
            zincToCopper.consumedHEP = 200;


            NeutronicTransmutationRecipe tungstenToIridium = NeutronicTransmutationRecipe.AddRecipe(new RecipeElement(SimHashes.Tungsten.CreateTag(), recipeBaseAmount),
            new RecipeElement[]
            {
                new RecipeElement(SimHashes.Iridium.CreateTag(), 20f, RecipeElement.TemperatureOperation.Heated, false),
                new RecipeElement(SimHashes.NuclearWaste.CreateTag(), recipeBaseAmount - 20f, RecipeElement.TemperatureOperation.Heated, true),
            }, BUILDINGS.ENERGY_CONSUMPTION_WHEN_ACTIVE.TIER3);
            tungstenToIridium.time = timeDecayCycle; // hard to tell, most logic is 29h, but needs multiple cycles
            tungstenToIridium.description = PHO_STRINGS.RECIPE.TUNGSTENE_TO_IRIDIUM;
            tungstenToIridium.nameDisplay = RecipeNameDisplay.IngredientToResult;
            tungstenToIridium.consumedHEP = 800;

            NeutronicTransmutationRecipe mercuryToGold = NeutronicTransmutationRecipe.AddRecipe(new RecipeElement(SimHashes.Mercury.CreateTag(), recipeBaseAmount),
            new RecipeElement[]
            {
                new RecipeElement(SimHashes.Gold.CreateTag(), 20f, RecipeElement.TemperatureOperation.Heated, false),
                new RecipeElement(SimHashes.Lead.CreateTag(), 40f, RecipeElement.TemperatureOperation.Heated, false),
                new RecipeElement(SimHashes.Mercury.CreateTag(), 40f, RecipeElement.TemperatureOperation.Heated, false),
                new RecipeElement(SimHashes.NuclearWaste.CreateTag(), recipeBaseAmount-20f-40-40f, RecipeElement.TemperatureOperation.Heated, true)
            }, BUILDINGS.ENERGY_CONSUMPTION_WHEN_ACTIVE.TIER3);
            mercuryToGold.time = timeDecayMinuteMedium; // hard to tell too, must logic is 64h
            mercuryToGold.description = PHO_STRINGS.RECIPE.MERCURY_TO_GOLD;
            mercuryToGold.nameDisplay = RecipeNameDisplay.Composite;
            mercuryToGold.consumedHEP = 400;

            NeutronicTransmutationRecipe mercuryToLead = NeutronicTransmutationRecipe.AddRecipe(new RecipeElement(SimHashes.Mercury.CreateTag(), recipeBaseAmount),
            new RecipeElement[]
            {
                new RecipeElement(SimHashes.Lead.CreateTag(), 150f, RecipeElement.TemperatureOperation.Heated, false),
                new RecipeElement(SimHashes.NuclearWaste.CreateTag(), 50f, RecipeElement.TemperatureOperation.Heated, true),
            }, BUILDINGS.ENERGY_CONSUMPTION_WHEN_ACTIVE.TIER3);
            mercuryToLead.time = timeDecayCycle; // 10min for most but multiple steps needs time
            mercuryToLead.description = PHO_STRINGS.RECIPE.MERCURY_TO_LEAD;
            mercuryToLead.nameDisplay = RecipeNameDisplay.IngredientToResult;
            mercuryToLead.consumedHEP = 400;

            NeutronicTransmutationRecipe depletedUraniumToPlutoniumSurgeneration = NeutronicTransmutationRecipe.AddRecipe(new RecipeElement(SimHashes.DepletedUranium.CreateTag(), 200),
            new RecipeElement[]
            {
                new RecipeElement(SimHashes.EnrichedUranium.CreateTag(), 10f, RecipeElement.TemperatureOperation.Heated, false),
                new RecipeElement(SimHashes.DepletedUranium.CreateTag(), 180f, RecipeElement.TemperatureOperation.Heated, false),
                new RecipeElement(SimHashes.NuclearWaste.CreateTag(), 10f, RecipeElement.TemperatureOperation.Heated, true),
            }, BUILDINGS.ENERGY_CONSUMPTION_WHEN_ACTIVE.TIER3);
            depletedUraniumToPlutoniumSurgeneration.time = timeDecayCycle;
            depletedUraniumToPlutoniumSurgeneration.description = PHO_STRINGS.RECIPE.URANIUM_SURGENERATION;
            depletedUraniumToPlutoniumSurgeneration.nameDisplay = RecipeNameDisplay.IngredientToResult;
            depletedUraniumToPlutoniumSurgeneration.consumedHEP = 400;
            depletedUraniumToPlutoniumSurgeneration.requiredTech = "NuclearPropulsion";

        }



        public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
        {
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
        }


    }
}
