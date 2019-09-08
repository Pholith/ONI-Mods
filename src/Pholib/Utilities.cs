using Harmony;
using System;
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
            Logs.Log(CustomGameSettings.Instance.name);
            return CustomGameSettings.Instance.name == worldName;
        }

        /// <summary>
        /// Add strings and icon for a world
        /// </summary>
        /// <param name="NAME"> Name of the world </param>
        /// <param name="DESCRIPTION"> Description of the world </param>
        /// <param name="iconName"> DDS icon name (incorporated ressources only) </param>
        /// <param name="className"> Class containing the locstrings </param>
        public static void addWorldYaml(string NAME, string DESCRIPTION, string iconName, Type className)
        {
            // Add strings used in Fuleria.yaml
            Strings.Add($"STRINGS.WORLDS." + NAME.ToUpper() + ".NAME", NAME);
            Strings.Add($"STRINGS.WORLDS." + NAME.ToUpper() + ".DESCRIPTION", DESCRIPTION);

            Logs.LogIfDebugging("Strings added at: " + "STRINGS.WORLDS." + NAME.ToUpper() + ".NAME");
            
            // Generate a translation .pot 
            ModUtil.RegisterForTranslation(className);

            if (!iconName.IsNullOrWhiteSpace())
            {
                //Load the sprite from Asteroid_Fuleria.dds (converted online from png) and set "generation action" to incorporated ressources
                try
                {
                    Sprite sprite = Sprites.CreateSpriteDXT5(Assembly.GetExecutingAssembly().GetManifestResourceStream(className.AssemblyQualifiedName + "." + iconName + ".dds"), 512, 512);
                    Assets.Sprites.Add(iconName, sprite);

                }
                catch (Exception e)
                {
                    Logs.Log(e.ToString());
                    throw new ArgumentException();
                }
            }
        }
    }
}
