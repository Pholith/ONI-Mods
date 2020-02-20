using Harmony;
using PeterHan.PLib;
using PeterHan.PLib.Options;
using UnityEngine;

namespace CustomizableSpeed
{
	[HarmonyPatch(typeof(Game), "Load")]
	public static class GameOnLoadPatch
	{
		public static SpeedOptions Settings { get; private set; }

		public static void Prefix()
		{
			// read the option each time the game is loaded - so we don't need to restart all the game
			Settings = POptions.ReadSettings<SpeedOptions>();
			if (Settings == null)
			{
				Settings = new SpeedOptions();
			}
		}

		public static void OnLoad()
		{
			PUtil.InitLibrary();
			POptions.RegisterOptions(typeof(SpeedOptions));
		}
	}

	[HarmonyPatch(typeof(SpeedControlScreen), "OnChanged")]
	public static class SpeedControlPatch
	{
		private static bool Prefix(SpeedControlScreen __instance)
		{
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
