using PeterHan.PLib.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Notepad
{
    class NotepadControl
    {


		private static readonly RectOffset ELEMENT_MARGIN = new RectOffset(2, 2, 2, 2);
		private static readonly RectOffset OUTER_MARGIN = new RectOffset(6, 10, 6, 14);
		internal static readonly Vector2 PANEL_SIZE = new Vector2(240.0f, 360.0f);
        internal static readonly Vector2 ROW_SIZE = new Vector2(48, 48);
        internal const int ROW_SPACING = 2;
        
        public GameObject RootPanel { get; }


		public NotepadControl()
		{
            PTextField textField = new PTextField();
            textField.Text = "test 5";
            textField.ToolTip = "test 7";
            textField.FlexSize = Vector2.one;

            PPanel panel = new PPanel("Text panel")
            {
                Direction = PanelDirection.Vertical,
                Alignment = TextAnchor.UpperLeft,
                Spacing = ROW_SPACING,
                FlexSize = Vector2.one
            };
            panel.AddChild(textField);

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
