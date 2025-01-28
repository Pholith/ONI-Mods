using Database;
using HarmonyLib;
using KMod;
using PeterHan.PLib.Database;
using PeterHan.PLib.Options;
using PeterHan.PLib.UI;
using Pholib;
using System;
using System.Collections.Generic;
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


    [HarmonyPatch(typeof(Db), "Initialize")]
    public static class NotepadTechPatch
    {
        public static void Postfix()
        {
            AddBuildingTech("InteriorDecor", NotepadConfig.ID);

            GameObject o = new GameObject();
            o.AddComponent<NotepadSideScreen>();

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
        public static void Postfix(SelectToolHoverTextCard __instance, List<KSelectable> hoverObjects)
        {

            foreach (KSelectable selectable in hoverObjects)
            {
                Notepad pad = selectable.gameObject.GetComponent<Notepad>();
                if (pad != null)
                {
                    HoverTextScreen instance = HoverTextScreen.Instance;
                    HoverTextDrawer hover = instance.BeginDrawing();
                    hover.BeginShadowBar();
                    hover.DrawIcon(Assets.GetSprite("icon_category_furniture"), 20);
                    hover.DrawText(pad.activateText, __instance.ToolTitleTextStyle);
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

    // Skin patchs
    [HarmonyPatch(typeof(InventoryOrganization))]
    [HarmonyPatch("GenerateSubcategories")]
    public static class InventoryOrganization_GenerateSubcategories_NotepadPatch
    {
        public static readonly string[] SkinIDs = new string[]
{
            "Notepad",
            "Notepad_2",
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
            __instance.Add(new BuildingFacadeResource("Notepad", PHO_STRINGS.NOTEPAD_B.NAME, PHO_STRINGS.NOTEPAD_B.DESC, PermitRarity.Universal, NotepadConfig.ID, "notepad_kanim", DlcManager.AVAILABLE_ALL_VERSIONS, null));
            __instance.Add(new BuildingFacadeResource("Notepad_2", PHO_STRINGS.NOTEPAD_B.NAME, PHO_STRINGS.NOTEPAD_B.DESC, PermitRarity.Universal, NotepadConfig.ID, "notepad2_kanim", DlcManager.AVAILABLE_ALL_VERSIONS, null));
        }
    }

}
