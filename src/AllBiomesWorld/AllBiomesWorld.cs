using Harmony;
using System.Reflection;
using UnityEngine;

namespace AllBiomesWorld
{
    [HarmonyPatch(typeof(Db), "Initialize")]
    class AllBiomesWorldPatch
    {

        public static LocString NAME = "Fuleria";
        public static LocString DESCRIPTION = "A beautifull little world (for less lags!) which contains all biomes.\n\nDuplicants will have no difficulty surviving but will have to adapt to the small size of this world.";

        public static void Prefix()
        {
            // Add strings used in Fuleria.yaml
            Strings.Add($"STRINGS.WORLDS.FULERIA.NAME", NAME);
            Strings.Add($"STRINGS.WORLDS.FULERIA.DESCRIPTION", DESCRIPTION);

            // Generate a translation .pot 
            ModUtil.RegisterForTranslation(typeof(AllBiomesWorldPatch));

            // Load the sprite from Asteroid_Fuleria.dds (converted online from png) and set "generation action" to incorporated ressources
            Sprite fuleriaSprite = Utilities.Sprites.CreateSpriteDXT5(Assembly.GetExecutingAssembly().GetManifestResourceStream("AllBiomesWorld.Asteroid_Fuleria.dds"), 512, 512);
            Assets.Sprites.Add("Asteroid_Fuleria", fuleriaSprite);

        }
    }
}
