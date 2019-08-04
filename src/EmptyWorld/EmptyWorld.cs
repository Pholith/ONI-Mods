using Harmony;
using System.Reflection;
using UnityEngine;

namespace EmptyWorld
{
    [HarmonyPatch(typeof(Db), "Initialize")]
    class EmptyWorldPatch
    {

        public static LocString NAME = "Emptera";
        public static LocString DESCRIPTION = "Emptera is a very difficult world made up essentially of emptiness. Resources are limited and you will have to take advantage of your geysers to get them.\n\n<smallcaps> Colonizing Emptera will be one of the most difficult experiences you've ever had. However, this asteroid has some advantages with its vacuum. </smallcaps>\n\n";

        public static void Prefix()
        {
            // Add strings used in Fuleria.yaml
            Strings.Add($"STRINGS.WORLDS.EMPTERA.NAME", NAME);
            Strings.Add($"STRINGS.WORLDS.EMPTERA.DESCRIPTION", DESCRIPTION);

            // Generate a translation .pot 
            ModUtil.RegisterForTranslation(typeof(EmptyWorldPatch));

            //Load the sprite from Asteroid_Fuleria.dds (converted online from png) and set "generation action" to incorporated ressources
            Sprite empteraSprite = Utilities.Sprites.CreateSpriteDXT5(Assembly.GetExecutingAssembly().GetManifestResourceStream("EmptyWorld.Asteroid_Emptera.dds"), 512, 512);
            Assets.Sprites.Add("Asteroid_Emptera", empteraSprite);

        }
    }
}
