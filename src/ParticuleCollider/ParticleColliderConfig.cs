using Klei.AI;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

namespace ParticleCollider
{
    public class ParticleColliderConfig : IBuildingConfig
    {
        public override string[] GetRequiredDlcIds()
        {
            return DlcManager.EXPANSION1;
        }

        // Buildingdef from SupermaterialRefineryConfig example
        public override BuildingDef CreateBuildingDef()
        {
            string id = ID;
            int width = 6;
            int height = 5;
            string anim = "supermaterial_refinery_kanim";
            int hitpoints = 60;
            float construction_time = 480f;
            float[] tier = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER7;
            string[] all_METALS = MATERIALS.ALL_METALS;
            float melting_point = 2400f;
            BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
            EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER6;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tier, all_METALS, melting_point, build_location_rule, TUNING.BUILDINGS.DECOR.PENALTY.TIER2, tier2, 0.2f);
            buildingDef.RequiresPowerInput = true;
            buildingDef.EnergyConsumptionWhenActive = 1900f;
            buildingDef.SelfHeatKilowattsWhenActive = 24f;
            buildingDef.UseHighEnergyParticleInputPort = true;
            buildingDef.HighEnergyParticleInputOffset = new CellOffset(0, 2);
            buildingDef.ViewMode = OverlayModes.Power.ID;
            buildingDef.AudioCategory = "HollowMetal";
            buildingDef.AudioSize = "large";
            buildingDef.InputConduitType = ConduitType.Gas;
            buildingDef.UtilityInputOffset = new CellOffset(-1, 0);
            buildingDef.OutputConduitType = ConduitType.Liquid;
            buildingDef.UtilityInputOffset = new CellOffset(-1, 0);

            buildingDef.RequiredSkillPerkID = Db.Get().SkillPerks.AllowNuclearResearch.Id;
            buildingDef.DiseaseCellVisName = RadiationPoisoning.ID;
            buildingDef.LogicOutputPorts = new List<LogicPorts.Port>
            {
                LogicPorts.Port.OutputPort("HEP_STORAGE", new CellOffset(0, 2), STRINGS.BUILDINGS.PREFABS.HEPENGINE.LOGIC_PORT_STORAGE, STRINGS.BUILDINGS.PREFABS.HEPENGINE.LOGIC_PORT_STORAGE_ACTIVE, STRINGS.BUILDINGS.PREFABS.HEPENGINE.LOGIC_PORT_STORAGE_INACTIVE, false, false)
            };

            return buildingDef;
        }

        //ConfigureBuildingTemplate from SupermaterialRefineryConfig example
        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            HighEnergyParticleStorage highEnergyParticleStorage = go.AddOrGet<HighEnergyParticleStorage>();
            highEnergyParticleStorage.capacity = 1800f;
            highEnergyParticleStorage.autoStore = true;
            highEnergyParticleStorage.PORT_ID = "HEP_STORAGE";
            highEnergyParticleStorage.showCapacityStatusItem = true;
            ComplexFabricatorWorkable component = go.AddOrGet<ComplexFabricatorWorkable>();
            /*MeterController meter = new MeterController(component.GetAnimController(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[]
            {
                "meter_target",
                "meter_fill",
                "meter_frame",
                "meter_OL"
            });*/
            HighEnergyParticleStorage hepStorage = component.GetComponent<HighEnergyParticleStorage>();
            /*component.Subscribe(-1837862626, delegate (object data)
            {
                //meter.SetPositionPercent(hepStorage.Particles / hepStorage.Capacity());
            });*/
            //meter.SetPositionPercent(hepStorage.Particles / hepStorage.Capacity());



            DropAllWorkable dropper = go.AddOrGet<DropAllWorkable>();

