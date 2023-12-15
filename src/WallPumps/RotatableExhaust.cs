using HarmonyLib;
using UnityEngine;

namespace WallPumps
{
    public class RotatableExhaust : Exhaust
    {
    }

    // The patch to apply UpdateEmission
    // We need to change where the exhaust gets the position to exhaust to. It does so in the middle of UpdateEmission.
    // We completely override that method, basically copying it's code, but replace place where it gets the cell.

    [HarmonyPatch(typeof(Exhaust))]
    [HarmonyPatch("UpdateEmission")]
    public static class Exhaust_UpdateEmission_Patch
    {
        [HarmonyPriority(10000)] // Extremely high priority. We want this to happen first, since this will only overwrite RotatableExhaust results
        public static bool Prefix(Exhaust __instance, ConduitConsumer ___consumer, Storage ___storage)
        {
            if (__instance is RotatableExhaust)
            {
                if (___consumer.ConsumptionRate == 0f) return false; // Don't call super UpdateEmission
                if (___storage.items.Count == 0) return false; // Don't call super UpdateEmission

                Building component = __instance.GetComponent<Building>();
                CellOffset cellOffset = component.GetUtilityOutputOffset();
                int cell = Grid.PosToCell(__instance.transform.GetPosition() + new Vector3(cellOffset.x, cellOffset.y));

                if (Grid.Solid[cell]) return false; // Don't call super UpdateEmission

                ConduitType typeOfConduit = ___consumer.TypeOfConduit;
                if (typeOfConduit != ConduitType.Liquid)
                {
                    if (typeOfConduit == ConduitType.Gas)
                    {
                        // Call private method to emit gas
                        AccessTools.Method(typeof(Exhaust), "EmitGas").Invoke(__instance, new object[] { cell });
                        return false; // Don't call super UpdateEmission
                    }
                }
                else
                {
                    // Call private method to emit liquid
                    AccessTools.Method(typeof(Exhaust), "EmitLiquid").Invoke(__instance, new object[] { cell });
                    return false; // Don't call super UpdateEmission
                }
            }
            return true; // Call super UpdateEmission
        }
    }
}
