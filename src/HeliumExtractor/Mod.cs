using HarmonyLib;
using KMod;
using PeterHan.PLib.Database;
using PeterHan.PLib.Options;

namespace HeliumExtractor
{
    public class Mod : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            new POptions().RegisterOptions(this, typeof(HeliumOptions));
            GameOnLoadPatch.ReadSettings();

            new PLocalization().Register();
            Pholib.Utilities.GenerateStringsTemplate(typeof(PHO_STRINGS));

        }
    }

    // Load PLib settings on game load
    [HarmonyPatch(typeof(Game), "Load")]
    public static class GameOnLoadPatch
    {
        public static HeliumOptions Settings { get; private set; }

        public static void Prefix()
        {
            ReadSettings();
        }
        public static void ReadSettings()
        {
            // read the option each time the game is loaded - so we don't need to restart all the game
            Settings = POptions.ReadSettings<HeliumOptions>();
            if (Settings == null)
            {
                Settings = new HeliumOptions();
            }
            if (Settings.PowerConsumption < 0) Settings.PowerConsumption = 0;

        }
    }

}
