using Database;
using Harmony;
using STRINGS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Pholib
{
    public class Utilities
    {

        public static bool IsSurfaceDiscovered()
        {
            return Traverse.Create<Game.SavedInfo>().Field("discoveredSurface").GetValue<bool>();
        }

        public static bool CycleCondition(int cycle)
        {
            return GameClock.Instance.GetCycle() >= cycle;
        }

        public static bool IsOnWorld(string worldName)
        {
            if (CustomGameSettings.Instance == null)
            {
                return false;
            }

            Dictionary<string, string> dict = Traverse.Create(CustomGameSettings.Instance).Field<Dictionary<string, string>>("CurrentQualityLevelsBySetting").Value;
            Logs.Log(worldName);
            Logs.Log(dict["World"]);
            if (dict == null || dict["World"] == null) return false;

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

            var locale = Localization.GetLocale();
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

            var stringsPath = Path.Combine(modPath, translationsDir ?? "");
            var translationsPath = Path.Combine(stringsPath, locale.Code + ".po");

            Debug.Log($"Loading translation file for {locale.Lang} ({locale.Code}) language: '{translationsPath}'");

            if (!File.Exists(translationsPath))
            {
                Debug.LogWarning($"Translation file not found: '{translationsPath}'");
                return;
            }

            try
            {
                var translations = Localization.LoadStringsFile(translationsPath, false);
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
            var field = Traverse.Create(immigration).Field("carePackages");
            var list = field.GetValue<CarePackageInfo[]>().ToList();
            list.Add(new CarePackageInfo(objectId, amount, requirement));
            field.SetValue(list.ToArray());
        }


        private static List<Type> alreadyLoaded = new List<Type>();
        /// <summary>
        /// Add strings and icon for a world
        /// Don't call this method OnLoad ! 
        /// Should be called at Db.Initialize
        /// </summary>
        /// <param name="NAME"> Name of the world </param>
        /// <param name="DESCRIPTION"> Description of the world </param>
        /// <param name="iconName"> DDS icon name (incorporated ressources only) </param>
        /// <param name="className"> Class containing the locstrings </param>
        public static void AddWorldYaml(string NAME, string DESCRIPTION, string iconName, Type className)
        {

            // DEPRECATED - Add strings used in ****.yaml 
            //Strings.Add($"STRINGS.WORLDS." + NAME.ToUpper() + ".NAME", NAME);
            //Strings.Add($"STRINGS.WORLDS." + NAME.ToUpper() + ".DESCRIPTION", DESCRIPTION);


            // Generate a translation .pot and prepare translation strings for loading
            if (!alreadyLoaded.Contains(className))
            {
                ModUtil.RegisterForTranslation(className);
                alreadyLoaded.Add(className);
            }


            if (!iconName.IsNullOrWhiteSpace())
            {
                //Load the sprite from Asteroid_****.dds (converted online from png) and set "generation action" to incorporated ressources
                Logs.LogIfDebugging("Loading Sprite: " + className.Assembly.GetName().Name + "." + iconName + ".dds");
                Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(className.Assembly.GetName().Name + "." + iconName + ".dds");
                if (stream == null)
                {
                    throw new ArgumentException("Sprite name is not valid.");
                }
                Sprite sprite = CreateSpriteDXT5(stream, 512, 512);
                Assets.Sprites.Add(iconName, sprite);
            }
        }

        // Load a incorporated sprite (old loading, the image is reversed)
        public static Sprite CreateSpriteDXT5Inversed(Stream inputStream, int width, int height)
        {
            byte[] array = new byte[inputStream.Length - 128L];
            inputStream.Seek(128L, SeekOrigin.Current);
            inputStream.Read(array, 0, array.Length);
            Texture2D texture2D = new Texture2D(width, height, TextureFormat.DXT5, false);
            texture2D.LoadRawTextureData(array);
            texture2D.Apply(false, true);
            return Sprite.Create(texture2D, new Rect(0f, 0f, (float)width, (float)height), new Vector2((float)(width / 2), (float)(height / 2)));
        }

        // Load a incorporated sprite v2 - thanks test447
        public static Sprite CreateSpriteDXT5(Stream inputStream, int width, int height)
        {
            byte[] array = new byte[inputStream.Length - 128L];
            inputStream.Seek(128L, SeekOrigin.Current);
            inputStream.Read(array, 0, array.Length);
            Texture2D texture2D = new Texture2D(width, height, TextureFormat.DXT5, false);
            texture2D.LoadRawTextureData(array);
            texture2D.Apply(false, false);
            // this isn't an efficient way to flip the loaded texture but it only runs once so the performance impact can't be that terrible
            Texture2D texture2DFlipped = new Texture2D(width, height, TextureFormat.RGBA32, false);
            for (int i = 0; i < texture2D.width; i++)
            {
                for (int j = 0; j < texture2D.height; j++)
                {
                    texture2DFlipped.SetPixel(i, j, texture2D.GetPixel(i, height - j - 1));
                }
            }
            texture2DFlipped.Apply(false, true);
            Sprite sprite = Sprite.Create(texture2DFlipped, new Rect(0f, 0f, (float)width, (float)height), new Vector2((float)(width / 2), (float)(height / 2)));
            return sprite;
        }

        public static Texture2D CreateTextureDXT5(Stream inputStream, int width, int height)
        {
            byte[] array = new byte[inputStream.Length - 128L];
            inputStream.Seek(128L, SeekOrigin.Current);
            inputStream.Read(array, 0, array.Length);
            Texture2D texture = new Texture2D(width, height, TextureFormat.DXT5, false);
            texture.LoadRawTextureData(array);
            texture.Apply(false, true);
            Debug.Assert(texture != null);
            return texture;
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

        public static void AddBuildingTech(string techName, string id)
        {
            List<string> list = new List<string>(Techs.TECH_GROUPING[techName]);
            list.Add(id);
            Techs.TECH_GROUPING[techName] = list.ToArray();
        }
    }



    public class Logs
    {
        private static readonly string version = "1.2.0";

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
            Debug.Log("Pholib: " + informations.ToString());
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

}
