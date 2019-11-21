using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DuplicantRoomSensor
{
    public class LogicDuplicantCountSensor : Switch, ISaveLoadable, IThresholdSwitch, ISim200ms
    {
        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            this.selectable = base.GetComponent<KSelectable>();
            base.Subscribe<LogicDuplicantCountSensor>(-905833192, LogicDuplicantCountSensor.OnCopySettingsDelegate);
        }

        private void OnCopySettings(object data)
        {
            GameObject gameObject = (GameObject)data;
            LogicDuplicantCountSensor component = gameObject.GetComponent<LogicDuplicantCountSensor>();
            if (component != null)
            {
                this.countThreshold = component.countThreshold;
                this.activateOnGreaterThan = component.activateOnGreaterThan;
            }
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();
            base.OnToggle += this.OnSwitchToggled;
            this.UpdateLogicCircuit();
            this.UpdateVisualState(true);
            this.wasOn = this.switchedOn;
        }

        public void Sim200ms(float dt)
        {
            Room roomOfGameObject = Game.Instance.roomProber.GetRoomOfGameObject(base.gameObject);
            if (roomOfGameObject != null)
            {
                this.currentCount = roomOfGameObject.cavity.creatures.Count;
                if (this.countEggs)
                {
                    this.currentCount += roomOfGameObject.cavity.eggs.Count;
                }
                bool state = (!this.activateOnGreaterThan) ? (this.currentCount < this.countThreshold) : (this.currentCount > this.countThreshold);
                this.SetState(state);
                if (this.selectable.HasStatusItem(Db.Get().BuildingStatusItems.NotInAnyRoom))
                {
                    this.selectable.RemoveStatusItem(this.roomStatusGUID, false);
                }
            }
            else
            {
                if (!this.selectable.HasStatusItem(Db.Get().BuildingStatusItems.NotInAnyRoom))
                {
                    this.roomStatusGUID = this.selectable.AddStatusItem(Db.Get().BuildingStatusItems.NotInAnyRoom, null);
                }
                this.SetState(false);
            }
        }

        private void OnSwitchToggled(bool toggled_on)
        {
            this.UpdateLogicCircuit();
            this.UpdateVisualState(false);
        }

        private void UpdateLogicCircuit()
        {
            LogicPorts component = base.GetComponent<LogicPorts>();
            component.SendSignal(LogicSwitch.PORT_ID, (!this.switchedOn) ? 0 : 1);
        }

        private void UpdateVisualState(bool force = false)
        {
            if (this.wasOn != this.switchedOn || force)
            {
                this.wasOn = this.switchedOn;
                KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
                component.Play((!this.switchedOn) ? "on_pst" : "on_pre", KAnim.PlayMode.Once, 1f, 0f);
                component.Queue((!this.switchedOn) ? "off" : "on", KAnim.PlayMode.Once, 1f, 0f);
            }
        }

        protected override void UpdateSwitchStatus()
        {
            StatusItem status_item = (!this.switchedOn) ? Db.Get().BuildingStatusItems.LogicSensorStatusInactive : Db.Get().BuildingStatusItems.LogicSensorStatusActive;
            base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, status_item, null);
        }

        public float Threshold
        {
            get
            {
                return (float)this.countThreshold;
            }
            set
            {
                this.countThreshold = (int)value;
            }
        }

        public bool ActivateAboveThreshold
        {
            get
            {
                return this.activateOnGreaterThan;
            }
            set
            {
                this.activateOnGreaterThan = value;
            }
        }

        public float CurrentValue
        {
            get
            {
                return (float)this.currentCount;
            }
        }

        public float RangeMin
        {
            get
            {
                return 0f;
            }
        }

        public float RangeMax
        {
            get
            {
                return 64f;
            }
        }

        public float GetRangeMinInputField()
        {
            return this.RangeMin;
        }

        public float GetRangeMaxInputField()
        {
            return this.RangeMax;
        }

        public LocString Title
        {
            get
            {
                return UI.UISIDESCREENS.CRITTER_COUNT_SIDE_SCREEN.TITLE;
            }
        }

        public LocString ThresholdValueName
        {
            get
            {
                return UI.UISIDESCREENS.CRITTER_COUNT_SIDE_SCREEN.VALUE_NAME;
            }
        }

        public string AboveToolTip
        {
            get
            {
                return UI.UISIDESCREENS.CRITTER_COUNT_SIDE_SCREEN.TOOLTIP_ABOVE;
            }
        }

        public string BelowToolTip
        {
            get
            {
                return UI.UISIDESCREENS.CRITTER_COUNT_SIDE_SCREEN.TOOLTIP_BELOW;
            }
        }

        public string Format(float value, bool units)
        {
            return value.ToString();
        }

        public float ProcessedSliderValue(float input)
        {
            return Mathf.Round(input);
        }

        public float ProcessedInputValue(float input)
        {
            return Mathf.Round(input);
        }

        public LocString ThresholdValueUnits()
        {
            return string.Empty;
        }

        public ThresholdScreenLayoutType LayoutType
        {
            get
            {
                return ThresholdScreenLayoutType.SliderBar;
            }
        }

        public int IncrementScale
        {
            get
            {
                return 1;
            }
        }

        public NonLinearSlider.Range[] GetRanges
        {
            get
            {
                return NonLinearSlider.GetDefaultRange(this.RangeMax);
            }
        }

        private bool wasOn;

        private bool countEggs = true;

        [Serialize]
        public int countThreshold;

        [Serialize]
        public bool activateOnGreaterThan = true;

        private int currentCount;

        private KSelectable selectable;

        private Guid roomStatusGUID;

        [MyCmpAdd]
        private CopyBuildingSettings copyBuildingSettings;

        private static readonly EventSystem.IntraObjectHandler<LogicDuplicantCountSensor> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<LogicDuplicantCountSensor>(delegate (LogicDuplicantCountSensor component, object data)
        {
            component.OnCopySettings(data);
        });
    }
}
