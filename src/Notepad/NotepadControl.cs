using PeterHan.PLib.UI;
using Pholib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Notepad
{
    public class NotepadControl
    {

		private static readonly RectOffset OUTER_MARGIN = new RectOffset(6, 10, 6, 14);
		internal static readonly Vector2 PANEL_SIZE = new Vector2(240.0f, 360.0f);
        internal static readonly Vector2 ROW_SIZE = new Vector2(48, 48);
        internal const int ROW_SPACING = 2;
        
        public GameObject RootPanel { get; }

        private PTextArea descriptionField;

        public Notepad currentTarget;


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
                LineCount = 6,
            };
        }

        // Destroy and recrate the text area to update it text
        public void UpdateTextArea()
        {
            if (currentTarget == null) return;
            descriptionField = DescriptionArea();
            Transform panelTransform = RootPanel.transform.Find("Text panel");
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

        public NotepadControl()
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
            RootPanel = root.SetKleiBlueColor().Build();
        }
	}
}
