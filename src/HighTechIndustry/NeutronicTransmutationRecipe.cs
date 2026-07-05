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
            radiationCreated = 2000;
        }

        public static NeutronicTransmutationRecipe AddRecipe(NeutronicTransmutationRecipe.RecipeElement ingredient, NeutronicTransmutationRecipe.RecipeElement[] results, float energyRequired, bool containHydrogen = false)
        {
            NeutronicTransmutationRecipe.RecipeElement[] ingredients = new NeutronicTransmutationRecipe.RecipeElement[] { ingredient };
            return AddRecipe(ingredients, results, energyRequired, containHydrogen);
        }
        public static NeutronicTransmutationRecipe AddRecipe(NeutronicTransmutationRecipe.RecipeElement ingredient, NeutronicTransmutationRecipe.RecipeElement result, float energyRequired, bool containHydrogen = false)
        {
            NeutronicTransmutationRecipe.RecipeElement[] ingredients = new NeutronicTransmutationRecipe.RecipeElement[] { ingredient };
            NeutronicTransmutationRecipe.RecipeElement[] results = new NeutronicTransmutationRecipe.RecipeElement[] { result };
            return AddRecipe(ingredients, results, energyRequired, containHydrogen);
        }

        private static int currentSortOrder = 100;
        public static NeutronicTransmutationRecipe AddRecipe(NeutronicTransmutationRecipe.RecipeElement[] ingredients, NeutronicTransmutationRecipe.RecipeElement[] results, float energyRequired, bool containHydrogen = false)
        {

            // Since it's no more a particle collider I don't need a Proton source anymore.
            /*if (!containHydrogen)
                ingredients = ingredients.Append(new RecipeElement(SimHashes.Hydrogen.CreateTag(), ProtonSourceUse, RecipeElement.TemperatureOperation.Heated, true));
            */
            // Same because I use radbolts instead.
            //var neutronSource = new RecipeElement(SimHashes.DepletedUranium.CreateTag(), DepletedUraniumUse, RecipeElement.TemperatureOperation.Heated);
            //neutronSource.possibleMaterials = new Tag[] { SimHashes.DepletedUranium.CreateTag() };
            //ingredients = ingredients.Append(neutronSource);

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
