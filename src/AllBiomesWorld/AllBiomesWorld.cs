using Harmony;

namespace AllBiomeWorld
{
    [HarmonyPatch(typeof(Db), "Initialize")]
    class AllBiomeWorldPatch
    {
        
        public static LocString NAME = "Fuleria";
        public static LocString DESCRIPTION = "A beautifull little world (for less lags!) which contains all biomes.\n\nDuplicants will have no difficulty surviving but will have to adapt to the small size of this world.";

        public static void Prefix()
        {

            Strings.Add($"STRINGS.WORLDS.FULERIA.NAME", NAME);
            Strings.Add($"STRINGS.WORLDS.FULERIA.DESCRIPTION", DESCRIPTION);

            ModUtil.RegisterForTranslation(typeof(AllBiomeWorldPatch));
        }
    }
}
