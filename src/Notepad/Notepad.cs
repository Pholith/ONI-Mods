using KSerialization;

namespace Notepad
{
    public class Notepad : KMonoBehaviour, ISaveLoadable
    {
        protected override void OnSpawn()
        {
            base.OnSpawn();
        }
        protected override void OnCleanUp()
        {
            base.OnCleanUp();
        }

        [Serialize]
        public string activateText = "test a";
    }
}
