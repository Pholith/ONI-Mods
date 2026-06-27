using HarmonyLib;
using KMod;
using Newtonsoft.Json;
using PeterHan.PLib.Options;
using Pholib;
using System.Collections.Generic;
using System.IO;

namespace PholithDuplicant
{
    public class PholithDuplicant : UserMod2
    {

        public static PholithOptions Settings;

        public static string modPath;

        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            modPath = path;

            new POptions().RegisterOptions(this, typeof(PholithOptions));

            // Init PLib and settings
            ///PUtil.InitLibrary();

            ReadSettings();
        }

        public static void ReadSettings()
        {
            Logs.Log("Loading settings");
            Settings = POptions.ReadSettings<PholithOptions>();
            if (Settings == null)
            {
                Settings = new PholithOptions();
            }

            // Read and write personalities
            string personalitiesPath = Path.Combine(modPath, "PERSONALITIES.json");
            if (!string.IsNullOrWhiteSpace(modPath))
            {
                try
                {
                    Dictionary<string, PersonalityOutline> readedPersonalities = default;
                    using (StreamReader streamReader = new StreamReader(personalitiesPath))
                    {
                        readedPersonalities = JsonConvert.DeserializeObject<Dictionary<string, PersonalityOutline>>(streamReader.ReadToEnd());
                        if (readedPersonalities == null || !readedPersonalities.ContainsKey("PHOLITH"))
                        {
                            Logs.Log($"Couldn't load Pholith dup in {personalitiesPath}");
                            return;
                        }
                        readedPersonalities["PHOLITH"].Name = Settings.UsePholithFirstName ? "Victoire" : "Pholith";
                        readedPersonalities["PHOLITH"].Hair = !Settings.AlternativePurpleHair ? "hair_pholith" : "hair_pholith_purple";
                    }

                    using (StreamWriter streamWriter = new StreamWriter(personalitiesPath))
                    {
                        streamWriter.Write(JsonConvert.SerializeObject(readedPersonalities));
                    }
                }
                catch (System.Exception e)
                {
                    Logs.Log(e.ToString());
                }
            }

        }
    }

    // Read settings on launching a game (mostly for GuaranteePholith option)
    [HarmonyPatch(typeof(Game), "Load")]
    public static class GameOnLoadPatch
    {
        public static void Prefix()
        {
            PholithDuplicant.ReadSettings();
        }
    }

    /*
 [HarmonyPatch("PersonalityManager", "ReadPersonalities")]
 public class Dupery_ReadPersonalities_Patch
 {
     public static void Postfix()
     {

         Logs.Log("test");
         //Dictionary<string, dynamic> test = __result;

     }
 }*/

    [HarmonyPatch(typeof(CharacterSelectionController), "InitializeContainers")]
    public class CharacterSelectionController_InitializeContainers_Patch
    {

        public static CharacterContainer firstCharContainer;
        public static void Postfix(MinionSelectScreen __instance, List<ITelepadDeliverableContainer> ___containers)
        {
            foreach (ITelepadDeliverableContainer container in ___containers)
            {
                if (container is CharacterContainer charContainer)
                {
                    firstCharContainer = charContainer;
                    break;
                }
            }
        }
    }
    [HarmonyPatch(typeof(CharacterContainer), "GenerateCharacter")]
    public class CharacterContainer_GenerateCharacter_Patch
    {
        public static void Postfix(CharacterContainer __instance, ref MinionStartingStats ___stats)
        {
            if (PholithDuplicant.Settings != null && PholithDuplicant.Settings.GuaranteePholith && __instance == CharacterSelectionController_InitializeContainers_Patch.firstCharContainer)
            {
                ___stats = new MinionStartingStats(Db.Get().Personalities.GetPersonalityFromNameStringKey("PHOLITH"));

                // Logs.Log(Traverse.Create(__instance).Method("IsCharacterInvalid").GetValue());

                Traverse.Create(__instance).Method("SetAnimator").GetValue();
                Traverse.Create(__instance).Method("SetInfoText").GetValue();

                __instance.StartCoroutine("SetAttributes");

            }
        }
    }
}
