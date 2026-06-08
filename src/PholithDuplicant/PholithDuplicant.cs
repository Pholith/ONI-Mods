using HarmonyLib;
using KMod;
using PeterHan.PLib.Options;
using Pholib;
using System.Collections.Generic;

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

       
    [HarmonyPatch("PersonalityManager", "ReadPersonalities")]
    public class Dupery_ReadPersonalities_Patch
    {
        public static void Postfix()
        {

            Logs.Log("test");
            //Dictionary<string, dynamic> test = __result;

        }
    }

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
            if (PholithDuplicant.Settings.GuaranteePholith && __instance == CharacterSelectionController_InitializeContainers_Patch.firstCharContainer)
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
