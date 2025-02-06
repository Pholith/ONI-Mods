using KSerialization;

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

    }
}
