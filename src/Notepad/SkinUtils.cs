using Database;
using HarmonyLib;
using Pholib;
using System;
using System.Linq;
using UnityEngine;

namespace Notepad
{
    public class SkinUtils
    {
        public static readonly string[] NotepadSkinIDs = new string[]
        {
            "blackboard",
            "blueprint",
            "postit",
            "stonks",
            "tv",
            "warning",
            "info",
        };

        public static readonly string[] LEDSkinIDs = new string[]
        {
            "red_green_led",
            "pixel",
            "diode",
        };


        // SKINS: Patch the background of the Anywhere building location
        [HarmonyPatch(typeof(KleiPermitDioramaVis))]
        [HarmonyPatch("GetPermitVisTarget")]
        public static class KleiPermitDioramaVis_GetPermitVisTarget_NotepadPatch
        {
            public static void Postfix(KleiPermitDioramaVis __instance, PermitResource permit, ref IKleiPermitDioramaVisTarget __result, KleiPermitDioramaVis_Fallback ___fallbackVis, KleiPermitDioramaVis_AutomationGates ___buildingAutomationGatesVis)
            {
                if ((object)__result == ___fallbackVis && permit.Category == PermitCategory.Building && KleiPermitVisUtil.GetBuildLocationRule(permit) == BuildLocationRule.Anywhere)
                {
                    __result = ___buildingAutomationGatesVis;
                }
            }
        }

        // Skin patchs. Create a category
        [HarmonyPatch(typeof(InventoryOrganization))]
        [HarmonyPatch("GenerateSubcategories")]
        public static class Inventory_GenSubcats_Notepad
        {
            public static void Postfix()
            {
                Utilities.AddSkinSubcategory("BUILDINGS", "BUILDING_NOTEPAD", Def.GetUISprite("Notepad", "ui", false).first, 130, NotepadSkinIDs);
                Utilities.AddSkinSubcategory("BUILDINGS", "BUILDING_LED", Def.GetUISprite("led", "ui", false).first, 131, LEDSkinIDs);
            }
        }

        // SKINS: Patch this background to make the Notepad bigger
        [HarmonyPatch(typeof(KleiPermitDioramaVis_AutomationGates))]
        [HarmonyPatch(nameof(KleiPermitDioramaVis_AutomationGates.ConfigureWith))]
        public static class KleiPermitDioramaVis_AutomationGates_NotepadPatch
        {
            public static void Postfix(PermitResource permit, KBatchedAnimController ___buildingKAnim)
            {
                if (NotepadSkinIDs.Contains(permit.Id))
                {
                    ___buildingKAnim.rectTransform().localScale = Vector3.one * 1.8f;
                    ___buildingKAnim.rectTransform().anchoredPosition -= new Vector2(0f, 26f); // 16 was too high and 32 too low
                }
            }
        }
        // Skin patchs
        [HarmonyPatch(typeof(BuildingFacades), MethodType.Constructor, new Type[] { typeof(ResourceSet) })]
        public static class BuildingFacades_Constructor_NotepadPatch
        {
            public static void Postfix(ResourceSet<BuildingFacadeResource> __instance)
            {
                __instance.Add(new BuildingFacadeResource(NotepadSkinIDs[0], PHO_STRINGS.BLACKBOARD.NAME, PHO_STRINGS.BLACKBOARD.DESC, PermitRarity.Universal, NotepadConfig.ID, NotepadSkinIDs[0] + "_kanim", null, null, null, null));
                __instance.Add(new BuildingFacadeResource(NotepadSkinIDs[1], PHO_STRINGS.BLUEPRINT.NAME, PHO_STRINGS.BLUEPRINT.DESC, PermitRarity.Universal, NotepadConfig.ID, NotepadSkinIDs[1] + "_kanim", null, null, null, null));
                __instance.Add(new BuildingFacadeResource(NotepadSkinIDs[2], PHO_STRINGS.POSTIT.NAME, PHO_STRINGS.POSTIT.DESC, PermitRarity.Universal, NotepadConfig.ID, NotepadSkinIDs[2] + "_kanim", null, null, null, null));
                __instance.Add(new BuildingFacadeResource(NotepadSkinIDs[3], PHO_STRINGS.STONKS.NAME, PHO_STRINGS.STONKS.DESC, PermitRarity.Universal, NotepadConfig.ID, NotepadSkinIDs[3] + "_kanim", null, null, null, null));
                __instance.Add(new BuildingFacadeResource(NotepadSkinIDs[4], PHO_STRINGS.TV.NAME, PHO_STRINGS.TV.DESC, PermitRarity.Universal, NotepadConfig.ID, NotepadSkinIDs[4] + "_kanim", null, null, null, null));
                __instance.Add(new BuildingFacadeResource(NotepadSkinIDs[5], PHO_STRINGS.WARNING.NAME, PHO_STRINGS.WARNING.DESC, PermitRarity.Universal, NotepadConfig.ID, NotepadSkinIDs[5] + "_kanim", null, null, null, null));
                __instance.Add(new BuildingFacadeResource(NotepadSkinIDs[6], PHO_STRINGS.INFO.NAME, PHO_STRINGS.INFO.DESC, PermitRarity.Universal, NotepadConfig.ID, NotepadSkinIDs[6] + "_kanim", null, null, null, null));

                __instance.Add(new BuildingFacadeResource(LEDSkinIDs[0], PHO_STRINGS.LED_RED_GREEN.NAME, PHO_STRINGS.LED_RED_GREEN.DESC, PermitRarity.Universal, LEDConfig.ID, LEDSkinIDs[0] + "_kanim", null, null, null, null));
                __instance.Add(new BuildingFacadeResource(LEDSkinIDs[1], PHO_STRINGS.PIXEL.NAME, PHO_STRINGS.PIXEL.DESC, PermitRarity.Universal, LEDConfig.ID, LEDSkinIDs[1] + "_kanim", null, null, null, null));
                __instance.Add(new BuildingFacadeResource(LEDSkinIDs[2], PHO_STRINGS.DIODE.NAME, PHO_STRINGS.DIODE.DESC, PermitRarity.Universal, LEDConfig.ID, LEDSkinIDs[2] + "_kanim", null, null, null, null));

            }
        }
    }
}
