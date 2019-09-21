using Harmony;
using System;
using System.Text;
using UnityEngine;

namespace HDScreenShot
{
    class PopupFeedback
    {


        public static Timelapser timelapser;

        public static void CreateScreen(PauseScreen pauseScreen)
        {
            var builder = new StringBuilder();

            if (timelapser == null)
            {
                timelapser = GameObject.Instantiate(GetTimeLapser.trueTimeLapser);
            }
            Traverse.Create(timelapser).Field("bufferRenderTexture").SetValue(new RenderTexture(1024, 1536, 32, RenderTextureFormat.ARGB32));
            timelapser.
            Traverse.Create(timelapser).Method("Render").GetValue();



            ((ConfirmDialogScreen)GameScreenManager.Instance.StartScreen(
                    ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject,
                    pauseScreen.transform.parent.gameObject))
                .PopupConfirmDialog(builder.ToString(),
                    () => { pauseScreen.gameObject.SetActive(true); },
                    null,
                    confirm_text: "CLOSE",
                    title_text: "HD ScreenShot");
        }
    }

    [HarmonyPatch(typeof(Timelapser))]
    [HarmonyPatch("OnPrefabInit")]
    class GetTimeLapser
    {
        public static Timelapser trueTimeLapser;

        public static void Postfix(Timelapser __instance)
        {
            Debug.Log("i'm here");
            PopupFeedback.timelapser = __instance;
        }

    }
}
