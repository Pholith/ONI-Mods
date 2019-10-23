using Harmony;
using Klei.CustomSettings;
using System;
using System.Collections.Generic;
using System.IO;
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
        public static bool IsOnWorld(string worldName)
        {
            if (CustomGameSettings.Instance == null)
            {
                return false;
            }
            Dictionary<string, string> dict = Traverse.Create(CustomGameSettings.Instance).Field<Dictionary<string, string>>("CurrentQualityLevelsBySetting").Value;
            if (dict == null || dict["World"] == null) return false;

            Logs.LogIfDebugging(dict["World"]);
            Logs.LogIfDebugging(dict["World"].Replace("worlds/", ""));

            return dict["World"].Replace("worlds/", "") == worldName;
        }

        private static List<Type> alreadyLoaded = new List<Type>();
        /// <summary>
        /// Add strings and icon for a world
        /// Don't call this method OnLoad ! 
        /// </summary>
        /// <param name="NAME"> Name of the world </param>
        /// <param name="DESCRIPTION"> Description of the world </param>
        /// <param name="iconName"> DDS icon name (incorporated ressources only) </param>
        /// <param name="className"> Class containing the locstrings </param>
        public static void addWorldYaml(string NAME, string DESCRIPTION, string iconName, Type className)
        {
            // Add strings used in ****.yaml
            Strings.Add($"STRINGS.WORLDS." + NAME.ToUpper() + ".NAME", NAME);
            Strings.Add($"STRINGS.WORLDS." + NAME.ToUpper() + ".DESCRIPTION", DESCRIPTION);

            Logs.LogIfDebugging("Strings added at: " + "STRINGS.WORLDS." + NAME.ToUpper() + ".NAME");

            // Generate a translation .pot
            if (!alreadyLoaded.Contains(className))
            {
                ModUtil.RegisterForTranslation(className);
            }
            alreadyLoaded.Add(className);

            if (!iconName.IsNullOrWhiteSpace())
            {
                //Load the sprite from Asteroid_****.dds (converted online from png) and set "generation action" to incorporated ressources
                try
                {
                    Logs.LogIfDebugging("Loading Sprite: " + className.Assembly.GetName().Name + "." + iconName + ".dds");
                    Sprite sprite = CreateSpriteDXT5(Assembly.GetExecutingAssembly().GetManifestResourceStream(className.Assembly.GetName().Name + "." + iconName + ".dds"), 512, 512);
                    Logs.LogIfDebugging((Assets.Sprites == null).ToString());
                    Assets.Sprites.Add(iconName, sprite);

                }
                catch (Exception e)
                {
                    Logs.Log("SpriteException: " + e.ToString());
                    throw new ArgumentException();
                }
            }
        }

        // Thanks Mayall for his search
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
    }
}
