using HarmonyLib;
using KMod;
using PeterHan.PLib.Options;
using Pholib;

namespace PholithDuplicant
{
    public class PholithDuplicant : UserMod2
    {

        public static PholithOptions Settings;

        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            new POptions().RegisterOptions(this, typeof(PholithOptions));

            // Init PLib and settings
            ///PUtil.InitLibrary();

            Settings = POptions.ReadSettings<PholithOptions>();
            if (Settings == null)
            {
                Settings = new PholithOptions();
            }
        }

        public static void ReadSettings()
        {
            Logs.Log("Loading settings");
            Settings = POptions.ReadSettings<PholithOptions>();
            if (Settings == null)
            {
                Settings = new PholithOptions();
            }
        }
    }
    /*
    [HarmonyPatch("PersonalityManager", "ReadPersonalities")]
    public class Dupery_ReadPersonalities_Patch
    {
        public static void Postfix(Dictionary<string, PersonalityOutline> __result)
        {

        }
    }*/
}
