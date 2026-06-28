using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Database;
using PeterHan.PLib.Options;
using Pholib;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HDScreenShot
{

    public class HDScreenshot : UserMod2
    {
        public static HDScreenshotOptions Settings { get; private set; }
        public static string modPath;

        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            PUtil.InitLibrary();
            new POptions().RegisterOptions(this, typeof(HDScreenshotOptions));

            modPath = path;
            Utilities.GenerateStringsTemplate(typeof(HD_STRINGS));
            new PLocalization().Register();

        }
    }

    /*[HarmonyPatch(typeof(Localization))]
    [HarmonyPatch("Initialize")]
    public static class Localization_Initialize_Patch
    {
        public static void Postfix()
        {
            Utilities.LoadTranslations(typeof(HD_STRINGS), HDScreenshot.modPath);
            LocString.CreateLocStringKeys(typeof(HD_STRINGS));
        }
    }*/

    // Load options when launching the game
    [HarmonyPatch(typeof(Game), "Load")]
    public static class HDScreenshot_GameOnLoadPatch
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
    [HarmonyPatch("ConfigureButtonInfos")]
    public static class HDScreenshot_PauseScreenOnPrefabInit
    {
        public static PauseScreen Instance;

        public static void Postfix(PauseScreen __instance)
        {

            var buttonsList = Traverse.Create(__instance).Field("buttons").GetValue<KButtonMenu.ButtonInfo[]>().ToList();
            Instance = __instance;

            buttonsList.Insert(buttonsList.Count - 2,
                new KButtonMenu.ButtonInfo(HD_STRINGS.UI.BUTTON_TEXT, Action.NumActions,
                new UnityAction(() =>
                {
                    //UnityEngine.ScreenCapture.CaptureScreenshot(this.GetScreenshotFileName(), 2);

                    __instance.StartCoroutine(Screenshot());
                })));
            __instance.SetButtons(buttonsList);
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
                //CameraController.Instance.timelapseFreezeCamera.enabled = true; // Hide the baseCamera moving during the screenshot
                DebugHandler.SetTimelapseMode(true, ClusterManager.Instance.activeWorldId);

                ////// Timelaspser.SetPostionAndOrtho
                Traverse.Create(Game.Instance.timelapser).Method("SetPositionAndOrtho", ClusterManager.Instance.activeWorld).GetValue();
                float targetOrthographicSize = ClusterManager.Instance.activeWorld.Width - 5; // 5 is just here to reduce the black offset
                CameraController.Instance.SetMaxOrthographicSize(targetOrthographicSize);
                CameraController.Instance.OrthographicSize = targetOrthographicSize;

                //OverlayScreen.Instance.ToggleOverlay(OverlayModes.None.ID);
                ////// Timelaspser.Render
                WorldContainer world = ClusterManager.Instance.activeWorld; //ClusterManager.Instance.GetWorld(world_id);
                if (world == null)
                {
                    yield break;
                }
                // yield return SequenceUtil.WaitForNextFrame; // DEBUG
                // yield return SequenceUtil.WaitForEndOfFrame; // DEBUG

                CameraController.Instance.SetPosition(new Vector3(world.WorldOffset.x + (world.WorldSize.x / 2), world.WorldOffset.y + (world.WorldSize.y / 2), CameraController.Instance.transform.position.z));
                // Traverse.Create(CameraController.Instance).Method("Update").GetValue();
                CameraController.Instance.VisibleArea.Update();

                // Wait the frame to be rendered
                yield return SequenceUtil.WaitForNextFrame;
                yield return SequenceUtil.WaitForEndOfFrame;

                ///// Timelaspser.Render second pass

                ///// Timelaspser.RenderAndPrint

                // Store renderText
                RenderTexture active = RenderTexture.active;
                RenderTexture screenshotTexture = new RenderTexture(HDScreenshot_GameOnLoadPatch.Settings.ScreenshotWidth, HDScreenshot_GameOnLoadPatch.Settings.ScreenshotHeight, 32, RenderTextureFormat.ARGB32);
                RenderTexture.active = screenshotTexture;

                LayerMask mask = Traverse.Create(CameraController.Instance).Field<LayerMask>("timelapseCameraCullingMask").Value;
                LayerMask mask2 = Traverse.Create(CameraController.Instance).Field<LayerMask>("timelapseOverlayCameraCullingMask").Value;
                RenderCameraForTimelapse(CameraController.Instance.baseCamera, ref screenshotTexture, mask);
                // CustomWriteToFile(screenshotTexture); // DEBUG PURPOSE
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
            cam.aspect = tex.width / (float)tex.height;
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

            switch (HDScreenshot_GameOnLoadPatch.Settings.SavedImageFormat)
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
            while (File.Exists(string.Concat(imagePath, imageNumber, ".", HDScreenshot_GameOnLoadPatch.Settings.SavedImageFormat.ToString())))
            {
                imageNumber += 1;
            }
            string finalPath = string.Concat(imagePath, imageNumber, ".", HDScreenshot_GameOnLoadPatch.Settings.SavedImageFormat.ToString());

            Logs.Log($"Saving screenshot to {finalPath}");
            File.WriteAllBytes(finalPath, bytes);
            ShowDialogScreen(Instance, finalPath);
        }


        private static void ShowDialogScreen(PauseScreen pauseScreen, string savedPath)
        {
            var builder = new StringBuilder();

            builder.Append(HD_STRINGS.UI.SAVED_AT);
            builder.AppendLine(Utilities.FormatColored(savedPath, "cdf0bb", false));
            builder.AppendLine();
            builder.Append(HD_STRINGS.UI.WARNING);
            builder.AppendLine();
            builder.Append(HD_STRINGS.UI.TIPS);

            ConfirmDialogScreen confirmScreen = (ConfirmDialogScreen)GameScreenManager.Instance.StartScreen(
                ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject,
                pauseScreen.transform.parent.gameObject);

            confirmScreen.PopupConfirmDialog(builder.ToString(),
                () =>
                {
                    pauseScreen.gameObject.SetActive(true);
                },
                null,
                confirm_text: HD_STRINGS.UI.CLOSE,
                title_text: "HD Screenshots");
        }

    }

    [HarmonyPatch(typeof(ConfirmDialogScreen), "PopupConfirmDialog")]
    public class HDScreenshot_ConfirmDialogScreen
    {

        public static void Postfix(ConfirmDialogScreen __instance)
        {
            // Ensure we're on the good ConfirmDialogScreen
            Transform foundLabel = __instance.GetComponentsInChildren<Transform>().FirstOrDefault(c => c.gameObject.name == "Label");
            if (foundLabel == null) return;
            LocText locText = foundLabel.GetComponent<LocText>();
            if (locText == null || locText.text != "HD Screenshots") return;

            

            /* // Inspect UI for debug
             * var builder = new StringBuilder();
            DumpGo(builder, __instance.transform);
            Logs.Log(builder);
            */

            // The gameobject with the panel that controls the width is really named "GameObject" ...
            Transform panelToGrow = __instance.GetComponentsInChildren<Transform>().FirstOrDefault(c => c.gameObject.name == "GameObject");
            if (panelToGrow == null) return;
            var rectTransform = panelToGrow.GetComponent<RectTransform>();
            if (rectTransform == null) return;
            
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x * 1.8f, rectTransform.sizeDelta.y);

        }

        // Generate spaces to read more easily the logs.
        private static string TabDepth(int depth)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < depth; i++)
            {
                builder.Append("  ");
            }
            return builder.ToString();
        }

        // Dump UI because it's pretty complicated.
        public static void DumpGo(StringBuilder builder, Transform go, int depth = 0)
        {
            builder.Append(TabDepth(depth)).Append("->GameObject: ").AppendLine(go.name);
            RectTransform rectTransform = go.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                builder.Append(TabDepth(depth)).AppendLine("->RectTransform");
                builder.Append(TabDepth(depth)).Append("rect: ").AppendLine(rectTransform.rect.ToString());
                builder.Append(TabDepth(depth)).Append("sizeDelta: ").AppendLine(rectTransform.sizeDelta.ToString());
            }
            LayoutElement layoutElement = go.GetComponent<LayoutElement>();
            if (layoutElement != null)
            {
                builder.Append(TabDepth(depth)).AppendLine("->LayoutElement");
                builder.Append(TabDepth(depth)).Append("preferredWidth: ").AppendLine(layoutElement.preferredWidth.ToString());
                builder.Append(TabDepth(depth)).Append("preferredHeight: ").AppendLine(layoutElement.preferredHeight.ToString());
                builder.Append(TabDepth(depth)).Append("minWidth: ").AppendLine(layoutElement.minWidth.ToString());
                builder.Append(TabDepth(depth)).Append("minHeight: ").AppendLine(layoutElement.minHeight.ToString());
            }

            LocText locText = go.GetComponent<LocText>();
            if (locText != null)
            {
                builder.Append(TabDepth(depth)).AppendLine("->LocText");
                builder.Append(TabDepth(depth)).Append("text: ").AppendLine(locText.text);
                builder.Append(TabDepth(depth)).Append("textInfo: ").AppendLine(locText.textInfo.ToString());
                builder.Append(TabDepth(depth)).Append("key: ").AppendLine(locText.key);
            }
            VerticalLayoutGroup verticalLayoutGroup = go.GetComponent<VerticalLayoutGroup>();
            if (verticalLayoutGroup != null)
            {
                builder.Append(TabDepth(depth)).AppendLine("->VerticalLayoutGroup");
                builder.Append(TabDepth(depth)).Append("preferredWidth: ").AppendLine(verticalLayoutGroup.preferredWidth.ToString());
                builder.Append(TabDepth(depth)).Append("preferredHeight: ").AppendLine(verticalLayoutGroup.preferredHeight.ToString());
                builder.Append(TabDepth(depth)).Append("minWidth: ").AppendLine(verticalLayoutGroup.minWidth.ToString());
                builder.Append(TabDepth(depth)).Append("minHeight: ").AppendLine(verticalLayoutGroup.minHeight.ToString());
            }

            if (go.childCount > 0)
            {
                builder.Append(TabDepth(depth)).AppendLine("->Childs: ");
                builder.Append(TabDepth(depth)).AppendLine("{");
                foreach (Transform child in go.transform)
                {
                    DumpGo(builder, child, depth+1);
                }
                builder.Append(TabDepth(depth)).AppendLine("}");
            }
        }
    }
}
