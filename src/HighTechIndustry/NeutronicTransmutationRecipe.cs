using System.Collections.Generic;

namespace HighTechIndustry
{
    public class NeutronicTransmutationRecipe : ComplexRecipe
    {

        public float energyRequired;

        public int radiationCreated;

        /* Deprecated
        public const float DepletedUraniumUse = 2f;
        public const float ProtonSourceUse = 1f;
        public const float GenericTotalWaste = 3f;
        */

        public NeutronicTransmutationRecipe(string id, RecipeElement[] ingredients, RecipeElement[] results, float energyRequired) : base(id, ingredients, results)
        {
            this.energyRequired = energyRequired;
            radiationCreated = 100000;
        }

        public static NeutronicTransmutationRecipe AddRecipe(NeutronicTransmutationRecipe.RecipeElement ingredient, NeutronicTransmutationRecipe.RecipeElement[] results, float energyRequired)
        {
            NeutronicTransmutationRecipe.RecipeElement[] ingredients = new NeutronicTransmutationRecipe.RecipeElement[] { ingredient };
            return AddRecipe(ingredients, results, energyRequired);
        }
        public static NeutronicTransmutationRecipe AddRecipe(NeutronicTransmutationRecipe.RecipeElement ingredient, NeutronicTransmutationRecipe.RecipeElement result, float energyRequired)
        {
            NeutronicTransmutationRecipe.RecipeElement[] ingredients = new NeutronicTransmutationRecipe.RecipeElement[] { ingredient };
            NeutronicTransmutationRecipe.RecipeElement[] results = new NeutronicTransmutationRecipe.RecipeElement[] { result };
            return AddRecipe(ingredients, results, energyRequired);
        }

        private static int currentSortOrder = 100;
        public static NeutronicTransmutationRecipe AddRecipe(NeutronicTransmutationRecipe.RecipeElement[] ingredients, NeutronicTransmutationRecipe.RecipeElement[] results, float energyRequired)
        {

            var recipe = new NeutronicTransmutationRecipe(ComplexRecipeManager.MakeRecipeID(NeutronicTransmutationChamberConfig.ID, ingredients, results), ingredients, results, energyRequired)
            {
                sortOrder = currentSortOrder,
                producedHEP = 0,
                fabricators = new List<Tag> { TagManager.Create(NeutronicTransmutationChamberConfig.ID) }
            };
            currentSortOrder += 10;
            return recipe;
        }

    }
}
