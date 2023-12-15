using KSerialization;
using UnityEngine;
using HarmonyLib;

namespace WallPumps
{

    [SkipSaveFileSerialization]
    [SerializationConfig(MemberSerialization.OptIn)]
    public class RotatableElementConsumer : ElementConsumer
    {
        // When using this, remember to always use RotatablePump too instead of Pump
        [SerializeField]
        public Vector3 rotatableCellOffset; // Set this instead, since sampleCellOffset will be constantly overridden
    }

    // The patch to apply GetSampleCell

    [HarmonyPatch(typeof(ElementConsumer))]
    [HarmonyPatch("GetSampleCell")]
    public static class ElementConsumer_GetSampleCell_Patch
    {
        [HarmonyPriority(-10000)] // Extremely low priority. We want this to happen last, since this will only overwrite RotatableElementConsumer variable
        public static void Prefix(ElementConsumer __instance)
        {
            if (__instance is RotatableElementConsumer)
            {
                Vector3 rotatableCellOffset = ((RotatableElementConsumer)__instance).rotatableCellOffset;
                Rotatable rotatable = __instance.GetComponent<Rotatable>();
                if (rotatable != null) __instance.sampleCellOffset = Rotatable.GetRotatedOffset(rotatableCellOffset, rotatable.GetOrientation());
                //Debug.Log("GetSampleCell call " + rotatableCellOffset + ", " + __instance.sampleCellOffset);
            }
        }
    }
}
