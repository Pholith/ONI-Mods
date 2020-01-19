using PeterHan.PLib.UI;
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

        public PTextField DescriptionField { get; private set; }


        public NotepadControl()
		{

            // Childs elements
            PTextField titleField = new PTextField("title field")
            {
                TextAlignment = TMPro.TextAlignmentOptions.Center,
                MinWidth = 192
            };

            //PButton buttonTest = new PButton();
            /*buttonTest.OnClick += delegate (GameObject obj4)
            {
                Debug.Log("test onClick");
            };
            */
            DescriptionField = new PTextField("description field")
            {
                FlexSize = Vector2.one, // new Vector2(1f, 10f),
            };
            DescriptionField.OnRealize += (obj4) => Debug.Log("On Realize");

            PLabel titleLabel = new PLabel("title label")
            {
                Text = "Title",
                TextAlignment = TextAnchor.MiddleCenter,
            };
            
            PSpacer space = new PSpacer();
            space.PreferredSize = new Vector2(0, 24f);

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

            panel.AddChild(titleLabel);
            panel.AddChild(titleField);
            panel.AddChild(space);
            panel.AddChild(descriptionLabel);
            panel.AddChild(DescriptionField);


            PPanel root = new PPanel("NotepadSideScreen")
            {
                Direction = PanelDirection.Vertical,
                Margin = OUTER_MARGIN,
                Alignment = TextAnchor.MiddleCenter,
                Spacing = 0,
                BackColor = PUITuning.Colors.BackgroundLight,
                FlexSize = Vector2.one
            };
            root.AddChild(panel);
            RootPanel = root.SetKleiBlueColor().Build();
        }
	}
}
