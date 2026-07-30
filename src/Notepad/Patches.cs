using HarmonyLib;
using KMod;
using PeterHan.PLib.Database;
using PeterHan.PLib.Options;
using PeterHan.PLib.UI;
using Pholib;
using System.Collections.Generic;
using UnityEngine;
using static Pholib.Utilities;
using static TUNING.BUILDINGS;

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
            Utilities.GenerateStringsTemplate(typeof(PHO_STRINGS));
        }
    }

    [HarmonyPatch(typeof(Db), "Initialize")]
    public static class NotepadTechPatch
    {
        public static void Postfix()
        {
            AddBuildingTech("InteriorDecor", NotepadConfig.ID);
            AddBuildingTech("PowerRegulation", LEDConfig.ID);


            GameObject o = new GameObject();
            o.AddComponent<NotepadSideScreen>();

            Strings.Add(new string[] { "STRINGS.UI.KLEI_INVENTORY_SCREEN.SUBCATEGORIES.BUILDING_NOTEPAD", PHO_STRINGS.NOTEPAD.NAME });
            Strings.Add(new string[] { "STRINGS.UI.KLEI_INVENTORY_SCREEN.SUBCATEGORIES.BUILDING_LED", PHO_STRINGS.LED.NAME });
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
            AddBuilding("Furniture", NotepadConfig.ID, PHO_STRINGS.NOTEPAD.NAME, PHO_STRINGS.NOTEPAD.DESC, PHO_STRINGS.NOTEPAD.EFFECT,
                PlanSubcategoryName.decor, WoodSculptureConfig.ID);

            AddBuilding("Automation", LEDConfig.ID, PHO_STRINGS.LED.NAME, PHO_STRINGS.LED.DESC, PHO_STRINGS.LED.EFFECT,
                PlanSubcategoryName.logicmanager, LogicAlarmConfig.ID);
        }
    }
    // Initiate NotepadSideScreen
    [HarmonyPatch(typeof(DetailsScreen), "OnPrefabInit")]
    public class DetailsScreen_OnPrefabInit_Patch
    {
        public static void Postfix()
        {
            PUIUtils.AddSideScreenContent<NotepadSideScreen>();
        }
    }
    // Show text when hovering the notepad
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
                    }
                    ;
                    notepadTooltipFontStyle.fontSize = pad.tooltipFontSize;
                    HoverTextScreen instance = HoverTextScreen.Instance;
                    HoverTextDrawer hover = instance.BeginDrawing();
                    hover.BeginShadowBar();
                    if (Assets.GetSprite(pad.iconName) != null)
                        hover.DrawIcon(Assets.GetSprite(pad.iconName), (int)PUIUtils.GetLineHeight(notepadTooltipFontStyle) + 5);
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
}
