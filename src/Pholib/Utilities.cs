using HarmonyLib;
using Newtonsoft.Json;
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
        private static readonly string version = "1.7";

        public static bool DebugLog = false;
        private static bool initiated = false;
        private static string modName = "";
        public static void InitIfNot()
        {
            if (initiated)
            {
                return;
            }
            modName = Assembly.GetExecutingAssembly().GetName().Name;
            Debug.Log($"== Game Launched with Pholib {version} [{modName}] {System.DateTime.Now}");
            
            initiated = true;
        }
        public static void Error(string informations)
        {
            InitIfNot();
            Debug.Log($"Pholib: [{modName}][ERROR] " + informations);
        }
        public static void Log(string informations)
        {
            InitIfNot();
            Debug.Log($"Pholib: [{modName}] " + informations);
        }
        public static void Log(object informations)
        {
            InitIfNot();
            Debug.Log($"Pholib: [{modName}] " + (informations == null ? "null" : informations.ToString()));
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
        /// <summary>
        /// Convert an object in a yaml readable string.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Dump(this object obj)
        {
            var jsonOptions = new JsonSerializerSettings();
            jsonOptions.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            return JsonConvert.SerializeObject(obj, jsonOptions);

        }

        public static string ListToString<T>(this List<T> list) where T : class
        {
            string result = "{";
            for (int i = 0; i < list.Count; i++)
            {
                result += list[i];
                if (i < list.Count - 1) result += ",";
            }
            result += "}";
            return result;
        }

    }

    public static class Utilities
    {
        public static float CelciusToKelvin(this float celcius)
        {
            return celcius + 273.15f;
        }

        public static string FormatColored(this string text, Color color, bool bold = true)
        {
            return FormatColored(text, color.ToHexString(), bold);
        }
        public static string FormatColored(this string text, string HexaColor, bool bold = true)
        {
            return $"{(bold ? "<b>" : "")}<color=#{HexaColor}>{text}</color>{(bold ? "</b>" : "")}";
        }

        public static string ModPath()
        {
            return Directory.GetParent(Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar.ToString();
        }

        /// <summary>
        /// Create a subcategory for plan skins.
        /// </summary>
        /// Inspired from SGT Closet utilityLib (Thanks)
        public static void AddSkinSubcategory(string mainCategory, string subcategoryID, Sprite icon, int sortkey, string[] permitIDs)
        {
            if (!InventoryOrganization.categoryIdToSubcategoryIdsMap.ContainsKey(mainCategory))
            {
                Logs.Log($"Category {mainCategory} not found");
                return;
            }
            InventoryOrganization.categoryIdToSubcategoryIdsMap[mainCategory].Add(subcategoryID);
            
            if (InventoryOrganization.subcategoryIdToPermitIdsMap.ContainsKey(subcategoryID))
            {
                return;
            }
            InventoryOrganization.subcategoryIdToPermitIdsMap.Add(subcategoryID, new HashSet<string>());
            for (int i = 0; i < permitIDs.Length; i++)
            {
                InventoryOrganization.subcategoryIdToPermitIdsMap[subcategoryID].Add(permitIDs[i]);
            }
            InventoryOrganization.subcategoryIdToPresentationDataMap.Add(subcategoryID, new InventoryOrganization.SubcategoryPresentationData(subcategoryID, icon, sortkey));
        }

        /// <summary>
        /// Generate a .pot file for user to be able to translate the mod.
        /// This is already called in ModUtil.RegisterForTranslation(), but not in PLib.
        /// </summary>
        public static void GenerateStringsTemplate(Type stringClass)
        {
            Localization.GenerateStringsTemplate(stringClass, Path.Combine(KMod.Manager.GetDirectory(), "strings_templates"));
        }

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
        /// <param name="clusterName"> Cluster name </param>
        /// <returns></returns>
        public static bool IsOnCluster(string clusterName)
        {
            if (CustomGameSettings.Instance == null)
            {
                return false;
            }
            Dictionary<string, string> dict = Traverse.Create(CustomGameSettings.Instance).Field<Dictionary<string, string>>("CurrentQualityLevelsBySetting").Value;


            if (dict == null || dict["ClusterLayout"] == null)
            {
                return false;
            }

            //Logs.LogIfDebugging(dict["World"]);
            //Logs.LogIfDebugging(dict["World"].Replace("worlds/", ""));

            return dict["ClusterLayout"].Replace("clusters/", "") == clusterName;
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
            List<CarePackageInfo> list = field.GetValue<List<CarePackageInfo>>().ToList();
            list.Add(new CarePackageInfo(objectId, amount, requirement));
            field.SetValue(list);
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


        public static void AddBuildingStrings(string id, string name, string desc, string effect)
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

        }
        /// <summary>
        /// Add building strings and add building to Plan screen.
        /// </summary>
        public static void AddBuilding(string category, string id, string name, string desc, string effect, bool addBuildintToPlanScreen = true)
        {
            AddBuildingStrings(id, name, desc, effect);
            if (addBuildintToPlanScreen) ModUtil.AddBuildingToPlanScreen(category, id);
        }

        /// <summary>
        /// Add building tech. Must be called using a postfix on Db.Init.
        /// Tech id can be found in Database.Techs class.
        /// </summary>
        /// <param name="techId"></param>
        /// <param name="buildingId"></param>
        public static void AddBuildingTech(string techId, string buildingId)
        {
            if (Db.Get().Techs.TryGet(techId) == null)
                Logs.Error($"Could not find tech: {techId}");
            else
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

    }

}
