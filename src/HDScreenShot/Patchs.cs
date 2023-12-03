using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using Pholib;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace HDScreenShot
{

    public class HDScreenshot : UserMod2
    {
        public static HDScreenshotOptions Settings { get; private set; }

        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            PUtil.InitLibrary();
            new POptions().RegisterOptions(this, typeof(HDScreenshotOptions));
        }
    }

    // Load options when launching the game
    [HarmonyPatch(typeof(Game), "Load")]
    public static class GameOnLoadPatch
    {
        public static HDScreenshotOptions Settings { get; private set; }

        public static void Prefix()
        {
            ReadSettings();
        }
        public static void ReadSettings()
        {
            // read the option each time the game is loaded - so we don't need to restart all the game
            Settings = POptions.ReadSettings<HDScreenshotOptions>();
            if (Settings == null || Settings.ScreenshotWidth < 1 || Settings.ScreenshotHeight < 1)
            {
                Settings = new HDScreenshotOptions();
            }

        }
    }


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

            buttonsList.Insert(buttonsList.Count - 2,
                new KButtonMenu.ButtonInfo("Take a HD Screenshot", Action.NumActions,
                new UnityAction(() =>
                {
                    //UnityEngine.ScreenCapture.CaptureScreenshot(this.GetScreenshotFileName(), 2);

                    __instance.StartCoroutine(Screenshot());
                })));

            instance.Field("buttons").SetValue(buttonsList.ToArray());
        }

        static Vector3 savedCameraPos;
        static float savedOrthographicSize;
        static float savedMaxOrthographicSize;
        public static IEnumerator Screenshot()
        {
            while (true)
            {
                yield return SequenceUtil.WaitForEndOfFrame;

                savedCameraPos = CameraController.Instance.transform.position;
                savedOrthographicSize = CameraController.Instance.OrthographicSize;
                savedMaxOrthographicSize = Traverse.Create(CameraController.Instance).Field<float>("maxOrthographicSize").Value;

                //OverlayScreen.Instance.ToggleOverlay(OverlayModes.None.ID);

                ///// Timelaspser.Render first pass
                CameraController.Instance.timelapseFreezeCamera.enabled = true; // Hide the baseCamera moving during the screenshot
                DebugHandler.SetTimelapseMode(true, ClusterManager.Instance.activeWorldId);

                ////// Timelaspser.SetPostionAndOrtho
                Traverse.Create(Game.Instance.timelapser).Method("SetPositionAndOrtho", ClusterManager.Instance.activeWorld).GetValue();
                float targetOrthographicSize = ClusterManager.Instance.activeWorld.Width - 5; // 5 is just here to reduce the black offset
                CameraController.Instance.SetMaxOrthographicSize(targetOrthographicSize);
                CameraController.Instance.OrthographicSize = targetOrthographicSize;

                OverlayScreen.Instance.ToggleOverlay(OverlayModes.None.ID);
                ////// Timelaspser.Render
                WorldContainer world = ClusterManager.Instance.activeWorld; //ClusterManager.Instance.GetWorld(world_id);
                if (world == null)
                {
                    yield break;
                }
                yield return SequenceUtil.WaitForNextFrame;
                yield return SequenceUtil.WaitForEndOfFrame;

                CameraController.Instance.SetPosition(new Vector3((world.WorldOffset.x + world.WorldSize.x / 2), (world.WorldOffset.y + world.WorldSize.y / 2), CameraController.Instance.transform.position.z));
                // Traverse.Create(CameraController.Instance).Method("Update").GetValue();
                CameraController.Instance.VisibleArea.Update();

                // Wait the frame to be rendered
                yield return SequenceUtil.WaitForNextFrame;
                yield return SequenceUtil.WaitForEndOfFrame;

                ///// Timelaspser.Render second pass

                ///// Timelaspser.RenderAndPrint

                // Store renderText
                RenderTexture active = RenderTexture.active;
                RenderTexture screenshotTexture = new RenderTexture(GameOnLoadPatch.Settings.ScreenshotWidth, GameOnLoadPatch.Settings.ScreenshotHeight, 32, RenderTextureFormat.ARGB32);
                RenderTexture.active = screenshotTexture;

                LayerMask mask = Traverse.Create(CameraController.Instance).Field<LayerMask>("timelapseCameraCullingMask").Value;
                LayerMask mask2 = Traverse.Create(CameraController.Instance).Field<LayerMask>("timelapseOverlayCameraCullingMask").Value;


                RenderCameraForTimelapse(CameraController.Instance.baseCamera, ref screenshotTexture, mask);
                CameraController.Instance.overlayCamera.clearFlags = CameraClearFlags.Nothing;
                RenderCameraForTimelapse(CameraController.Instance.overlayCamera, ref screenshotTexture, mask2);

                CustomWriteToFile(screenshotTexture);

                // Set previous values
                CameraController.Instance.timelapseFreezeCamera.enabled = false;
                DebugHandler.SetTimelapseMode(false);
                CameraController.Instance.SetPosition(savedCameraPos);
                CameraController.Instance.OrthographicSize = savedOrthographicSize;
                CameraController.Instance.SetMaxOrthographicSize(savedMaxOrthographicSize);
                RenderTexture.active = active;
                yield break;
            }
        }
        /// <summary>
        /// Copy of CameraController.RenderCameraForTimelapse because it can't be used easily since it is private and ref keyword.
        /// </summary>
        private static void RenderCameraForTimelapse(Camera cam, ref RenderTexture tex, LayerMask mask, float overrideAspect = -1f)
        {

            int cullingMask = cam.cullingMask;
            RenderTexture targetTexture = cam.targetTexture;
            cam.targetTexture = tex;
            cam.aspect = (float)tex.width / (float)tex.height;
            if (overrideAspect != -1f)
            {
                cam.aspect = overrideAspect;
            }
            if (mask != -1)
            {
                cam.cullingMask = mask;
            }
            cam.Render();
            cam.ResetAspect();
            cam.cullingMask = cullingMask;
            cam.targetTexture = targetTexture;
        }

        public static void CustomWriteToFile(Texture renderTex)
        {
            Texture2D texture2D = new Texture2D(renderTex.width, renderTex.height, TextureFormat.ARGB32, false);
            texture2D.ReadPixels(new Rect(0f, 0f, renderTex.width, renderTex.height), 0, 0);
            texture2D.Apply();

            byte[] bytes;

            switch (GameOnLoadPatch.Settings.SavedImageFormat)
            {
                default:
                case HDScreenshotOptions.ImageFormat.jpg:
                    bytes = texture2D.EncodeToJPG();
                    break;

                case HDScreenshotOptions.ImageFormat.png:
                    bytes = texture2D.EncodeToPNG();
                    break;
            }

            UnityEngine.Object.Destroy(texture2D);

            if (!Directory.Exists(Util.RootFolder()))
            {
                Directory.CreateDirectory(Util.RootFolder());
            }
            string imagePath = Path.Combine(Util.RootFolder(), "HD_Screenshots");
            if (!Directory.Exists(imagePath))
            {
                Directory.CreateDirectory(imagePath);
            }
            string name = RetireColonyUtility.StripInvalidCharacters(SaveGame.Instance.BaseName);

            string format = "0000.##_";
            imagePath = imagePath + Path.DirectorySeparatorChar + name + "_cycle_" + GameClock.Instance.GetCycle().ToString(format);

            int imageNumber = 1;
            while (File.Exists(string.Concat(imagePath, imageNumber, ".", GameOnLoadPatch.Settings.SavedImageFormat.ToString())))
            {
                imageNumber += 1;
            }
            string finalPath = string.Concat(imagePath, imageNumber, ".", GameOnLoadPatch.Settings.SavedImageFormat.ToString());

            Logs.Log($"Saving screenshot to {finalPath}");
            File.WriteAllBytes(finalPath, bytes);
            ShowDialogScreen(Instance, finalPath);
        }

        private static void ShowDialogScreen(PauseScreen pauseScreen, string savedPath)
        {
            var builder = new StringBuilder();

            builder.Append("Screenshot saved at: \n").Append(savedPath);

            ((ConfirmDialogScreen)GameScreenManager.Instance.StartScreen(
                ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject,
                pauseScreen.transform.parent.gameObject))
            .PopupConfirmDialog(builder.ToString(),
                () => { pauseScreen.gameObject.SetActive(true); },
                null,
                confirm_text: "CLOSE",
                title_text: "HD Screenshots");
        }

    }
}
