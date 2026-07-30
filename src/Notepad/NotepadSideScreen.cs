using PeterHan.PLib.UI;
using UnityEngine;

namespace Notepad
{
    public class NotepadSideScreen : SideScreenContent
    {
#pragma warning disable IDE1006 // Styles d'affectation de noms
        private const string TEXT_FIELD_NAME = "Notepad text field";
        private const string SIDE_SCREEN_PANEL_NAME = "NotepadSideScreenPanel";
        private const string TOOLTIP_PANEL_NAME = "tooltip panel";
        private const string FONT_SIZE_FIELD_NAME = "tooltip size";

        private static readonly RectOffset OUTER_MARGIN = new RectOffset(6, 10, 6, 14);
        private const int ROW_SPACING = 2;
        private const bool DEBUG_GRID = false;
        private const int GRID_ROW_SIZE = 30;
        private const int GRID_ICON_SIZE = 29;
        private const int GRID_COLUMN_NB = 9;
        private RectOffset GRID_ICON_MARGIN = new RectOffset();

#pragma warning restore IDE1006 // Styles d'affectation de noms

        public Notepad currentTarget;
        private PTextArea descriptionField;
        private PTextField toolTipSizeField;

        protected override void OnPrefabInit()
        {
            BuildPanel();
            base.OnPrefabInit();
            titleKey = PHO_STRINGS.NOTEPAD.NAME.key.String;
            activateOnSpawn = true;
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

            if (ContentContainer != null) UpdatePanels();
        }

        public override void ClearTarget()
        {
            base.ClearTarget();
            currentTarget = null;
        }

        private PTextArea CreateDescriptionArea()
        {
            return new PTextArea(TEXT_FIELD_NAME)
            {
                FlexSize = Vector2.one,
                TextAlignment = TMPro.TextAlignmentOptions.TopLeft,
                Text = (currentTarget != null) ? currentTarget.contentText : "",

                OnTextChanged = (go, text) =>
                {
                    if (currentTarget == null) return;
                    currentTarget.contentText = text;
                    // change the anim looking on the text
                    KBatchedAnimController animController = currentTarget.gameObject.AddOrGet<KBatchedAnimController>();
                    animController.Play(currentTarget.contentText.IsNullOrWhiteSpace() ? "empty" : "full", KAnim.PlayMode.Paused);

                },
                LineCount = GameOnLoadPatch.Settings.LineNumber,
            };
        }
        // Delete and update some fields, it is required because after build, fields don't update for others notepad if not reset like that. (Maybe there's a better handle?)
        public void UpdatePanels()
        {
            Debug.Assert(currentTarget != null, "Notepad shouldn't be null here");

            Transform panelTransform = ContentContainer.transform.Find(SIDE_SCREEN_PANEL_NAME);
            Debug.Assert(panelTransform != null, "Panel transform should never be null");

            Transform oldDescriptionTransform = panelTransform.gameObject.transform.Find(TEXT_FIELD_NAME);
            Debug.Assert(oldDescriptionTransform != null, "Description area transform should never be null");
            oldDescriptionTransform.gameObject.DeleteObject();

            descriptionField.Text = currentTarget.contentText;
            GameObject newTextArea = descriptionField.Build();
            newTextArea.transform.SetParent(panelTransform);
            newTextArea.transform.SetSiblingIndex(1);
            newTextArea.transform.localScale = Vector3.one;

            Transform tooltipPanel = panelTransform.gameObject.transform.Find(TOOLTIP_PANEL_NAME);
            Transform oldTooltipTransform = tooltipPanel.Find(FONT_SIZE_FIELD_NAME);
            Debug.Assert(oldTooltipTransform != null, "Tooltip size field area transform should never be null");
            oldTooltipTransform.gameObject.DeleteObject();

            toolTipSizeField.Text = currentTarget.tooltipFontSize.ToString();
            GameObject newToolTipSizeField = toolTipSizeField.Build();
            newToolTipSizeField.transform.SetParent(tooltipPanel);
            newToolTipSizeField.transform.SetSiblingIndex(1);
            newToolTipSizeField.transform.localScale = Vector3.one;

        }

        private static int iconRow = 0;
        private static int iconColumn = 0;

        private void AddIconButton(PGridPanel parentPanel, string spriteName)
        {
            PButton button = new PButton();
            button.OnClick += (go) =>
            {
                currentTarget.iconName = spriteName;
                UpdatePanels();
            };
            button.Sprite = Assets.GetSprite(spriteName);
            button.FlexSize = Vector2.zero;
            button.SpriteSize = new Vector2(GRID_ICON_SIZE, GRID_ICON_SIZE);
            button.IconSpacing = 0;
            button.Margin = GRID_ICON_MARGIN;
            parentPanel.AddChild(button, new GridComponentSpec(iconRow, iconColumn));
            iconColumn++;
            if (iconColumn >= GRID_COLUMN_NB)
            {
                iconColumn = 0;
                iconRow++;
                parentPanel.AddRow(new GridRowSpec(GRID_ROW_SIZE));
            }
        }


