using KSerialization;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Notepad
{
    public class Notepad : KMonoBehaviour, ISaveLoadable
    {
        protected override void OnSpawn()
        {
            base.OnSpawn();

            KBatchedAnimController animController = gameObject.AddOrGet<KBatchedAnimController>();
            animController.Play(contentText.IsNullOrWhiteSpace() ? "empty" : "full", KAnim.PlayMode.Paused);
        }
        protected override void OnCleanUp()
        {
            base.OnCleanUp();
        }

        [Serialize]
        public string contentText = "";

        [Serialize]
        public int tooltipFontSize = 20;

        [Serialize]
        public string iconName = "icon_category_furniture";

        public static void Blueprints_SetData(GameObject source, JObject data)
        {
            if (source.TryGetComponent<Notepad>(out var behavior))
            {
                var t1 = data.GetValue("contentText");
                if (t1 != null)
                {
                    behavior.contentText = t1.Value<string>();
                }
                var t2 = data.GetValue("tooltipFontSize");
                if (t2 != null)
                {
                    behavior.tooltipFontSize = t2.Value<int>();
                }
                var t3 = data.GetValue("iconName");
                if (t3 != null)
                {
                    behavior.iconName = t2.Value<string>();
                }
            }
        }
        public static JObject Blueprints_GetData(GameObject source)
        {
            if (source.TryGetComponent<Notepad>(out var behavior))
            {
                return new JObject()
                {
                    { "contentText", behavior.contentText},
                    { "tooltipFontSize", behavior.tooltipFontSize},
                    { "iconName", behavior.iconName},
                };
            }
            return null;
        }
    }
}
