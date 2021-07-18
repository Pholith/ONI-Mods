﻿using HarmonyLib;
using STRINGS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Pholib
{
    public class Logs
    {
        private static readonly string version = "1.2.3";

        public static bool DebugLog = false;
        private static bool initiated = false;

        public static void InitIfNot()
        {
            if (initiated)
            {
                return;
            }
            Debug.Log("== Game Launched with Pholib " + version + "  " + System.DateTime.Now);
            initiated = true;
        }


        public static void Log(string informations)
        {
            InitIfNot();
            Debug.Log("Pholib: " + informations);
        }
        public static void Log(object informations)
        {
            InitIfNot();
            Debug.Log("Pholib: " + informations == null ? "null" : informations.ToString());
        }


        public static void LogIfDebugging(string informations)
        {
            InitIfNot();
            if (DebugLog)
            {
                Debug.Log("Pholib: " + informations);
            }
        }
    }

    public static class Extensions
    {
        public static void RemoveDef<DefType>(this GameObject go) where DefType : StateMachine.BaseDef
        {
            StateMachineController stateMachineController = go.AddOrGet<StateMachineController>();
            DefType defType = stateMachineController.GetDef<DefType>();
            if (defType != null)
            {
                defType.Configure(null);
                stateMachineController.cmpdef.defs.Remove(defType);

                //defType = Activator.CreateInstance<DefType>();
                //stateMachineController.AddDef(defType);
                //defType.Configure(stateMachineController.gameObject);
            }
        }
    }

    public class Utilities
    {
        /// <summary>
        /// Return true if the surface is discovered. Should not be called until game loaded.
        /// </summary>
        /// <returns></returns>
        public static bool IsSurfaceDiscovered()
        {
            return Game.Instance.savedInfo.discoveredSurface;
        }

        /// <summary>
        /// Return true if a oilfield is discovered. Should not be called until game loaded.
        /// </summary>
        /// <returns></returns>
        public static bool IsOilFieldDiscovered()
        {
            return Game.Instance.savedInfo.discoveredOilField;
        }

        /// <summary>
        /// Return true if the tag is discovered (exemple: IsTagDiscovered("EthanolOilfloater") return true if the player discovered a ethanoloilfloater). 
        /// Should not be called until game loaded.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static bool IsTagDiscovered(string tag)
        {
            return DiscoveredResources.Instance.IsDiscovered(tag);
        }

        /// <summary>
        /// Return true if a SimHashes is discovered. Should not be called until game loaded.
        /// </summary>
        /// <param name="hash"> SimHash of the element </param>
        /// <returns></returns>
        public static bool IsSimHashesDiscovered(SimHashes hash)
        {
            return DiscoveredResources.Instance.IsDiscovered(ElementLoader.FindElementByHash(hash).tag);
        }

        /// <summary>
        /// Return true if the current cycle greater than the condition. Should not be called until game loaded.
        /// </summary>
        /// <param name="cycle"> minimum condition cycle </param>
        /// <returns></returns>
        public static bool CycleCondition(int cycle)
        {
            return GameClock.Instance.GetCycle() >= cycle;
        }

        /// <summary>
        /// Return True if the current cycle is in range. Should not be called until game loaded.
        /// </summary>
        /// <param name="minimum"> minimum cycle </param>
        /// <param name="maximum"> maximum cycle </param>
        /// <returns></returns>
        public static bool CycleInRange(int minimum, int maximum)
        {
            return GameClock.Instance.GetCycle() >= minimum && GameClock.Instance.GetCycle() <= maximum;
        }

        /// <summary>
        /// Return true if the game is loaded a this world. Should not be called until game loaded.
        /// </summary>
        /// <param name="worldName"> Worldname </param>
        /// <returns></returns>
        public static bool IsOnWorld(string worldName)
        {
            if (CustomGameSettings.Instance == null)
            {
                return false;
            }

            Dictionary<string, string> dict = Traverse.Create(CustomGameSettings.Instance).Field<Dictionary<string, string>>("CurrentQualityLevelsBySetting").Value;
            if (dict == null || dict["World"] == null)
            {
                return false;
            }

            //Logs.LogIfDebugging(dict["World"]);
            //Logs.LogIfDebugging(dict["World"].Replace("worlds/", ""));

            return dict["World"].Replace("worlds/", "") == worldName;
        }

        /// <summary>
        /// Load a .po file and override translations
        /// Should be called at Localization.Initialize (Postfix)
        /// </summary>
        /// <param name="locStringRoot">  locStringRoot is typeof(YourStringsClass) </param>
        /// <param name="modPath"> modPath is obtained via OnLoad(string) </param>
        /// <param name="translationsDir"> translationsDir is the directory with your translations. 
        /// Ideally, it should be named something other than "strings" 
        /// because that's where the game will try to look for translations by default and load them on its own
        ///</param>
        /// Thanks exnihilo to helping making translations
        public static void LoadTranslations(Type locStringRoot, string modPath, string translationsDir = "translations")
        {
            // you still need to call this
            Localization.RegisterForTranslation(locStringRoot);

            Localization.Locale locale = Localization.GetLocale();
            if (locale == null)
            {
                // english language is selected, so no action is needed
                return;
            }

            if (string.IsNullOrEmpty(modPath))
            {
                Debug.LogError("modPath is empty");
                return;
            }

            string stringsPath = Path.Combine(modPath, translationsDir ?? "");
            string translationsPath = Path.Combine(stringsPath, locale.Code + ".po");

            Debug.Log($"Loading translation file for {locale.Lang} ({locale.Code}) language: '{translationsPath}'");

            if (!File.Exists(translationsPath))
            {
                Debug.LogWarning($"Translation file not found: '{translationsPath}'");
                return;
            }

            try
            {
                Dictionary<string, string> translations = Localization.LoadStringsFile(translationsPath, false);
                Localization.OverloadStrings(translations);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Unexpected error while loading translation file: '{translationsPath}'");
                Debug.LogError(ex);
            }
        }

        public static void AddCarePackage(ref Immigration immigration, string objectId, float amount, Func<bool> requirement = null)
        {
            Traverse field = Traverse.Create(immigration).Field("carePackages");
            List<CarePackageInfo> list = field.GetValue<CarePackageInfo[]>().ToList();
            list.Add(new CarePackageInfo(objectId, amount, requirement));
            field.SetValue(list.ToArray());
        }


        private static readonly List<Type> alreadyLoaded = new List<Type>();
        /// <summary>
        /// Add strings and icon for a world
        /// Don't call this method OnLoad ! 
        /// Should be called at Db.Initialize
        /// </summary>
        /// <param name="className"> Class containing the locstrings </param>
        public static void AddWorldYaml(Type className)
        {

            // Generate a translation .pot and prepare translation strings for loading
            if (!alreadyLoaded.Contains(className))
            {
                ModUtil.RegisterForTranslation(className);
                alreadyLoaded.Add(className);
            }
        }


        public static void AddBuilding(string category, string id, string name, string desc, string effect)
        {
            string upperCaseID = id.ToUpperInvariant();
            Strings.Add(new string[]
                {
                "STRINGS.BUILDINGS.PREFABS." + upperCaseID + ".NAME",
                UI.FormatAsLink(name, id)
            });
            Strings.Add(new string[]
            {
                "STRINGS.BUILDINGS.PREFABS." + upperCaseID + ".DESC",
                   desc
            });
            Strings.Add(new string[]
            {
                "STRINGS.BUILDINGS.PREFABS." + upperCaseID + ".EFFECT",
                   effect
            });
            ModUtil.AddBuildingToPlanScreen(category, id);
        }

        /// <summary>
        /// Add building tech. Must be called using a postfix on Db.Init
        /// </summary>
        /// <param name="techId"></param>
        /// <param name="buildingId"></param>
        public static void AddBuildingTech(string techId, string buildingId)
        {
            Db.Get().Techs.Get(techId).unlockedItemIDs.Add(buildingId);
        }

        public static ComplexRecipe AddComplexRecipe(ComplexRecipe.RecipeElement[] input, ComplexRecipe.RecipeElement[] output,
            string fabricatorId, float productionTime, LocString recipeDescription, ComplexRecipe.RecipeNameDisplay nameDisplayType, int sortOrder, string requiredTech = null)
        {
            string recipeId = ComplexRecipeManager.MakeRecipeID(fabricatorId, input, output);

            return new ComplexRecipe(recipeId, input, output)
            {
                time = productionTime,
                description = recipeDescription,
                nameDisplay = nameDisplayType,
                fabricators = new List<Tag> { fabricatorId },
                sortOrder = sortOrder,
                requiredTech = requiredTech
            };
        }




        //ElementsUtils from Heinermann

        public static void AddSubstance(Substance substance)
        {
            Assets.instance.substanceTable.GetList().Add(substance);
        }

        public static Substance CreateSubstance(string name, Element.State state, KAnimFile kanim, Material material, Color32 colour)
        {
            return ModUtil.CreateSubstance(name, state, kanim, material, colour, colour, colour);
        }

        public static void RegisterElementStrings(string elementId, string name, string description)
        {
            string upperElemId = elementId.ToUpper();
            Strings.Add($"STRINGS.ELEMENTS.{upperElemId}.NAME", STRINGS.UI.FormatAsLink(name, upperElemId));
            Strings.Add($"STRINGS.ELEMENTS.{upperElemId}.DESC", description);
        }

        // Note: As of 2021-03-14
        // Needed for vanilla as it does not map anims into a dictionary until after elements have been loaded
        // Therefore don't use Assets.GetAnim here.
        public static KAnimFile FindAnim(string name)
        {
            KAnimFile result = Assets.Anims.Find((anim) => anim.name == name);
            if (result == null)
                Debug.LogError($"Failed to find KAnim: {name}");
            return result;
        }
        public static Substance CreateRegisteredSubstance(string name, Element.State state, KAnimFile kanim, Material material, Color32 colour)
        {
            Substance result = CreateSubstance(name, state, kanim, material, colour);
            SimHashUtil.RegisterSimHash(name);
            AddSubstance(result);
            ElementLoader.FindElementByHash(result.elementID).substance = result;
            return result;
        }


        public static class SimHashUtil
        {
            public static Dictionary<SimHashes, string> SimHashNameLookup = new Dictionary<SimHashes, string>();
            public static readonly Dictionary<string, object> ReverseSimHashNameLookup = new Dictionary<string, object>();

            public static void RegisterSimHash(string name)
            {
                SimHashes simHash = (SimHashes)Hash.SDBMLower(name);
                SimHashNameLookup.Add(simHash, name);
                ReverseSimHashNameLookup.Add(name, simHash);
            }
        }

        [HarmonyPatch(typeof(Enum), "ToString", new Type[] { })]
        public class SimHashes_ToString
        {
            public static bool Prefix(ref Enum __instance, ref string __result)
            {
                if (!(__instance is SimHashes)) return true;
                return !SimHashUtil.SimHashNameLookup.TryGetValue((SimHashes)__instance, out __result);
            }
        }

        [HarmonyPatch(typeof(Enum), nameof(Enum.Parse), new Type[] { typeof(Type), typeof(string), typeof(bool) })]
        public class SimHashes_Parse
        {
            public static bool Prefix(Type enumType, string value, ref object __result)
            {
                if (!enumType.Equals(typeof(SimHashes))) return true;
                return !SimHashUtil.ReverseSimHashNameLookup.TryGetValue(value, out __result);
            }
        }

    }

}
