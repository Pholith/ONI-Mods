using Harmony;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;

namespace HDScreenShot
{
    class CustomTilapser
    {
        public static Timelapser timelapser; // copy of in game Timelapser
        public static void CreateScreen(PauseScreen pauseScreen)
        {
            if (timelapser == null)
            {
                timelapser = GameObject.Instantiate(GetTimeLapser.trueTimeLapser);
            }
            Traverse.Create(timelapser).Field<RenderTexture>("bufferRenderTexture").Value =new RenderTexture(2048, 2304, 32, RenderTextureFormat.ARGB32);
            //Traverse.Create(timelapser).Field("screenshotPending").SetValue(true);
            //Traverse.Create(timelapser).Method("Render").GetValue();

            pauseScreen.StartCoroutine(Render());

            IEnumerator coroutine = WaitAndDialog(pauseScreen);
            pauseScreen.StartCoroutine(coroutine);

        }
        private static IEnumerator WaitAndDialog(PauseScreen pauseScreen)
        {
            // Wait 2 frame (To be sure Render function is end)
            for (int i = 0; i < 2; i++)
            {
                yield return new WaitForEndOfFrame();
            }
            ShowDialogScreen(pauseScreen);

        }
        private static void ShowDialogScreen(PauseScreen pauseScreen)
        {
            var builder = new StringBuilder();

            builder.Append("ScreenShot saved at: ").Append(savedPath);

            ((ConfirmDialogScreen)GameScreenManager.Instance.StartScreen(
                ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject,
                pauseScreen.transform.parent.gameObject))
            .PopupConfirmDialog(builder.ToString(),
                () => { pauseScreen.gameObject.SetActive(true); },
                null,
                confirm_text: "CLOSE",
                title_text: "HD Screenshots");
        }

        public static IEnumerator Render()
        {
            WaitForEndOfFrame wait = new WaitForEndOfFrame();
            for (;;)
            {
                yield return wait;

                if (!Traverse.Create(timelapser).Field<Camera>("freezeCamera").Value.enabled)
                {
                    timelapser.freezeTexture.ReadPixels(new Rect(0f, 0f, Camera.main.pixelWidth, Camera.main.pixelHeight), 0, 0);
                    timelapser.freezeTexture.Apply();
                    Traverse.Create(timelapser).Field<Camera>("freezeCamera").Value.gameObject.GetComponent<FillRenderTargetEffect>().SetFillTexture(timelapser.freezeTexture);
                    Traverse.Create(timelapser).Field<Camera>("freezeCamera").Value.enabled = true;
                    Traverse.Create(timelapser).Field<bool>("screenshotActive").Value = true;
                    Traverse.Create(timelapser).Method("SetPostionAndOrtho").GetValue();
                    DebugHandler.SetHideUI(true);
                }
                else
                {
                    Traverse.Create(timelapser).Method("RenderAndPrint").GetValue();
                    Traverse.Create(timelapser).Field<Camera>("freezeCamera").Value.enabled = false;
                    DebugHandler.SetHideUI(false);
                    Traverse.Create(timelapser).Field<bool>("screenshotPending").Value = false;
                    Traverse.Create(timelapser).Field<bool>("screenshotActive").Value = false;
                }
            }
            yield break;
        }

        private static string savedPath = "";
        // Adapted from WriteToPng method
        public static void CustomWriteToPng(RenderTexture renderTex)
        {
            Texture2D texture2D = new Texture2D(renderTex.width, renderTex.height, TextureFormat.ARGB32, false);
            texture2D.ReadPixels(new Rect(0f, 0f, renderTex.width, renderTex.height), 0, 0);
            texture2D.Apply();
            byte[] bytes = texture2D.EncodeToPNG();
            Object.Destroy(texture2D);
            if (!Directory.Exists(Util.RootFolder()))
            {
                Directory.CreateDirectory(Util.RootFolder());
            }
            string text = Path.Combine(Util.RootFolder(), "HD_Screenshots");
            if (!Directory.Exists(text))
            {
                Directory.CreateDirectory(text);
            }
            string name = RetireColonyUtility.StripInvalidCharacters(SaveGame.Instance.BaseName);

            DebugUtil.LogArgs(new object[]
            {
            "Saving screenshot to",
            text
            });
            savedPath = text;
            string format = "0000.##";
            text = text + Path.DirectorySeparatorChar + name + "_cycle_" + GameClock.Instance.GetCycle().ToString(format);

            File.WriteAllBytes(text + ".png", bytes);
        }
    }

    [HarmonyPatch(typeof(Timelapser))]
    [HarmonyPatch("RefreshRenderTextureSize")]
    class RefreshPatch
    {
        public static bool Prefix(Timelapser __instance)
        {
            if (__instance == CustomTilapser.timelapser)
            {
                return false; // don't execute the method if this is my timelapser
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Timelapser))]
    [HarmonyPatch("OnPrefabInit")]
    class GetTimeLapser
    {
        public static Timelapser trueTimeLapser;

        public static void Postfix(Timelapser __instance)
        {
            trueTimeLapser = __instance;
        }
    }


    [HarmonyPatch(typeof(Timelapser))]
    [HarmonyPatch("WriteToPng")]
    class WritePngPatch
    {
        public static bool Prefix(Timelapser __instance, RenderTexture renderTex)
        {
            if (__instance == CustomTilapser.timelapser)
            {
                CustomTilapser.CustomWriteToPng(renderTex);
                return false;
            }
            return true;
        }
    }

}
