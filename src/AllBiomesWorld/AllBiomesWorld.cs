using HarmonyLib;
using KMod;
using System.Reflection;
using UnityEngine;

namespace AllBiomesWorld
{
    [HarmonyPatch(typeof(Db), "Initialize")]
    public class AllBiomesWorldPatch
    {

        public static LocString NAME = "Fuleria";
        public static LocString DESCRIPTION = "A beautifull little world (for less lags!) which contains all biomes.\n\nDuplicants will have no difficulty surviving but will have to adapt to the small size of this world.";

        public static void Prefix()
        {
            // Add strings used in Fuleria.yaml
            Strings.Add($"STRINGS.WORLDS.FULERIA.NAME", NAME);
            Strings.Add($"STRINGS.WORLDS.FULERIA.DESCRIPTION", DESCRIPTION);

        }
    }
    public class AllBiomesWorld : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            //PUtil.InitLibrary();
            //new PLocalization().Register();
        }
    }
}
