using System.Collections.Generic;
using UnityEngine;

namespace ILoveSlicksters
{
    class OilFloaters
    {
        public static GameObject SetupDiet(GameObject prefab, List<Diet.Info> diet_infos, float caloriesPerKg, float minPoopSizeInKg, float consumptionRate = 5f)
        {
            Diet diet = new Diet(diet_infos.ToArray());
            CreatureCalorieMonitor.Def def = prefab.AddOrGetDef<CreatureCalorieMonitor.Def>();
            def.diet = diet;
            def.minConsumedCaloriesBeforePooping = minPoopSizeInKg * caloriesPerKg;
            GasAndLiquidConsumerMonitor.Def def2 = prefab.AddOrGetDef<GasAndLiquidConsumerMonitor.Def>();
            def2.diet = diet;
            def2.consumptionRate = consumptionRate;
            return prefab;
        }
    }
}
