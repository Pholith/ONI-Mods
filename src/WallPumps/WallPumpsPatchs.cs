using HarmonyLib;
using PeterHan.PLib.Core;
using PeterHan.PLib.Database;
using PeterHan.PLib.Options;
using System;
using static Localization;
using static STRINGS.INPUT_BINDINGS;

namespace WallPumps
{
    public class WallPumpsPatchs : KMod.UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            PUtil.InitLibrary();

            new PLocalization().Register();
            Pholib.Utilities.GenerateStringsTemplate(typeof(WALLPUMP_STRINGS));

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
            Pholib.Utilities.AddBuildingStrings(GasWallVentHighPressure.ID, WALLPUMP_STRINGS.BUILDINGS.PREFABS.FAIRGASWALLVENTHIGHTPRESSURE.NAME, 
                WALLPUMP_STRINGS.BUILDINGS.PREFABS.FAIRGASWALLVENTHIGHTPRESSURE.DESC, WALLPUMP_STRINGS.BUILDINGS.PREFABS.FAIRGASWALLVENTHIGHTPRESSURE.EFFECT);
            
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

    [HarmonyPatch(typeof(Db))]
    [HarmonyPatch("Initialize")]
    public static class Db_Initialize_Patch
    {
        public static void Postfix()
        {
            Debug.Log(" === WallPumps v. 2.7 Db_Initialize Postfix === ");
            // Add buildings



            // Add tech tree/building menu entries
            GasWallPump.AddToMenus();
            LiquidWallPump.AddToMenus();
            GasWallVent.AddToMenus();
            GasWallVentHighPressure.AddToMenus();
            LiquidWallVent.AddToMenus();
        }
    }


}