        private void BuildPanel()
        {
            iconRow = 0;
            iconColumn = 0;
            // this button does nothing but it enable to deselect the textarea and to valide the input without closing the sidescreen
            PButton validationButton = new PButton("button")
            {
                Sprite = PUITuning.Images.Checked,
                SpriteSize = new Vector2(50, 40),
                MaintainSpriteAspect = true,
                Margin = new RectOffset(15, 10, 15, 10),

            };
            PLabel descriptionLabel = new PLabel("description label")
            {
                Text = "Description",
                TextAlignment = TextAnchor.MiddleCenter,
            };
            descriptionField = CreateDescriptionArea();

            PPanel panel = new PPanel(SIDE_SCREEN_PANEL_NAME)
            {
                Direction = PanelDirection.Vertical,
                Alignment = TextAnchor.UpperLeft,
                Spacing = ROW_SPACING,
            };


            PGridPanel panelIconSelection = new PGridPanel()
            {
                FlexSize = Vector2.zero,
                Margin = new RectOffset(1, 1, -30, 1),
            };
#pragma warning disable CS0162 // Code inaccessible détecté
            if (DEBUG_GRID) panelIconSelection.BackColor = Color.yellow;
#pragma warning restore CS0162 // Code inaccessible détecté
            for (int i = 0; i < GRID_COLUMN_NB; i++)
            {
                panelIconSelection.AddColumn(new GridColumnSpec(flex: 100f / GRID_COLUMN_NB));
            }
            panelIconSelection.AddRow(new GridRowSpec(GRID_ROW_SIZE));

            AddIconButton(panelIconSelection, "icon_category_furniture");
            AddIconButton(panelIconSelection, "icon_category_electrical");
            AddIconButton(panelIconSelection, "icon_category_automation");
            AddIconButton(panelIconSelection, "icon_category_ventilation");
            AddIconButton(panelIconSelection, "icon_category_plumbing");
            AddIconButton(panelIconSelection, "icon_category_refinery");
            AddIconButton(panelIconSelection, "icon_errand_research");
            AddIconButton(panelIconSelection, "icon_category_misc");
            AddIconButton(panelIconSelection, "icon_action_cancel");
            AddIconButton(panelIconSelection, "icon_errand_build");
            AddIconButton(panelIconSelection, "icon_display_screen_status");
            AddIconButton(panelIconSelection, "icon_display_screen_blueprint");
            AddIconButton(panelIconSelection, "icon_display_screen_errands");
            AddIconButton(panelIconSelection, "icon_errand_toggle");
            AddIconButton(panelIconSelection, "icon_display_screen_properties");
            AddIconButton(panelIconSelection, "icon_category_lights");
            AddIconButton(panelIconSelection, "icon_errand_operate");
            AddIconButton(panelIconSelection, "overlay_heatflow");
            AddIconButton(panelIconSelection, "icon_category_morale");
            AddIconButton(panelIconSelection, "icon_category_utilities");
            AddIconButton(panelIconSelection, "icon_category_shipping");
            AddIconButton(panelIconSelection, "icon_errand_cook");
            AddIconButton(panelIconSelection, "icon_action_prioritize");
            AddIconButton(panelIconSelection, "icon_action_deprioritize");
            AddIconButton(panelIconSelection, "icon_category_equipment");
            AddIconButton(panelIconSelection, "icon_action_deconstruct");
            AddIconButton(panelIconSelection, "icon_errand_rocketry");
            AddIconButton(panelIconSelection, "icon_category_radiation");
            AddIconButton(panelIconSelection, "icon_errand_dig");
            AddIconButton(panelIconSelection, "icon_action_region_medical");
            AddIconButton(panelIconSelection, "icon_display_screen_germs");
            AddIconButton(panelIconSelection, "icon_action_harvest");
            AddIconButton(panelIconSelection, "icon_errand_farm");
            AddIconButton(panelIconSelection, "icon_action_disinfect");
            AddIconButton(panelIconSelection, "icon_action_region_disposal");
            AddIconButton(panelIconSelection, "icon_action_disconnect");

            PLabel toolTipSizeLabel = new PLabel("toolTipSizeLabel")
            {
                Text = "Tooltip Font Size",
                Margin = new RectOffset(0, 10, 2, 0)
            };

            toolTipSizeField = new PTextField(FONT_SIZE_FIELD_NAME)
            {
                FlexSize = Vector2.one,
                Type = PTextField.FieldType.Integer,
                Text = currentTarget?.tooltipFontSize.ToString() ?? "20",
                TextAlignment = TMPro.TextAlignmentOptions.Center,
            };
            toolTipSizeField.OnTextChanged += (go, text) =>
            {
                if (int.TryParse(text, out int size))
                {
                    currentTarget.tooltipFontSize = size;
                }
            };

            panel.AddChild(descriptionLabel);
            panel.AddChild(descriptionField);
            panel.AddChild(panelIconSelection);
            panel.AddChild(validationButton);
            panel.AddChild(new PPanel(TOOLTIP_PANEL_NAME) { Direction = PanelDirection.Horizontal, Spacing = ROW_SPACING }.AddChild(toolTipSizeLabel).AddChild(toolTipSizeField));
            //panel.AddChild(new PPanel { Direction = PanelDirection.Horizontal, Spacing = ROW_SPACING }.AddChild(colorSelectorLabel).AddChild(colorSelector));


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

    }
}
