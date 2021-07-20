using HarmonyLib;
using System;
using System.Collections.Generic;

namespace AchievementReEnabler
{

    // modify the save File to toggle debugWasUsed to false
    [HarmonyPatch(typeof(Game))]
    [HarmonyPatch("Load")]
    internal class AchievementPatch
    {

        public static void Postfix(Game __instance)
        {
            __instance.debugWasUsed = false;
        }
    }


    // Steam Unlock achievment that were unlocked when debugWasUsed was true
    [HarmonyPatch(typeof(ColonyAchievementTracker))]
    [HarmonyPatch("OnSpawn")]
    internal class AchievementsReactiver
    {

        public static void Postfix(ColonyAchievementTracker __instance)
        {
            foreach (KeyValuePair<string, ColonyAchievementStatus> keyValuePair in __instance.achievements)
            {
                if (keyValuePair.Value.success && !keyValuePair.Value.failed)
                {

                    Type[] parameters_type = new Type[] { typeof(string) };
                    object[] paramaters = new object[] { keyValuePair.Key };

                    Traverse.Create<ColonyAchievementTracker>().Method("UnlockPlatformAchievement", parameters_type).GetValue(paramaters);
                }
            }

        }
    }
}
