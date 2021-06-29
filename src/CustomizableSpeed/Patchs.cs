﻿using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
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


    [HarmonyPatch(typeof(SpeedControlScreen), "OnChanged")]
    public static class SpeedControlPatchOnChanged
    {
        public static bool Prefix(SpeedControlScreen __instance)
        {
            if (GameOnLoadPatch.Settings == null)
            {
                GameOnLoadPatch.ReadSettings();
            }

            if (__instance.IsPaused)
            {
                Time.timeScale = 0f;
            }
            else
            {
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
            }
            return false;
        }
    }
}
