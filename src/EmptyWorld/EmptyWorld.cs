using Harmony;
using System.Reflection;
using UnityEngine;

namespace EmptyWorld
{
    [HarmonyPatch(typeof(Db), "Initialize")]
    class EmpteraPatch
    {

        public static LocString NAME = "Emptera";
        public static LocString DESCRIPTION = "Emptera is a difficult empty asteroid. Resources are limited and you will have to take advantage of your geysers to get them.\n\n<smallcaps> Colonizing Emptera will be a difficult experience. However, this asteroid has some advantages with its vacuum. </smallcaps>\n\n";

        public static void Prefix()
        {
            // Add strings used in Fuleria.yaml
            Strings.Add($"STRINGS.WORLDS.EMPTERA.NAME", NAME);
            Strings.Add($"STRINGS.WORLDS.EMPTERA.DESCRIPTION", DESCRIPTION);

            // Generate a translation .pot 
            ModUtil.RegisterForTranslation(typeof(EmpteraPatch));

            //Load the sprite from Asteroid_Fuleria.dds (converted online from png) and set "generation action" to incorporated ressources
            Sprite empteraSprite = Utilities.Sprites.CreateSpriteDXT5(Assembly.GetExecutingAssembly().GetManifestResourceStream("EmptyWorld.Asteroid_Emptera.dds"), 512, 512);
            Assets.Sprites.Add("Asteroid_Emptera", empteraSprite);

        }
    }
    [HarmonyPatch(typeof(Db), "Initialize")]
    class IslandsPatch
    {

        public static LocString NAME = "Islands";
        public static LocString DESCRIPTION = "Islands is an asteroid composed of asteroids. The resources are scattered and you will have to go through space to colonize the place.\n\n<smallcaps> Colonizing Islands will be an original and fun experience. This will also allow you to easily access the vacuum for some setups</smallcaps>\n\n";

        public static void Prefix()
        {
            // Add strings used in Fuleria.yaml
            Strings.Add($"STRINGS.WORLDS.ISLANDS.NAME", NAME);
            Strings.Add($"STRINGS.WORLDS.ISLANDS.DESCRIPTION", DESCRIPTION);

            // Generate a translation .pot 
            ModUtil.RegisterForTranslation(typeof(IslandsPatch));

            //Load the sprite from Asteroid_Fuleria.dds (converted online from png) and set "generation action" to incorporated ressources
            Sprite sprite = Utilities.Sprites.CreateSpriteDXT5(Assembly.GetExecutingAssembly().GetManifestResourceStream("EmptyWorld.Asteroid_Islands.dds"), 512, 512);
            Assets.Sprites.Add("Asteroid_Islands", sprite);

        }
    }
}
