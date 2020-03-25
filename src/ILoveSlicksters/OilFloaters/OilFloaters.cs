using System.Collections.Generic;
using UnityEngine;

namespace ILoveSlicksters
{
    class OilFloaters
    {
        public static GameObject SetupDiet(GameObject prefab, List<Diet.Info> diet_infos, float CALORIES_PER_KG_OF_ORE, float minPoopSizeInKg, float consumptionRate = 3f)
        {
            Diet diet = new Diet(diet_infos.ToArray());
            CreatureCalorieMonitor.Def def = prefab.AddOrGetDef<CreatureCalorieMonitor.Def>();
            def.diet = diet;
            def.minPoopSizeInCalories = CALORIES_PER_KG_OF_ORE * minPoopSizeInKg;
            GasAndLiquidConsumerMonitor.Def def2 = prefab.AddOrGetDef<GasAndLiquidConsumerMonitor.Def>();
            def2.diet = diet;
            def2.consumptionRate = consumptionRate;
            return prefab;
        }
    }
}
