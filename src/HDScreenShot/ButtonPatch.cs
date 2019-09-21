using Harmony;
using System.Linq;

namespace HDScreenShot
{

    [HarmonyPatch(typeof(PauseScreen))]
    [HarmonyPatch("OnPrefabInit")]
    public static class PauseScreenOnPrefabInit
    {
        public static PauseScreen Instance;

        public static void Postfix(PauseScreen __instance)
        {

        var instance = Traverse.Create(__instance);
        var buttons = instance.Field("buttons").GetValue<KButtonMenu.ButtonInfo[]>();
        var buttonsList = buttons.ToList();
        Instance = __instance;

            buttonsList.Insert(buttonsList.Count - 2, new KButtonMenu.ButtonInfo("Take a HD Screenshot", Action.NumActions,
                () => { CustomTilapser.CreateScreen(Instance); }));

            instance.Field("buttons").SetValue(buttonsList.ToArray());
        }
    }
}
