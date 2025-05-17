using System.Collections.Generic;
using TUNING;
using UnityEngine;

namespace ParticleCollider
{
    public class ParticleColliderConfig : IBuildingConfig
    {
        // Buildingdef from SupermaterialRefineryConfig example
        public override BuildingDef CreateBuildingDef()
        {
            string id = ID;
            int width = 4;
            int height = 5;
            string anim = "supermaterial_refinery_kanim";
            int hitpoints = 30;
            float construction_time = 480f;
            float[] tier = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
            string[] all_METALS = MATERIALS.ALL_METALS;
            float melting_point = 2400f;
            BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
            EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER6;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tier, all_METALS, melting_point, build_location_rule, TUNING.BUILDINGS.DECOR.PENALTY.TIER2, tier2, 0.2f);
            buildingDef.RequiresPowerInput = true;
            buildingDef.EnergyConsumptionWhenActive = 1600f;
            buildingDef.SelfHeatKilowattsWhenActive = 16f;
            buildingDef.ViewMode = OverlayModes.Power.ID;
            buildingDef.AudioCategory = "HollowMetal";
            buildingDef.AudioSize = "large";
            return buildingDef;
        }

        //ConfigureBuildingTemplate from SupermaterialRefineryConfig example
        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            go.AddOrGet<DropAllWorkable>();
            ParticleCollider particleCollider = go.AddOrGet<ParticleCollider>();
            particleCollider.heatedTemperature = 313.15f;
            particleCollider.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
            particleCollider.duplicantOperated = true;
            go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
            go.AddOrGet<FabricatorIngredientStatusManager>();
            go.AddOrGet<CopyBuildingSettings>();
            go.AddOrGet<ComplexFabricatorWorkable>();
            BuildingTemplates.CreateComplexFabricatorStorage(go, particleCollider);
            Prioritizable.AddRef(go);