            ParticleCollider particleCollider = go.AddOrGet<ParticleCollider>();
            BuildingTemplates.CreateComplexFabricatorStorage(go, particleCollider);
            particleCollider.heatedTemperature = 313.15f;
            particleCollider.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
            particleCollider.duplicantOperated = true;
            particleCollider.outputOffset = new Vector3(3, 1);

            ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
            conduitConsumer.conduitType = ConduitType.Gas;
            conduitConsumer.capacityTag = SimHashes.Hydrogen.CreateTag();
            conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
            conduitConsumer.capacityKG = 400f;
            conduitConsumer.forceAlwaysSatisfied = true;
            conduitConsumer.storage = particleCollider.inStorage;

            ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
            conduitDispenser.conduitType = ConduitType.Liquid;
            conduitDispenser.alwaysDispense = true;
            conduitDispenser.elementFilter = new SimHashes[] { SimHashes.NuclearWaste };
            conduitDispenser.storage = particleCollider.outStorage;

            go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
            go.AddOrGet<FabricatorIngredientStatusManager>();
            go.AddOrGet<CopyBuildingSettings>();
            Prioritizable.AddRef(go);


            SetRecipes();
        }
        public override void DoPostConfigureComplete(GameObject go)
        {
            go.GetComponent<KPrefabID>().prefabSpawnFn += delegate (GameObject game_object)
            {
                ComplexFabricatorWorkable component = game_object.GetComponent<ComplexFabricatorWorkable>();
                component.requiredSkillPerk = Db.Get().SkillPerks.AllowNuclearResearch.Id;
                component.WorkerStatusItem = Db.Get().DuplicantStatusItems.Researching;
                component.AttributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
                component.AttributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
                component.SkillExperienceSkillGroup = Db.Get().SkillGroups.Research.Id;
                component.SkillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
                KAnimFile anim = Assets.GetAnim("anim_interacts_supermaterial_refinery_kanim");
                KAnimFile[] overrideAnims = new KAnimFile[]
                {
                    anim
                };
                component.overrideAnims = overrideAnims;
                component.workAnims = new HashedString[]
                {
                    "working_pre",
                    "working_loop"
                };
                component.synchronizeAnims = false;
                KAnimFileData data = anim.GetData();
                int animCount = data.animCount;
                dupeInteractAnims = new HashedString[animCount - 2];
                int i = 0;
                int num = 0;
                while (i < animCount)
                {
                    HashedString hashedString = data.GetAnim(i).name;
                    if (hashedString != "working_pre" && hashedString != "working_pst")
                    {
                        dupeInteractAnims[num] = hashedString;
                        num++;
                    }
                    i++;
                }
                component.GetDupeInteract = () => new HashedString[]
                {
                    "working_loop",
                    dupeInteractAnims.GetRandom<HashedString>()
                };
            };
        }
        public void SetRecipes()
        {

            ColliderRecipe dataCreation = ColliderRecipe.AddRecipe(
            new ColliderRecipe.RecipeElement[]
            {
                new ColliderRecipe.RecipeElement(SimHashes.Hydrogen.CreateTag(), 200f + ColliderRecipe.ProtonSourceUse),
            },
            new ColliderRecipe.RecipeElement[]
            {
                new ColliderRecipe.RecipeElement(DatabankHelper.TAG, 20, ColliderRecipe.RecipeElement.TemperatureOperation.Heated, false),
                new ColliderRecipe.RecipeElement(SimHashes.NuclearWaste.CreateTag(), ColliderRecipe.GenericTotalWaste, ColliderRecipe.RecipeElement.TemperatureOperation.Heated, true)
            }, 800, true);
            dataCreation.time = 60f;
            dataCreation.description = STRINGS.BUILDINGS.PREFABS.SUPERMATERIALREFINERY.SUPERCOOLANT_RECIPE_DESCRIPTION;
            dataCreation.nameDisplay = ColliderRecipe.RecipeNameDisplay.Result;
            dataCreation.consumedHEP = 50;

            ColliderRecipe HydrogenToHelium = ColliderRecipe.AddRecipe(new ColliderRecipe.RecipeElement(SimHashes.Hydrogen.CreateTag(), 200f + ColliderRecipe.ProtonSourceUse),
            new ColliderRecipe.RecipeElement[]
            {
                new ColliderRecipe.RecipeElement(SimHashes.Helium.CreateTag(), 200f, ColliderRecipe.RecipeElement.TemperatureOperation.Heated, false),
                new ColliderRecipe.RecipeElement(SimHashes.NuclearWaste.CreateTag(), ColliderRecipe.GenericTotalWaste, ColliderRecipe.RecipeElement.TemperatureOperation.Heated, true)
            }, 800, true);
            HydrogenToHelium.time = 60f;
            HydrogenToHelium.description = STRINGS.BUILDINGS.PREFABS.SUPERMATERIALREFINERY.SUPERCOOLANT_RECIPE_DESCRIPTION;
            HydrogenToHelium.nameDisplay = ColliderRecipe.RecipeNameDisplay.IngredientToResult;
            HydrogenToHelium.consumedHEP = 50;

            // nuclear fusion
            //https://fr.wikipedia.org/wiki/R%C3%A9action_triple_alpha
            /*ColliderRecipe heliumToCarbon = ColliderRecipe.AddRecipe(new ColliderRecipe.RecipeElement(SimHashes.Helium.CreateTag(), 200f),
            new ColliderRecipe.RecipeElement[]
            {
                new ColliderRecipe.RecipeElement(SimHashes.RefinedCarbon.CreateTag(), 160f, ColliderRecipe.RecipeElement.TemperatureOperation.Heated, false),
                new ColliderRecipe.RecipeElement(SimHashes.Oxygen.CreateTag(), 40f, ColliderRecipe.RecipeElement.TemperatureOperation.Heated, false)
            }, 1200);
            heliumToCarbon.time = 60f;
            heliumToCarbon.description = STRINGS.BUILDINGS.PREFABS.SUPERMATERIALREFINERY.SUPERCOOLANT_RECIPE_DESCRIPTION;
            heliumToCarbon.nameDisplay = ColliderRecipe.RecipeNameDisplay.IngredientToResult;
            heliumToCarbon.consumedHEP = 50;*/


            ColliderRecipe carbonToOxygen = ColliderRecipe.AddRecipe(new ColliderRecipe.RecipeElement(SimHashes.RefinedCarbon.CreateTag(), 200f),
            new ColliderRecipe.RecipeElement[]
            {
                new ColliderRecipe.RecipeElement(SimHashes.Oxygen.CreateTag(), 180f, ColliderRecipe.RecipeElement.TemperatureOperation.Heated, false),
                new ColliderRecipe.RecipeElement(SimHashes.NuclearWaste.CreateTag(), 20f + ColliderRecipe.GenericTotalWaste, ColliderRecipe.RecipeElement.TemperatureOperation.Heated, true)
            }, 1200);

            carbonToOxygen.time = 60f;
            carbonToOxygen.description = STRINGS.BUILDINGS.PREFABS.SUPERMATERIALREFINERY.SUPERCOOLANT_RECIPE_DESCRIPTION;
            carbonToOxygen.nameDisplay = ColliderRecipe.RecipeNameDisplay.IngredientToResult;
            carbonToOxygen.consumedHEP = 50;

            ColliderRecipe sodiumToAluminium = ColliderRecipe.AddRecipe(new ColliderRecipe.RecipeElement(SimHashes.Salt.CreateTag(), 200f),
            new ColliderRecipe.RecipeElement[]
            {
                new ColliderRecipe.RecipeElement(SimHashes.Aluminum.CreateTag(), 90f, ColliderRecipe.RecipeElement.TemperatureOperation.Heated, false),
                new ColliderRecipe.RecipeElement(SimHashes.NuclearWaste.CreateTag(), 110f + ColliderRecipe.GenericTotalWaste, ColliderRecipe.RecipeElement.TemperatureOperation.Heated, true)
            }, 1200);
            sodiumToAluminium.time = 60f;
            sodiumToAluminium.description = STRINGS.BUILDINGS.PREFABS.SUPERMATERIALREFINERY.SUPERCOOLANT_RECIPE_DESCRIPTION;
            sodiumToAluminium.nameDisplay = ColliderRecipe.RecipeNameDisplay.IngredientToResult;
            sodiumToAluminium.consumedHEP = 50;

            ColliderRecipe aluminiumToPhosphore = ColliderRecipe.AddRecipe(new ColliderRecipe.RecipeElement(SimHashes.Aluminum.CreateTag(), 200f),
            new ColliderRecipe.RecipeElement[]
            {
                new ColliderRecipe.RecipeElement(SimHashes.Sand.CreateTag(), 40f, ColliderRecipe.RecipeElement.TemperatureOperation.Heated, false),
                new ColliderRecipe.RecipeElement(SimHashes.Phosphorus.CreateTag(), 120f, ColliderRecipe.RecipeElement.TemperatureOperation.Heated, false),
                new ColliderRecipe.RecipeElement(SimHashes.Sulfur.CreateTag(), 40f, ColliderRecipe.RecipeElement.TemperatureOperation.Heated, false),
                new ColliderRecipe.RecipeElement(SimHashes.NuclearWaste.CreateTag(), ColliderRecipe.GenericTotalWaste, ColliderRecipe.RecipeElement.TemperatureOperation.Heated, true)
            }, 1200);
            aluminiumToPhosphore.time = 60f;
            aluminiumToPhosphore.description = STRINGS.BUILDINGS.PREFABS.SUPERMATERIALREFINERY.SUPERCOOLANT_RECIPE_DESCRIPTION;
            aluminiumToPhosphore.nameDisplay = ColliderRecipe.RecipeNameDisplay.IngredientToResult;
            aluminiumToPhosphore.consumedHEP = 50;

            ColliderRecipe siliconToPhosphorus = ColliderRecipe.AddRecipe(new ColliderRecipe.RecipeElement(SimHashes.Sand.CreateTag(), 200f),
            new ColliderRecipe.RecipeElement[]
            {
                new ColliderRecipe.RecipeElement(SimHashes.Phosphorus.CreateTag(), 100f, ColliderRecipe.RecipeElement.TemperatureOperation.Heated, false),
                new ColliderRecipe.RecipeElement(SimHashes.Sulfur.CreateTag(), 60f, ColliderRecipe.RecipeElement.TemperatureOperation.Heated, false),
                new ColliderRecipe.RecipeElement(SimHashes.NuclearWaste.CreateTag(), 40f + ColliderRecipe.GenericTotalWaste, ColliderRecipe.RecipeElement.TemperatureOperation.Heated, true)
            }, 1200);
            siliconToPhosphorus.time = 60f;
            siliconToPhosphorus.description = STRINGS.BUILDINGS.PREFABS.SUPERMATERIALREFINERY.SUPERCOOLANT_RECIPE_DESCRIPTION;
            siliconToPhosphorus.nameDisplay = ColliderRecipe.RecipeNameDisplay.Composite;
            siliconToPhosphorus.consumedHEP = 50;


            ColliderRecipe sulfurToPhosphorus = ColliderRecipe.AddRecipe(new ColliderRecipe.RecipeElement(SimHashes.Sulfur.CreateTag(), 200f),
            new ColliderRecipe.RecipeElement[]
            {
                new ColliderRecipe.RecipeElement(SimHashes.Phosphorus.CreateTag(), 150f, ColliderRecipe.RecipeElement.TemperatureOperation.Heated, false),
                new ColliderRecipe.RecipeElement(SimHashes.ChlorineGas.CreateTag(), 30f, ColliderRecipe.RecipeElement.TemperatureOperation.Heated, false),
                new ColliderRecipe.RecipeElement(SimHashes.NuclearWaste.CreateTag(), 20f + ColliderRecipe.GenericTotalWaste, ColliderRecipe.RecipeElement.TemperatureOperation.Heated, true)
            }, 1200);
            sulfurToPhosphorus.time = 60f;
            sulfurToPhosphorus.description = STRINGS.BUILDINGS.PREFABS.SUPERMATERIALREFINERY.SUPERCOOLANT_RECIPE_DESCRIPTION;
            sulfurToPhosphorus.nameDisplay = ColliderRecipe.RecipeNameDisplay.IngredientToResult;
            sulfurToPhosphorus.consumedHEP = 50;


            ColliderRecipe ironToCobalt = ColliderRecipe.AddRecipe(new ColliderRecipe.RecipeElement(SimHashes.Iron.CreateTag(), 200f),
            new ColliderRecipe.RecipeElement[]
            {
                new ColliderRecipe.RecipeElement(SimHashes.Cobalt.CreateTag(), 160f, ColliderRecipe.RecipeElement.TemperatureOperation.Heated, false),
                new ColliderRecipe.RecipeElement(SimHashes.NuclearWaste.CreateTag(), 40f + ColliderRecipe.GenericTotalWaste, ColliderRecipe.RecipeElement.TemperatureOperation.Heated, true)
            }, 1800);
            ironToCobalt.time = 60f;
            ironToCobalt.description = STRINGS.BUILDINGS.PREFABS.SUPERMATERIALREFINERY.SUPERCOOLANT_RECIPE_DESCRIPTION;
            ironToCobalt.nameDisplay = ColliderRecipe.RecipeNameDisplay.IngredientToResult;
            ironToCobalt.consumedHEP = 100;

            ColliderRecipe cobaltToNickel = ColliderRecipe.AddRecipe(new ColliderRecipe.RecipeElement(SimHashes.Cobalt.CreateTag(), 200f),
            new ColliderRecipe.RecipeElement[]
            {
                new ColliderRecipe.RecipeElement(SimHashes.Nickel.CreateTag(), 150f, ColliderRecipe.RecipeElement.TemperatureOperation.Heated, false),
                new ColliderRecipe.RecipeElement(SimHashes.NuclearWaste.CreateTag(), 50f + ColliderRecipe.GenericTotalWaste, ColliderRecipe.RecipeElement.TemperatureOperation.Heated, true)
            }, 1800);
            cobaltToNickel.time = 60f;
            cobaltToNickel.description = STRINGS.BUILDINGS.PREFABS.SUPERMATERIALREFINERY.SUPERCOOLANT_RECIPE_DESCRIPTION;
            cobaltToNickel.nameDisplay = ColliderRecipe.RecipeNameDisplay.IngredientToResult;
            cobaltToNickel.consumedHEP = 100;

            ColliderRecipe nickelToCopper = ColliderRecipe.AddRecipe(new ColliderRecipe.RecipeElement(SimHashes.Nickel.CreateTag(), 200f),
            new ColliderRecipe.RecipeElement[]
            {
                new ColliderRecipe.RecipeElement(SimHashes.Copper.CreateTag(), 150f, ColliderRecipe.RecipeElement.TemperatureOperation.Heated, false),
                new ColliderRecipe.RecipeElement(SimHashes.NuclearWaste.CreateTag(), 50f + ColliderRecipe.GenericTotalWaste, ColliderRecipe.RecipeElement.TemperatureOperation.Heated, true)
            }, 1800);
            nickelToCopper.time = 60f;
            nickelToCopper.description = STRINGS.BUILDINGS.PREFABS.SUPERMATERIALREFINERY.SUPERCOOLANT_RECIPE_DESCRIPTION;
            nickelToCopper.nameDisplay = ColliderRecipe.RecipeNameDisplay.IngredientToResult;
            nickelToCopper.consumedHEP = 100;

            ColliderRecipe mercuryToGold = ColliderRecipe.AddRecipe(new ColliderRecipe.RecipeElement(SimHashes.Mercury.CreateTag(), 200f),
            new ColliderRecipe.RecipeElement[]
            {
                new ColliderRecipe.RecipeElement(SimHashes.Gold.CreateTag(), 20f, ColliderRecipe.RecipeElement.TemperatureOperation.Heated, false),
                new ColliderRecipe.RecipeElement(SimHashes.Lead.CreateTag(), 40f, ColliderRecipe.RecipeElement.TemperatureOperation.Heated, false),
                new ColliderRecipe.RecipeElement(SimHashes.NuclearWaste.CreateTag(), 200f-20f-40f + ColliderRecipe.GenericTotalWaste, ColliderRecipe.RecipeElement.TemperatureOperation.Heated, true)
            }, 2400);
            mercuryToGold.time = 60f;
            mercuryToGold.description = STRINGS.BUILDINGS.PREFABS.SUPERMATERIALREFINERY.SUPERCOOLANT_RECIPE_DESCRIPTION;
            mercuryToGold.nameDisplay = ColliderRecipe.RecipeNameDisplay.Composite;
            mercuryToGold.consumedHEP = 100;


            ColliderRecipe tungstenToIridium = ColliderRecipe.AddRecipe(new ColliderRecipe.RecipeElement(SimHashes.Tungsten.CreateTag(), 200f),
            new ColliderRecipe.RecipeElement[]
            {
                new ColliderRecipe.RecipeElement(SimHashes.Iridium.CreateTag(), 20f, ColliderRecipe.RecipeElement.TemperatureOperation.Heated, false),
                new ColliderRecipe.RecipeElement(SimHashes.Lead.CreateTag(), 40f, ColliderRecipe.RecipeElement.TemperatureOperation.Heated, false),
                new ColliderRecipe.RecipeElement(SimHashes.NuclearWaste.CreateTag(), 200f-40f-20f + ColliderRecipe.GenericTotalWaste, ColliderRecipe.RecipeElement.TemperatureOperation.Heated, true),
            }, 2400);
            tungstenToIridium.time = 60f;
            tungstenToIridium.description = STRINGS.BUILDINGS.PREFABS.SUPERMATERIALREFINERY.SUPERCOOLANT_RECIPE_DESCRIPTION;
            tungstenToIridium.nameDisplay = ColliderRecipe.RecipeNameDisplay.Composite;
            tungstenToIridium.consumedHEP = 200;

            ColliderRecipe mercuryToLead = ColliderRecipe.AddRecipe(new ColliderRecipe.RecipeElement(SimHashes.Mercury.CreateTag(), 200f),
            new ColliderRecipe.RecipeElement[]
            {
                new ColliderRecipe.RecipeElement(SimHashes.Lead.CreateTag(), 150f, ColliderRecipe.RecipeElement.TemperatureOperation.Heated, false),
                new ColliderRecipe.RecipeElement(SimHashes.NuclearWaste.CreateTag(), 50f + ColliderRecipe.GenericTotalWaste, ColliderRecipe.RecipeElement.TemperatureOperation.Heated, true),
            }, 2400);
            mercuryToLead.time = 60f;
            mercuryToLead.description = STRINGS.BUILDINGS.PREFABS.SUPERMATERIALREFINERY.SUPERCOOLANT_RECIPE_DESCRIPTION;
            mercuryToLead.nameDisplay = ColliderRecipe.RecipeNameDisplay.IngredientToResult;
            mercuryToLead.consumedHEP = 100;

        }


   
        public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
        {
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
        }


        private const float INPUT_KG = 100f;

        private const float OUTPUT_KG = 100f;

        private const float OUTPUT_TEMPERATURE = 313.15f;

        private HashedString[] dupeInteractAnims;


        public const string ID = "ParticleCollider";
    }
}
