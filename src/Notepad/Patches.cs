using Database;
using HarmonyLib;
using KMod;
using PeterHan.PLib.Database;
using PeterHan.PLib.Options;
using PeterHan.PLib.UI;
using Pholib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using static Pholib.Utilities;

namespace Notepad
{
    public class NotepadMod : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            new POptions().RegisterOptions(this, typeof(NotepadOptions));
            GameOnLoadPatch.ReadSettings(); // Read settings early for the notepad description setting.

            new PLocalization().Register();
            GenerateStringsTemplate(typeof(PHO_STRINGS));
        }
    }

    [HarmonyPatch(typeof(Db), "Initialize")]
    public static class NotepadTechPatch
    {
        public static void Postfix()
        {
            AddBuildingTech("InteriorDecor", NotepadConfig.ID);

            GameObject o = new GameObject();
            o.AddComponent<NotepadSideScreen>();

            Strings.Add(new string[] { "STRINGS.UI.KLEI_INVENTORY_SCREEN.SUBCATEGORIES.BUILDING_NOTEPAD", PHO_STRINGS.NOTEPAD.NAME });
        }
    }

    // Load PLib settings on game load
    [HarmonyPatch(typeof(Game), "Load")]
    public static class GameOnLoadPatch
    {
        public static NotepadOptions Settings { get; private set; }

        public static void Prefix()
        {
            ReadSettings();
        }
        public static void ReadSettings()
        {
            // read the option each time the game is loaded - so we don't need to restart all the game
            Settings = POptions.ReadSettings<NotepadOptions>();
            if (Settings == null)
            {
                Settings = new NotepadOptions();
            }

        }
    }

    [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
    public static class NotepadStringsPatch
    {
        public static void Prefix()
        {
            AddBuilding("Furniture", NotepadConfig.ID, PHO_STRINGS.NOTEPAD.NAME, PHO_STRINGS.NOTEPAD.DESC, PHO_STRINGS.NOTEPAD.EFFECT);
        }
    }

    [HarmonyPatch(typeof(DetailsScreen), "OnPrefabInit")]
    public class DetailsScreen_OnPrefabInit_Patch
    {

        public static void Postfix()
        {
            PUIUtils.AddSideScreenContent<NotepadSideScreen>();
        }
    }

    [HarmonyPatch(typeof(SelectToolHoverTextCard))]
    [HarmonyPatch("UpdateHoverElements")]
    public class HoverText_ConfigureTitlePatch
    {
        public static TextStyleSetting notepadTooltipFontStyle = null;

        public static void Postfix(SelectToolHoverTextCard __instance, List<KSelectable> hoverObjects)
        {
            foreach (KSelectable selectable in hoverObjects)
            {
                Notepad pad = selectable.gameObject.GetComponent<Notepad>();
                if (pad != null)
                {
                    if (notepadTooltipFontStyle == null)
                    {
                        notepadTooltipFontStyle = __instance.Styles_Title.Standard.DeriveStyle();
                    };
                    notepadTooltipFontStyle.fontSize = pad.tooltipFontSize;
                    HoverTextScreen instance = HoverTextScreen.Instance;
                    HoverTextDrawer hover = instance.BeginDrawing();
                    hover.BeginShadowBar();
                    if (Assets.GetSprite(pad.iconName) != null)
                        hover.DrawIcon(Assets.GetSprite(pad.iconName), (int) PUIUtils.GetLineHeight(notepadTooltipFontStyle) + 5);
                    hover.DrawText(pad.contentText, notepadTooltipFontStyle);
                    hover.EndShadowBar();
                    hover.EndDrawing();
                }
            }
        }
    }

    // Instant Build patch
    [HarmonyPatch(typeof(BuildingDef))]
    [HarmonyPatch(nameof(BuildingDef.Instantiate))]
    public static class BuildingDef_Instantiate_Patch
    {
        public static bool Prefix(Vector3 pos, Orientation orientation, IList<Tag> selected_elements, int layer, BuildingDef __instance, ref GameObject __result)
        {
            if (__instance.PrefabID != NotepadConfig.ID || !GameOnLoadPatch.Settings.InstantBuild) return true;
            else
            {
                __instance.Build(Grid.PosToCell(pos), orientation, null, selected_elements, 293.15f, playsound: false, GameClock.Instance.GetTime());
                return false;
            }

        }
    }


    // Patch the background of the Anywhere building location
    [HarmonyPatch(typeof(KleiPermitDioramaVis))]
    [HarmonyPatch("GetPermitVisTarget")]
    public static class KleiPermitDioramaVis_GetPermitVisTarget_NotepadPatch
    {
        public static void Postfix(KleiPermitDioramaVis __instance, PermitResource permit, ref IKleiPermitDioramaVisTarget __result, KleiPermitDioramaVis_Fallback ___fallbackVis, KleiPermitDioramaVis_AutomationGates ___buildingAutomationGatesVis)
        {
            if (__result == ___fallbackVis && permit.Category == PermitCategory.Building && KleiPermitVisUtil.GetBuildLocationRule(permit) == BuildLocationRule.Anywhere)
            {
                __result = ___buildingAutomationGatesVis;
            }
        }
    }

    // Patch this background to make the Notepad bigger
    [HarmonyPatch(typeof(KleiPermitDioramaVis_AutomationGates))]
    [HarmonyPatch(nameof(KleiPermitDioramaVis_AutomationGates.ConfigureWith))]
    public static class KleiPermitDioramaVis_AutomationGates_NotepadPatch
    {
        public static void Postfix(PermitResource permit, KBatchedAnimController ___buildingKAnim)
        {
            if (Inventory_GenSubcats_Notepad.SkinIDs.Contains(permit.Id))
            {
                ___buildingKAnim.rectTransform().localScale = Vector3.one * 1.3f;
                ___buildingKAnim.rectTransform().anchoredPosition -= new Vector2(0f, 26f); // 16 was too high and 32 too low
            }
        }
    }


    // Skin patchs
    [HarmonyPatch(typeof(InventoryOrganization))]
    [HarmonyPatch("GenerateSubcategories")]
    public static class Inventory_GenSubcats_Notepad
    {
        public static readonly string[] SkinIDs = new string[]
    {
            "blackboard",
            "blueprint",
            "postit",
            "stonks",
            "tv",
            "warning",
    };

        public static void Postfix()
        {
            Utilities.AddSkinSubcategory("BUILDINGS", "BUILDING_NOTEPAD", Def.GetUISprite("Notepad", "ui", false).first, 130, SkinIDs);
        }
    }

    // Skin patchs
    [HarmonyPatch(typeof(BuildingFacades), MethodType.Constructor, new Type[] { typeof(ResourceSet) })]
    public static class BuildingFacades_Constructor_NotepadPatch
    {
        public static void Postfix(ResourceSet<BuildingFacadeResource> __instance)
        {
            __instance.Add(new BuildingFacadeResource(Inventory_GenSubcats_Notepad.SkinIDs[0], PHO_STRINGS.BLACKBOARD.NAME, PHO_STRINGS.BLACKBOARD.DESC, PermitRarity.Universal, NotepadConfig.ID, Inventory_GenSubcats_Notepad.SkinIDs[0] + "_kanim", DlcManager.AVAILABLE_ALL_VERSIONS, null));
            __instance.Add(new BuildingFacadeResource(Inventory_GenSubcats_Notepad.SkinIDs[1], PHO_STRINGS.BLUEPRINT.NAME, PHO_STRINGS.BLUEPRINT.DESC, PermitRarity.Universal, NotepadConfig.ID, Inventory_GenSubcats_Notepad.SkinIDs[1] + "_kanim", DlcManager.AVAILABLE_ALL_VERSIONS, null));
            __instance.Add(new BuildingFacadeResource(Inventory_GenSubcats_Notepad.SkinIDs[2], PHO_STRINGS.POSTIT.NAME, PHO_STRINGS.POSTIT.DESC, PermitRarity.Universal, NotepadConfig.ID, Inventory_GenSubcats_Notepad.SkinIDs[2] + "_kanim", DlcManager.AVAILABLE_ALL_VERSIONS, null));
            __instance.Add(new BuildingFacadeResource(Inventory_GenSubcats_Notepad.SkinIDs[3], PHO_STRINGS.STONKS.NAME, PHO_STRINGS.STONKS.DESC, PermitRarity.Universal, NotepadConfig.ID, Inventory_GenSubcats_Notepad.SkinIDs[3] + "_kanim", DlcManager.AVAILABLE_ALL_VERSIONS, null));
            __instance.Add(new BuildingFacadeResource(Inventory_GenSubcats_Notepad.SkinIDs[4], PHO_STRINGS.TV.NAME, PHO_STRINGS.TV.DESC, PermitRarity.Universal, NotepadConfig.ID, Inventory_GenSubcats_Notepad.SkinIDs[4] + "_kanim", DlcManager.AVAILABLE_ALL_VERSIONS, null));
            __instance.Add(new BuildingFacadeResource(Inventory_GenSubcats_Notepad.SkinIDs[5], PHO_STRINGS.WARNING.NAME, PHO_STRINGS.WARNING.DESC, PermitRarity.Universal, NotepadConfig.ID, Inventory_GenSubcats_Notepad.SkinIDs[5] + "_kanim", DlcManager.AVAILABLE_ALL_VERSIONS, null));
        }
    }

}
