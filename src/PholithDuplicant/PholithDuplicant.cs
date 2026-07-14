using Database;
using HarmonyLib;
using Klei.AI;
using KMod;
using Newtonsoft.Json;
using PeterHan.PLib.Options;
using Pholib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UtilLibs;
using static BionicUpgradeComponentConfig;

namespace PholithDuplicant
{
    public class PholithDuplicant : UserMod2
    {
        public const string degausser_coil_id = "Booster_Degausser_Coil";
        public const string hat_id = "hat_degausser_coil";

        public static PholithOptions Settings;

        public static string ModPath;

        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            ModPath = path;

            new POptions().RegisterOptions(this, typeof(PholithOptions));
            ModUtil.RegisterForTranslation(typeof(STRINGS));
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
            string personalitiesPath = Path.Combine(ModPath, "PERSONALITIES.json");
            if (!string.IsNullOrWhiteSpace(ModPath))
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
    [HarmonyPatch(typeof(Localization), nameof(Localization.Initialize))]
    public static class PholithDup_Localization_Initialize
    {
        public static void Postfix()
        {
            Utilities.LoadTranslations(typeof(STRINGS), PholithDuplicant.ModPath);
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

    // Patch the hat
    [HarmonyPatch(typeof(Db), "Initialize")]
    public class Db_Initialize_Patch
    {
        public static void Postfix(Db __instance)
        {
            WAccessories.Register(__instance.AccessorySlots, __instance.Accessories);
        }
    }
    // Copy from Sgt Weeb dup thanks!
    public class WAccessories
    {
        public static void Register(AccessorySlots slots, Accessories accessories)
        {
            try
            {
                var coil = Assets.GetAnim($"{PholithDuplicant.hat_id}_kanim");
                AddAccessories(coil, slots.Hat, accessories);
            }
            catch (Exception e)
            {
                Logs.Log(e);
            }
        }
        public static void AddAccessories(KAnimFile file, AccessorySlot slot, ResourceSet parent)
        {
            var build = file.GetData().build;
            var id = slot.Id.ToLower();
            for (var i = 0; i < build.symbols.Length; i++)
            {
                var symbolName = HashCache.Get().Get(build.symbols[i].hash);
                SgtLogger.l(symbolName);

                if (symbolName.StartsWith(id))
                {
                    var accessory = new Accessory(symbolName, parent, slot, file.batchTag, build.symbols[i]);
                    slot.accessories.Add(accessory);
                    HashCache.Get().Add(accessory.IdHash.HashValue, accessory.Id);

                    SgtLogger.l("Added accessory: " + accessory.Id);
                }
                else
                {
                    SgtLogger.l($"Symbol {symbolName} in file {file.name} is not starting with {id}");
                }
            }
        }
    }

    // Get the first char container to guarantee Pholith
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
    // Force Pholith personnality
    [HarmonyPatch(typeof(CharacterContainer), "GenerateCharacter")]
    public class CharacterContainer_GenerateCharacter_Patch
    {
        public static void Postfix(CharacterContainer __instance, ref MinionStartingStats ___stats)
        {
            if (PholithDuplicant.Settings != null && PholithDuplicant.Settings.GuaranteePholith && __instance == CharacterSelectionController_InitializeContainers_Patch.firstCharContainer)
            {
                ___stats = new MinionStartingStats(Db.Get().Personalities.GetPersonalityFromNameStringKey("PHOLITH"));

                Traverse.Create(__instance).Method("SetAnimator").GetValue();
                Traverse.Create(__instance).Method("SetInfoText").GetValue();

                __instance.StartCoroutine("SetAttributes");

            }
        }
    }

    // Load hat sprite for menu selection
    [HarmonyPatch(typeof(Assets), "OnPrefabInit")]
    public class Assets_OnPrefabInit_Patch
    {
        public static void Prefix(Assets __instance)
        {
            try
            {
                AddSpriteToAssets(__instance, PholithDuplicant.hat_id);
            }
            catch (Exception e)
            {
                Logs.Log(e);
            }
        }

        // Copy from Sgt UtilLibs.AssetsUtil (Thanks!) because I need the good ModPath
        public static Sprite AddSpriteToAssets(Assets instance, string spriteid, bool overrideExisting = false, TextureWrapMode mode = TextureWrapMode.Repeat)
        {
            string directory = Path.Combine(PholithDuplicant.ModPath, "assets");
            Texture2D texture2D = AssetUtils.LoadTexture(spriteid, directory);
            Logs.Log(texture2D);
            texture2D.wrapMode = mode;
            Sprite sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), Vector3.zero);
            Logs.Log(sprite);
            sprite.name = spriteid;
            if (!overrideExisting && instance.SpriteAssets.Any((Sprite spritef) => spritef != null && spritef.name == spriteid))
            {
                SgtLogger.l("Sprite " + spriteid + " was already existent in the sprite assets");
                return null;
            }
            if (overrideExisting)
            {
                instance.SpriteAssets.RemoveAll((Sprite foundsprite2) => foundsprite2 != null && foundsprite2.name == spriteid);
            }
            instance.SpriteAssets.Add(sprite);
            return sprite;
        }
    }

    // Create degausser coil booster
    [HarmonyPatch(typeof(BionicUpgradeComponentConfig), nameof(BionicUpgradeComponentConfig.CreatePrefabs))]
    public class BionicUpgradeComponentConfig_CreatePrefabs_Patch
    {

        public static void Postfix(BionicUpgradeComponentConfig __instance, List<GameObject> __result)
        {

            AttributeModifier[] modifiers = __instance.CreateBoosterModifiers(PholithDuplicant.degausser_coil_id, new Dictionary<string, float>
            {
                {
                    Db.Get().Attributes.Athletics.Id, 5f
                },
                {
                    Db.Get().Amounts.Stress.deltaAttribute.Id, - 1f / 60f // -1f/60 means -10%
                },
                {
                    Db.Get().Attributes.Caring.Id, -2f
                },
                {
                    Db.Get().Attributes.Learning.Id, -5f
                }
            });
            BionicUpgrade_SkilledWorker.Def skill_worker_def = new BionicUpgrade_SkilledWorker.Def(
                PholithDuplicant.degausser_coil_id, Db.Get().Attributes.Athletics.Id, modifiers, new SkillPerk[0], new string[1] { PholithDuplicant.hat_id });

            __result.Add(CreateNewUpgradeComponent(PholithDuplicant.degausser_coil_id, null, null,
                0f, (StateMachine.Instance smi) => new BionicUpgrade_SkilledWorker.Instance(smi.GetMaster(), skill_worker_def),
                skill_worker_def.GetDescription() + "\n\n" + string.Format(global::STRINGS.ITEMS.BIONIC_BOOSTERS.FABRICATION_SOURCE, global::STRINGS.BUILDINGS.PREFABS.ADVANCEDCRAFTINGTABLE.NAME),
                DlcManager.DLC3, "booster_degausser_coil_kanim", "mod_degausser_coil", SimHashes.Creature, null, BoosterType.Overclocked,
                isStartingBooster: false, isCarePackage: false, new SkillPerk[0], new ComplexRecipe.RecipeElement[2]
                {
                new ComplexRecipe.RecipeElement(Booster_Research1, 1f),
                new ComplexRecipe.RecipeElement(PowerStationToolsConfig.ID, 4f)
            }));

        }
    }
}

