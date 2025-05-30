using System.Collections.Generic;
using System.Linq;

namespace ParticleCollider
{
    public class ColliderRecipe : ComplexRecipe
    {

        public int energyRequired;

        public int radiationCreated;

        public const float DepletedUraniumUse = 2f;
        public const float ProtonSourceUse = 1f;
        public const float GenericTotalWaste = 3f;


        public ColliderRecipe(string id, RecipeElement[] ingredients, RecipeElement[] results, int energyRequired) : base(id, ingredients, results)
        {
            this.energyRequired = energyRequired;
            radiationCreated = 2000;
        }

        public static ColliderRecipe AddRecipe(ColliderRecipe.RecipeElement ingredient, ColliderRecipe.RecipeElement[] results, int energyRequired, bool containHydrogen = false)
        {
            ColliderRecipe.RecipeElement[] ingredients = new ColliderRecipe.RecipeElement[] { ingredient };
            return AddRecipe(ingredients, results, energyRequired, containHydrogen);
        }
        public static ColliderRecipe AddRecipe(ColliderRecipe.RecipeElement ingredient, ColliderRecipe.RecipeElement result, int energyRequired, bool containHydrogen = false)
        {
            ColliderRecipe.RecipeElement[] ingredients = new ColliderRecipe.RecipeElement[] { ingredient };
            ColliderRecipe.RecipeElement[] results = new ColliderRecipe.RecipeElement[] { result };
            return AddRecipe(ingredients, results, energyRequired, containHydrogen);
        }

        private static int currentSortOrder = 100;
        public static ColliderRecipe AddRecipe(ColliderRecipe.RecipeElement[] ingredients, ColliderRecipe.RecipeElement[] results, int energyRequired, bool containHydrogen = false)
        {
            if (!containHydrogen)
                ingredients = ingredients.Append(new RecipeElement(SimHashes.Hydrogen.CreateTag(), ProtonSourceUse, RecipeElement.TemperatureOperation.Heated, true));

            var neutronSource = new RecipeElement(SimHashes.DepletedUranium.CreateTag(), DepletedUraniumUse, RecipeElement.TemperatureOperation.Heated);
            neutronSource.possibleMaterials = new Tag[] { SimHashes.DepletedUranium.CreateTag() };

            ingredients = ingredients.Append(neutronSource);
            var recipe = new ColliderRecipe(ComplexRecipeManager.MakeRecipeID(ParticleColliderConfig.ID, ingredients, results), ingredients, results, energyRequired)
            {
                sortOrder = currentSortOrder,
                producedHEP = 0,
                fabricators = new List<Tag> { TagManager.Create(ParticleColliderConfig.ID) }
            };
            currentSortOrder += 10;
            return recipe;
        }

    }
}
