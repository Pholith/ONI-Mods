using PeterHan.PLib.UI;
using UnityEngine;

namespace Notepad
{
    public class NotepadSideScreen : SideScreenContent
    {
        
        private static readonly RectOffset OUTER_MARGIN = new RectOffset(6, 10, 6, 14);
        internal const int ROW_SPACING = 2;

        private PTextArea descriptionField;

        public Notepad currentTarget;

        protected override void OnPrefabInit()
        {
            BuildPanel();
            base.OnPrefabInit();
            titleKey = PHO_STRINGS.NOTEPAD.NAME.key.String;
            activateOnSpawn = true;
        }


        public PTextArea DescriptionArea()
        {
            return new PTextArea("description field")
            {
                FlexSize = Vector2.one,
                Text = (currentTarget != null) ? currentTarget.activateText : "",

                OnTextChanged = (go, text) =>
                {
                    currentTarget.activateText = text;
                    if (currentTarget == null) return;
                    // change the anim looking on the text
                    KBatchedAnimController animController = currentTarget.gameObject.AddOrGet<KBatchedAnimController>();
                    animController.Play(currentTarget.activateText.IsNullOrWhiteSpace() ? "empty" : "full", KAnim.PlayMode.Paused);

                },
                LineCount = GameOnLoadPatch.Settings.LineNumber,
            };
        }

        // Destroy and recrate the text area to update it text
        public void UpdateTextArea()
        {
            if (currentTarget == null) return;
            descriptionField = DescriptionArea();
            Transform panelTransform = ContentContainer.transform.Find("Text panel");
            Debug.Assert(panelTransform != null, "Panel transform shound never be null");

            Transform descriptionTransform = panelTransform.gameObject.transform.Find("description field");
            Debug.Assert(descriptionTransform != null, "Description area transform shound never be null");

            if (descriptionTransform == null) return;
            descriptionTransform.gameObject.DeleteObject();

            GameObject newTextArea = descriptionField.Build();
            newTextArea.transform.SetParent(panelTransform);
            newTextArea.transform.SetSiblingIndex(1);
            newTextArea.transform.localScale = Vector3.one;

        }
        public override void ClearTarget()
        {
            base.ClearTarget();
            currentTarget = null;
        }
        
        private void BuildPanel()
        {
            descriptionField = DescriptionArea();

            // this button does nothing but it enable to deselect the textarea and to valide the input without closing the sidescreen
            PButton validationButton = new PButton("button")
            {
                Sprite = PUITuning.Images.Checked,
                SpriteSize = new Vector2(25, 30),
            };

            PLabel descriptionLabel = new PLabel("description label")
            {
                Text = "Description",
                TextAlignment = TextAnchor.MiddleCenter,
            };


            PPanel panel = new PPanel("Text panel")
            {
                Direction = PanelDirection.Vertical,
                Alignment = TextAnchor.UpperLeft,
                Spacing = ROW_SPACING,
                FlexSize = Vector2.one,
            };

            panel.AddChild(descriptionLabel);
            panel.AddChild(descriptionField);
            panel.AddChild(validationButton);


            PPanel root = new PPanel("NotepadSideScreen")
            {
                Direction = PanelDirection.Vertical,
                Margin = OUTER_MARGIN,
                Alignment = TextAnchor.MiddleCenter,
                Spacing = 0,
                BackColor = PUITuning.Colors.BackgroundLight,
                FlexSize = Vector2.one,

            };
            root.AddChild(panel);
            ContentContainer = root.SetKleiBlueColor().AddTo(gameObject);//Build();
        }

        public override bool IsValidForTarget(GameObject target)
        {
            return target.GetComponent<Notepad>() != null;
        }

        public override void SetTarget(GameObject target)
        {
            if (target == null)
            {
                Debug.LogError("Invalid gameObject received");
                return;
            }
            currentTarget = target.GetComponent<Notepad>();
            if (currentTarget == null)
            {
                Debug.LogError("The gameObject received does not contain a Notepad component");
                return;
            }
            
            if (ContentContainer != null) UpdateTextArea();

        }
    }
}
