using Pholib;
using UnityEngine;

namespace Notepad
{
    public class NotepadSideScreen : SideScreenContent
    {
        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            Logs.Log("PrefabInit");
            editButton = new KButton();
            editButton.onClick += delegate ()
            {
                ManagementMenu.Instance.SendMessage("Coucou Pholith!");
            };
        }

        public override bool IsValidForTarget(GameObject target)
        {
            Logs.Log("IsValid ??");
            return target.GetComponent<Notepad>() != null;
        }

        public override void SetTarget(GameObject target)
        {
            Logs.Log("SetTarget");
            if (target == null)
            {
                Debug.LogError("Invalid gameObject received");
                return;
            }
            targetNotepad = target.GetComponent<Notepad>();
            if (targetNotepad == null)
            {
                Debug.LogError("The gameObject received does not contain a Notepad component");
                return;
            }
            UpdateLabels();
        }

        private void UpdateLabels()
        {
            descriptionText.text = targetNotepad.activateText;

        }

        public Notepad targetNotepad;

        public LocText descriptionText;

        public KButton editButton;

    }
}
