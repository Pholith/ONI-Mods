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
            if (dict == null || dict["World"] == null) return false;

            //Logs.LogIfDebugging(dict["World"]);
            //Logs.LogIfDebugging(dict["World"].Replace("worlds/", ""));

            return dict["World"].Replace("worlds/", "") == worldName;
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
        /// To call at Db.Initialize
        /// </summary>
        /// <param name="NAME"> Name of the world </param>
        /// <param name="DESCRIPTION"> Description of the world </param>
        /// <param name="iconName"> DDS icon name (incorporated ressources only) </param>
        /// <param name="className"> Class containing the locstrings </param>
        public static void AddWorldYaml(string NAME, string DESCRIPTION, string iconName, Type className)
        {
            // Add strings used in ****.yaml
            Strings.Add($"STRINGS.WORLDS." + NAME.ToUpper() + ".NAME", NAME);
            Strings.Add($"STRINGS.WORLDS." + NAME.ToUpper() + ".DESCRIPTION", DESCRIPTION);

            Logs.LogIfDebugging("Strings added at: " + "STRINGS.WORLDS." + NAME.ToUpper() + ".NAME");

            // Generate a translation .pot
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

        // Load a incorporated sprite
        public static Sprite CreateSpriteDXT5(Stream inputStream, int width, int height)
        {
            byte[] array = new byte[inputStream.Length - 128L];
            inputStream.Seek(128L, SeekOrigin.Current);
            inputStream.Read(array, 0, array.Length);
            Texture2D texture2D = new Texture2D(width, height, TextureFormat.DXT5, false);
            texture2D.LoadRawTextureData(array);
            texture2D.Apply(false, true);
            return Sprite.Create(texture2D, new Rect(0f, 0f, (float)width, (float)height), new Vector2((float)(width / 2), (float)(height / 2)));
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