            SetRecipes();
        }

        public void SetRecipes()
        {
            float percentOfFullerene = 0.01f;
            float restOfRecipe = (1f - percentOfFullerene) * 0.5f;


            ComplexRecipe HydrogenToHelium = AddRecipe(new ComplexRecipe.RecipeElement(SimHashes.Hydrogen.CreateTag(), 200f),
            new ComplexRecipe.RecipeElement[]
            {
                new ComplexRecipe.RecipeElement(SimHashes.Helium.CreateTag(), 200f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false)
            });
            HydrogenToHelium.time = 80f;
            HydrogenToHelium.description = STRINGS.BUILDINGS.PREFABS.SUPERMATERIALREFINERY.SUPERCOOLANT_RECIPE_DESCRIPTION;
            HydrogenToHelium.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;

            // nuclear fusion
            //https://fr.wikipedia.org/wiki/R%C3%A9action_triple_alpha
            ComplexRecipe heliumToCarbon = AddRecipe(new ComplexRecipe.RecipeElement(SimHashes.Helium.CreateTag(), 200f),
            new ComplexRecipe.RecipeElement[]
            {
                new ComplexRecipe.RecipeElement(SimHashes.RefinedCarbon.CreateTag(), 160f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false)
                new ComplexRecipe.RecipeElement(SimHashes.Oxygen.CreateTag(), 40f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false)
            });
            heliumToCarbon.time = 80f;
            heliumToCarbon.description = STRINGS.BUILDINGS.PREFABS.SUPERMATERIALREFINERY.SUPERCOOLANT_RECIPE_DESCRIPTION;
            heliumToCarbon.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;


            ComplexRecipe carbonToOxygen = AddRecipe(new ComplexRecipe.RecipeElement(SimHashes.RefinedCarbon.CreateTag(), 200f),
            new ComplexRecipe.RecipeElement[]
            {
                new ComplexRecipe.RecipeElement(SimHashes.Oxygen.CreateTag(), 180f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false)
            });

            carbonToOxygen.time = 80f;
            carbonToOxygen.description = STRINGS.BUILDINGS.PREFABS.SUPERMATERIALREFINERY.SUPERCOOLANT_RECIPE_DESCRIPTION;
            carbonToOxygen.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;

            ComplexRecipe sodiumToAluminium = AddRecipe(new ComplexRecipe.RecipeElement(SimHashes.Salt.CreateTag(), 200f),
            new ComplexRecipe.RecipeElement[]
            {
                new ComplexRecipe.RecipeElement(SimHashes.Aluminum.CreateTag(), 90f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false)
            });
            sodiumToAluminium.time = 80f;
            sodiumToAluminium.description = STRINGS.BUILDINGS.PREFABS.SUPERMATERIALREFINERY.SUPERCOOLANT_RECIPE_DESCRIPTION;
            sodiumToAluminium.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;

            ComplexRecipe aluminiumToPhosphore = AddRecipe(new ComplexRecipe.RecipeElement(SimHashes.Aluminum.CreateTag(), 200f),
            new ComplexRecipe.RecipeElement[]
            {
                new ComplexRecipe.RecipeElement(SimHashes.Sand.CreateTag(), 40f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false),
                new ComplexRecipe.RecipeElement(SimHashes.Phosphorus.CreateTag(), 120f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false),
                new ComplexRecipe.RecipeElement(SimHashes.Sulfur.CreateTag(), 40f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false)
            });
            aluminiumToPhosphore.time = 80f;
            aluminiumToPhosphore.description = STRINGS.BUILDINGS.PREFABS.SUPERMATERIALREFINERY.SUPERCOOLANT_RECIPE_DESCRIPTION;
            aluminiumToPhosphore.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;

            ComplexRecipe siliconToPhosphorus = AddRecipe(new ComplexRecipe.RecipeElement(SimHashes.Sand.CreateTag(), 200f),
            new ComplexRecipe.RecipeElement[]
            {
                new ComplexRecipe.RecipeElement(SimHashes.Phosphorus.CreateTag(), 100f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false),
                new ComplexRecipe.RecipeElement(SimHashes.Sulfur.CreateTag(), 60f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false)
            });
            siliconToPhosphorus.time = 80f;
            siliconToPhosphorus.description = STRINGS.BUILDINGS.PREFABS.SUPERMATERIALREFINERY.SUPERCOOLANT_RECIPE_DESCRIPTION;
            siliconToPhosphorus.nameDisplay = ComplexRecipe.RecipeNameDisplay.Composite;


            ComplexRecipe sulfurToPhosphorus = AddRecipe(new ComplexRecipe.RecipeElement(SimHashes.Sulfur.CreateTag(), 200f),
            new ComplexRecipe.RecipeElement[]
            {
                new ComplexRecipe.RecipeElement(SimHashes.Phosphorus.CreateTag(), 150f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false),
                new ComplexRecipe.RecipeElement(SimHashes.ChlorineGas.CreateTag(), 30f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false)
            });
            sulfurToPhosphorus.time = 80f;
            sulfurToPhosphorus.description = STRINGS.BUILDINGS.PREFABS.SUPERMATERIALREFINERY.SUPERCOOLANT_RECIPE_DESCRIPTION;
            sulfurToPhosphorus.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;


            ComplexRecipe ironToCobalt = AddRecipe(new ComplexRecipe.RecipeElement(SimHashes.Iron.CreateTag(), 200f),
            new ComplexRecipe.RecipeElement[]
            {
                new ComplexRecipe.RecipeElement(SimHashes.Cobalt.CreateTag(), 160f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false),
            });
            ironToCobalt.time = 80f;
            ironToCobalt.description = STRINGS.BUILDINGS.PREFABS.SUPERMATERIALREFINERY.SUPERCOOLANT_RECIPE_DESCRIPTION;
            ironToCobalt.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;

            ComplexRecipe cobaltToCopper = AddRecipe(new ComplexRecipe.RecipeElement(SimHashes.Cobalt.CreateTag(), 200f),
            new ComplexRecipe.RecipeElement[]
            {
                new ComplexRecipe.RecipeElement(SimHashes.Copper.CreateTag(), 150f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false),
            });
            cobaltToCopper.time = 80f;
            cobaltToCopper.description = STRINGS.BUILDINGS.PREFABS.SUPERMATERIALREFINERY.SUPERCOOLANT_RECIPE_DESCRIPTION;
            cobaltToCopper.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;

            ComplexRecipe mercuryToGold = AddRecipe(new ComplexRecipe.RecipeElement(SimHashes.Mercury.CreateTag(), 200f),
            new ComplexRecipe.RecipeElement[]
            {
                new ComplexRecipe.RecipeElement(SimHashes.Gold.CreateTag(), 20f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false),
                new ComplexRecipe.RecipeElement(SimHashes.Lead.CreateTag(), 40f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false),
            });
            mercuryToGold.time = 80f;
            mercuryToGold.description = STRINGS.BUILDINGS.PREFABS.SUPERMATERIALREFINERY.SUPERCOOLANT_RECIPE_DESCRIPTION;
            mercuryToGold.nameDisplay = ComplexRecipe.RecipeNameDisplay.Composite;

            ComplexRecipe mercuryToLead = AddRecipe(new ComplexRecipe.RecipeElement(SimHashes.Mercury.CreateTag(), 200f),
            new ComplexRecipe.RecipeElement[]
            {
                new ComplexRecipe.RecipeElement(SimHashes.Lead.CreateTag(), 150f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false),
            });
            mercuryToLead.time = 80f;
            mercuryToLead.description = STRINGS.BUILDINGS.PREFABS.SUPERMATERIALREFINERY.SUPERCOOLANT_RECIPE_DESCRIPTION;
            mercuryToLead.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;

        }

        private static int currentSortOrder = 100;
        private static ComplexRecipe AddRecipe(ComplexRecipe.RecipeElement[] ingredients, ComplexRecipe.RecipeElement[] results)
        {
            var recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID(ID, ingredients, results), ingredients, results);
            recipe.sortOrder = currentSortOrder;
            currentSortOrder += 10;
            recipe.fabricators = new List<Tag> { TagManager.Create(ID) };
            return recipe;
        }
        private static ComplexRecipe AddRecipe(ComplexRecipe.RecipeElement ingredient, ComplexRecipe.RecipeElement[] results)
        {
            ComplexRecipe.RecipeElement[] ingredients = new ComplexRecipe.RecipeElement[] { ingredient };
            return AddRecipe(ingredients, results);
        }
        private static ComplexRecipe AddRecipe(ComplexRecipe.RecipeElement ingredient, ComplexRecipe.RecipeElement result)
        {
            ComplexRecipe.RecipeElement[] ingredients = new ComplexRecipe.RecipeElement[] { ingredient };
            ComplexRecipe.RecipeElement[] results = new ComplexRecipe.RecipeElement[] { result };
            return AddRecipe(ingredients, results);
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go.GetComponent<KPrefabID>().prefabSpawnFn += delegate (GameObject game_object)
            {
                ComplexFabricatorWorkable component = game_object.GetComponent<ComplexFabricatorWorkable>();
                component.WorkerStatusItem = Db.Get().DuplicantStatusItems.Processing;
                component.AttributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
                component.AttributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
                component.SkillExperienceSkillGroup = Db.Get().SkillGroups.Technicals.Id;
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
