using HarmonyLib;
using Pholib;
using UnityEngine;

namespace CustomizeYourPaints.Art
{

    [HarmonyPatch(typeof(CanvasConfig), "DoPostConfigureComplete")]
    public class CanvasConfig_DoPostConfigureComplete_Patch
    {
        public static void ModifyGameObject(GameObject go) {
            go.AddComponent<ArtOverride>().customExtraStages = CustomizeYourPaints.myOverrides;
            go.AddComponent<ArtOverrideRestorer>().fallback = "Default";
            go.GetComponent<KPrefabID>().prefabSpawnFn += delegate (GameObject g)
            {
                CustomizeYourPaints.artRestorers.Add(g.GetComponent<ArtOverrideRestorer>());
            };
        }
        public static void Postfix(GameObject go)
        {
            ModifyGameObject(go);
            go.AddComponent<ArtOverride>();
        }
    }
    [HarmonyPatch(typeof(CanvasTallConfig), "DoPostConfigureComplete")]
    public class CanvasTallConfig_DoPostConfigureComplete_Patch
    {
        public static void Postfix(GameObject go)
        {
            CanvasConfig_DoPostConfigureComplete_Patch.ModifyGameObject(go);
        }
    }
    [HarmonyPatch(typeof(CanvasWideConfig), "DoPostConfigureComplete")]
    public class CanvasWideConfig_DoPostConfigureComplete_Patch
    {
        public static void Postfix(GameObject go)
        {

            CanvasConfig_DoPostConfigureComplete_Patch.ModifyGameObject(go);
        }
    }
}
