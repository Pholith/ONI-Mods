using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using STRINGS;
using UnityEngine;

namespace CustomizableSpeed
{
    public class CustomizableSpeed : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            PUtil.InitLibrary();
            new POptions().RegisterOptions(this, typeof(SpeedOptions));
        }
    }

    [HarmonyPatch(typeof(Game), "Load")]
    public static class GameOnLoadPatch
    {
        public static SpeedOptions Settings { get; private set; }

        public static void Prefix()
        {
            ReadSettings();
        }
        public static void ReadSettings()
        {
            // read the option each time the game is loaded - so we don't need to restart all the game
            Settings = POptions.ReadSettings<SpeedOptions>();
            if (Settings == null)
            {
                Settings = new SpeedOptions();
            }

        }
    }

    // Show the speed on hover
    [HarmonyPatch(typeof(SpeedControlScreen), nameof(SpeedControlScreen.ResetToolTip))]
    public class SpeedControlScreen_ResetToolTipPatch
    {
        public static void Postfix(SpeedControlScreen __instance, TextStyleSetting ___TooltipTextStyle)
        {
            if (GameOnLoadPatch.Settings == null)
            {
                GameOnLoadPatch.ReadSettings();
            }
            __instance.speedButtonWidget_slow.GetComponent<ToolTip>().ClearMultiStringTooltip();
            __instance.speedButtonWidget_medium.GetComponent<ToolTip>().ClearMultiStringTooltip();
            __instance.speedButtonWidget_fast.GetComponent<ToolTip>().ClearMultiStringTooltip();
            __instance.speedButtonWidget_slow.GetComponent<ToolTip>().AddMultiStringTooltip($"{GameUtil.ReplaceHotkeyString(UI.TOOLTIPS.SPEEDBUTTON_SLOW, Action.CycleSpeed)} (x{GameOnLoadPatch.Settings.slowSpeed})", ___TooltipTextStyle);
            __instance.speedButtonWidget_medium.GetComponent<ToolTip>().AddMultiStringTooltip($"{GameUtil.ReplaceHotkeyString(UI.TOOLTIPS.SPEEDBUTTON_SLOW, Action.CycleSpeed)} (x{GameOnLoadPatch.Settings.normalSpeed})", ___TooltipTextStyle);
            __instance.speedButtonWidget_fast.GetComponent<ToolTip>().AddMultiStringTooltip($"{GameUtil.ReplaceHotkeyString(UI.TOOLTIPS.SPEEDBUTTON_SLOW, Action.CycleSpeed)} (x{GameOnLoadPatch.Settings.superSpeed})", ___TooltipTextStyle);
        }
    }
    // Show the speed on hover
    [HarmonyPatch(typeof(SpeedControlScreen), "OnPrefabInit")]
    public class SpeedControlScreen_OnPrefabInitPatch
    {
        public static void Postfix(SpeedControlScreen __instance)
        {
            __instance.ResetToolTip();
        }
    }

    [HarmonyPatch(typeof(SpeedControlScreen), "OnChanged")]
    public static class SpeedControlPatchOnChanged
    {
        public static void Postfix(SpeedControlScreen __instance)
        {
            if (GameOnLoadPatch.Settings == null)
            {
                GameOnLoadPatch.ReadSettings();
            }

            if (__instance.IsPaused)
            {
                Time.timeScale = 0f;
                return;
            }
            switch (__instance.GetSpeed())
            {
                case 0:
                    Time.timeScale = GameOnLoadPatch.Settings.slowSpeed;
                    break;
                case 1:
                    Time.timeScale = GameOnLoadPatch.Settings.normalSpeed;
                    break;
                case 2:
                    Time.timeScale = GameOnLoadPatch.Settings.superSpeed;
                    break;

                default:
                    break;
            }
            return;
        }
    }
}
