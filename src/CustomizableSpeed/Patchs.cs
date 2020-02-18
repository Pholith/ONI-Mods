using Harmony;
using System;
using UnityEngine;

namespace CustomizableSpeed
{
	[HarmonyPatch(typeof(SpeedControlScreen), "OnChanged")]
	
	internal static class CustomizableSpeedPatch
	{
		private static bool Prefix(SpeedControlScreen __instance)
		{
			bool isPaused = __instance.IsPaused;
			if (isPaused)
			{
				Time.timeScale = 0f;
			}
			else
			{
				switch (__instance.GetSpeed())
				{
					case 0:
						Time.timeScale = 5f;
						break;
					case 1:
						Time.timeScale = 5f;
						break;
					case 2:
						Time.timeScale = 5f;
						break;

					default:
						break;
				}
			}
			return false;
		}
	}
}
