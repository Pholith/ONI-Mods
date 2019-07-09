using Harmony;

namespace OldSteamTurbine
{

    [HarmonyPatch(typeof(SteamTurbineConfig), "CreateBuildingDef")]
    public class OldSteamTurbine
    {
        public static void Postfix(SteamTurbineConfig __instance, ref BuildingDef __result)
        {
            __result.Deprecated = false;
        }
    }
}
