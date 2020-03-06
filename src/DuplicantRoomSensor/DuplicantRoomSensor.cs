using KSerialization;
using STRINGS;
using System;
using UnityEngine;

namespace DuplicantRoomSensor
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class LogicDuplicantCountSensor : Switch, ISaveLoadable, IThresholdSwitch, ISim200ms
    {
        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            selectable = GetComponent<KSelectable>();
            //Subscribe((int)GameHashes.CopySettings, OnCopySettingsDelegate);
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();
            OnToggle += OnSwitchToggled;
            UpdateLogicCircuit();
            UpdateVisualState(true);
            wasOn = switchedOn;
        }

        public void Sim200ms(float dt)
        {
            Room roomOfGameObject = Game.Instance.roomProber.GetRoomOfGameObject(gameObject);
            if (roomOfGameObject != null)
            {
                if (CavityInfoDuplicants.map.ContainsKey(roomOfGameObject.cavity))// CavityInfoDuplicants.map.Add(roomOfGameObject.cavity, new List<KPrefabID>());
                    currentCount = CavityInfoDuplicants.map[roomOfGameObject.cavity].Count;
                else currentCount = 0;

                bool state = (!activateOnGreaterThan) ? (currentCount < countThreshold) : (currentCount > countThreshold);
                SetState(state);
                if (selectable.HasStatusItem(Db.Get().BuildingStatusItems.NotInAnyRoom))
                {
                    selectable.RemoveStatusItem(roomStatusGUID, false);
                }
            }
            else
            {
                if (!selectable.HasStatusItem(Db.Get().BuildingStatusItems.NotInAnyRoom))
                {
                    roomStatusGUID = selectable.AddStatusItem(Db.Get().BuildingStatusItems.NotInAnyRoom, null);
                }
                SetState(false);
            }
        }

        private void OnSwitchToggled(bool toggled_on)
        {
            UpdateLogicCircuit();
            UpdateVisualState(false);
        }

        private void UpdateLogicCircuit()
        {
            GetComponent<LogicPorts>().SendSignal(LogicSwitch.PORT_ID, this.switchedOn ? 1 : 0);
        }

        private void UpdateVisualState(bool force = false)
        {
            if (wasOn != switchedOn || force)
            {
                wasOn = switchedOn;
                KBatchedAnimController component = GetComponent<KBatchedAnimController>();
                component.Play((!switchedOn) ? "on_pst" : "on_pre", KAnim.PlayMode.Once, 1f, 0f);
                component.Queue((!switchedOn) ? "off" : "on", KAnim.PlayMode.Once, 1f, 0f);
            }
        }

        protected override void UpdateSwitchStatus()
        {
            StatusItem status_item = (!switchedOn) ? Db.Get().BuildingStatusItems.LogicSensorStatusInactive : Db.Get().BuildingStatusItems.LogicSensorStatusActive;
            GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, status_item, null);
        }

        public float Threshold
        {
            get
            {
                return (float)countThreshold;
            }
            set
            {
                countThreshold = (int)value;
            }
        }

        public bool ActivateAboveThreshold
        {
            get
            {
                return activateOnGreaterThan;
            }
            set
            {
                activateOnGreaterThan = value;
            }
        }

        public float CurrentValue => currentCount;
        public float RangeMin => 0f;
        public float RangeMax => 64f;
        public float GetRangeMinInputField() => RangeMin;
        public float GetRangeMaxInputField() => RangeMax;

        public LocString Title => PHO_STRINGS.DUP_CRITTER_COUNT_SIDE_SCREEN.TITLE;
        public LocString ThresholdValueName => UI.FRONTEND.LOADSCREEN.DUPLICANTS_ALIVE;
        public string AboveToolTip => PHO_STRINGS.DUP_CRITTER_COUNT_SIDE_SCREEN.TOOLTIP_ABOVE;
        public string BelowToolTip => PHO_STRINGS.DUP_CRITTER_COUNT_SIDE_SCREEN.TOOLTIP_BELOW;

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

        public ThresholdScreenLayoutType LayoutType => ThresholdScreenLayoutType.SliderBar;

        public int IncrementScale => 1;

        public NonLinearSlider.Range[] GetRanges => NonLinearSlider.GetDefaultRange(RangeMax);

        private bool wasOn;

        [Serialize]
        public int countThreshold;

        [Serialize]
        public bool activateOnGreaterThan = true;

        private int currentCount;

        private KSelectable selectable;

        private Guid roomStatusGUID;

    }
}
