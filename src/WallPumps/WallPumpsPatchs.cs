using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Database;
using PeterHan.PLib.Options;
using Pholib;
using System.Collections.Generic;

namespace WallPumps
{
    public class WallPumpsPatchs : KMod.UserMod2
    {
        public static string modPath;
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            PUtil.InitLibrary();

            modPath = this.path;
            new POptions().RegisterOptions(this, typeof(WallPumpOptions));
            GameOnLoadPatch.ReadSettings();
        }
    }
    [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
    public static class WallPumpStringsPatch
    {
        public static void Prefix()
        {
            Pholib.Utilities.AddBuildingStrings(GasWallPump.ID, WALLPUMP_STRINGS.BUILDINGS.PREFABS.FAIRGASWALLPUMP.NAME,
    WALLPUMP_STRINGS.BUILDINGS.PREFABS.FAIRGASWALLPUMP.DESC, WALLPUMP_STRINGS.BUILDINGS.PREFABS.FAIRGASWALLPUMP.EFFECT);
            Pholib.Utilities.AddBuildingStrings(GasWallVent.ID, WALLPUMP_STRINGS.BUILDINGS.PREFABS.FAIRGASWALLVENT.NAME,
                WALLPUMP_STRINGS.BUILDINGS.PREFABS.FAIRGASWALLVENT.DESC, WALLPUMP_STRINGS.BUILDINGS.PREFABS.FAIRGASWALLVENT.EFFECT);
            Pholib.Utilities.AddBuildingStrings(GasWallVentHighPressure.ID, WALLPUMP_STRINGS.BUILDINGS.PREFABS.VENTHIGHPRESSURE.NAME,
                WALLPUMP_STRINGS.BUILDINGS.PREFABS.VENTHIGHPRESSURE.DESC, WALLPUMP_STRINGS.BUILDINGS.PREFABS.VENTHIGHPRESSURE.EFFECT);

            Pholib.Utilities.AddBuildingStrings(LiquidWallPump.ID, WALLPUMP_STRINGS.BUILDINGS.PREFABS.FAIRLIQUIDWALLPUMP.NAME,
                WALLPUMP_STRINGS.BUILDINGS.PREFABS.FAIRLIQUIDWALLPUMP.DESC, WALLPUMP_STRINGS.BUILDINGS.PREFABS.FAIRLIQUIDWALLPUMP.EFFECT);
            Pholib.Utilities.AddBuildingStrings(LiquidWallVent.ID, WALLPUMP_STRINGS.BUILDINGS.PREFABS.FAIRLIQUIDWALLVENT.NAME,
                WALLPUMP_STRINGS.BUILDINGS.PREFABS.FAIRLIQUIDWALLVENT.DESC, WALLPUMP_STRINGS.BUILDINGS.PREFABS.FAIRLIQUIDWALLVENT.EFFECT);

        }
    }
    // Load PLib settings on game load
    [HarmonyPatch(typeof(Game), "Load")]
    public static class GameOnLoadPatch
    {
        public static WallPumpOptions Settings { get; private set; }

        public static void Prefix()
        {
            ReadSettings();
        }
        public static void ReadSettings()
        {
            // read the option each time the game is loaded - so we don't need to restart all the game
            Settings = POptions.ReadSettings<WallPumpOptions>();
            if (Settings == null)
            {
                Settings = new WallPumpOptions();
            }
        }
    }

    [HarmonyPatch(typeof(Localization))]
    [HarmonyPatch("Initialize")]
    public static class Localization_Initialize_Patch
    {
        public static void Postfix()
        {
            Utilities.LoadTranslations(typeof(WALLPUMP_STRINGS), WallPumpsPatchs.modPath);
            LocString.CreateLocStringKeys(typeof(WALLPUMP_STRINGS.UI_ADD));
            Pholib.Utilities.GenerateStringsTemplate(typeof(WALLPUMP_STRINGS));

        }
    }
    [HarmonyPatch(typeof(Db))]
    [HarmonyPatch("Initialize")]
    public static class Db_Initialize_Patch
    {
        public static void Postfix()
        {

            // Add tech tree/building menu entries
            GasWallPump.AddToMenus();
            LiquidWallPump.AddToMenus();
            GasWallVent.AddToMenus();
            GasWallVentHighPressure.AddToMenus();
            LiquidWallVent.AddToMenus();
        }
    }


}
