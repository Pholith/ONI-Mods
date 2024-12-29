using KSerialization;

namespace Notepad
{
    public class Notepad : KMonoBehaviour, ISaveLoadable
    {
        protected override void OnSpawn()
        {
            base.OnSpawn();

            KBatchedAnimController animController = gameObject.AddOrGet<KBatchedAnimController>();
            animController.Play(activateText.IsNullOrWhiteSpace() ? "empty" : "full", KAnim.PlayMode.Paused);
        }
        protected override void OnCleanUp()
        {
            base.OnCleanUp();
        }

        [Serialize]
        public string activateText = "";
    }
}
