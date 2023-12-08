using HarmonyLib;
using KMod;
using PeterHan.PLib.Database;
using PeterHan.PLib.Options;
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

            new PLocalization().Register();
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
    public static class DupRoomSensorStringsPatch
    {
        public static void Prefix()
        {
            AddBuilding("Furniture", NotepadConfig.ID, PHO_STRINGS.NOTEPAD.NAME, PHO_STRINGS.NOTEPAD.DESC, PHO_STRINGS.NOTEPAD.EFFECT);
        }
    }


    [HarmonyPatch(typeof(Db), "Initialize")]
    public static class DupRoomSensorTechPatch
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
            List<DetailsScreen.SideScreenRef> sideScreens = Traverse.Create(DetailsScreen.Instance).Field("sideScreens").GetValue<List<DetailsScreen.SideScreenRef>>();
            GameObject sideScreenContentBody = Traverse.Create(DetailsScreen.Instance).Field("sideScreenContentBody").GetValue<GameObject>();


            NotepadControl controller = new NotepadControl();
            NotepadSideScreen screen = controller.RootPanel.AddComponent<NotepadSideScreen>();

            screen.Control = controller;
            screen.gameObject.transform.parent = sideScreenContentBody.transform;
            screen.gameObject.transform.localScale = Vector3.one;

            DetailsScreen.SideScreenRef myRef = new DetailsScreen.SideScreenRef
            {
                name = "NotepadSideScreen",
                screenPrefab = screen,
                offset = new Vector2(0f, 0f),
                screenInstance = screen
            };
            sideScreens.Add(myRef);
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
}
