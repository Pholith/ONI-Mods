using HarmonyLib;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace HDScreenShot
{

    [HarmonyPatch(typeof(ConfirmDialogScreen), "PopupConfirmDialog")]
    public class HDScreenshot_ConfirmDialogScreen
    {

        public static void Postfix(ConfirmDialogScreen __instance)
        {
            // Ensure we're on the good ConfirmDialogScreen
            Transform foundLabel = __instance.GetComponentsInChildren<Transform>().FirstOrDefault(c => c.gameObject.name == "Label");
            if (foundLabel == null) return;
            LocText locText = foundLabel.GetComponent<LocText>();
            if (locText == null || locText.text != HDScreenshot_PauseScreenOnPrefabInit.POPUP_ID) return;



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
                    DumpGo(builder, child, depth + 1);
                }
                builder.Append(TabDepth(depth)).AppendLine("}");
            }
        }
    }
}
