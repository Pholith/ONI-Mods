using Klei.AI;
using KSerialization;
using STRINGS;
using System;
using UnityEngine;

namespace ILoveSlicksters
{

    public class GasDrowningMonitorUpdater : SlicedUpdaterSim1000ms<GasDrowningMonitor>
    {
        public GasDrowningMonitorUpdater() : base()
        {
            OnPrefabInit();
        }
    }

    public class GasDrowningMonitor : KMonoBehaviour, IWiltCause, ISlicedSim1000ms
    {
        private OccupyArea occupyArea
        {
            get
            {
                if (_occupyArea == null)
                {
                    _occupyArea = GetComponent<OccupyArea>();
                }
                return _occupyArea;
            }
        }

        public bool Drowning => drowning;

        private StatusItem NeedLiquid;

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            if (NeedLiquid == null)
            {
                NeedLiquid = new StatusItem("NeedLiquid", "CREATURES", "status_item_plant_liquid", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, StatusItem.ALL_OVERLAYS);
                Strings.Add($"STRINGS.CREATURES.STATUSITEMS.NEEDLIQUID.NAME", "NeedLiquid");
                Strings.Add($"STRINGS.CREATURES.STATUSITEMS.NEEDLIQUID.TOOLTIP", "This creature must be in water to survive !");
                NeedLiquid.resolveStringCallback = ((string str, object data) => str);
            }

            timeToDrown = 100f;
            if (drowningEffect == null)
            {
                drowningEffect = new Effect("GasDrowning", PHO_STRINGS.DROWNING.NAME, PHO_STRINGS.DROWNING.TOOLTIP, 0f, false, false, true, null, 0f, null);
                drowningEffect.Add(new AttributeModifier(Db.Get().CritterAttributes.Happiness.Id, -100f, CREATURES.STATUSITEMS.DROWNING.NAME, false, false, true));
            }
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();
            if (SlicedUpdaterSim1000ms<GasDrowningMonitor>.instance == null)
            {
                Game.Instance.gameObject.AddOrGet<GasDrowningMonitorUpdater>();
            }

            SlicedUpdaterSim1000ms<GasDrowningMonitor>.instance.RegisterUpdate1000ms(this);
            OnMove();
            CheckDrowning();
            Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(transform, new System.Action(OnMove), "GasDrowningMonitor.OnSpawn");
        }

        private void OnMove()
        {
            if (partitionerEntry.IsValid())
            {
                Extents extents = occupyArea.GetExtents();
                GameScenePartitioner.Instance.UpdatePosition(partitionerEntry, extents.x, extents.y);
            }
            else
            {
                partitionerEntry = GameScenePartitioner.Instance.Add("GasDrowningMonitor.OnSpawn", gameObject, occupyArea.GetExtents(), GameScenePartitioner.Instance.liquidChangedLayer, new Action<object>(OnLiquidChanged));
            }
            CheckDrowning();
        }

        protected override void OnCleanUp()
        {
            Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(transform, new System.Action(OnMove));
            GameScenePartitioner.Instance.Free(ref partitionerEntry);
            SlicedUpdaterSim1000ms<GasDrowningMonitor>.instance.UnregisterUpdate1000ms(this);
            base.OnCleanUp();
        }


        private void CheckDrowning(object data = null)
        {
            if (drowned)
            {
                return;
            }
            int cell = Grid.PosToCell(gameObject.transform.GetPosition());
            if (!IsCellSafe(cell))
            {
                if (!drowning)
                {
                    drowning = true;
                    Trigger((int)GameHashes.Drowning);
                    GetComponent<KPrefabID>().AddTag(GameTags.Creatures.Drowning, false);
                }
                if (timeToDrown <= 0f && canDrownToDeath)
                {
                    DeathMonitor.Instance smi = this.GetSMI<DeathMonitor.Instance>();
                    if (smi != null)
                    {
                        smi.Kill(Db.Get().Deaths.Drowned);
                    }
                    Trigger((int)GameHashes.Drowned);
                    drowned = true;
                }
            }
            else if (drowning)
            {
                drowning = false;
                GetComponent<KPrefabID>().RemoveTag(GameTags.Creatures.Drowning);
                Trigger((int)GameHashes.EnteredBreathableArea);

            }
            drowningStatusGuid = selectable.ToggleStatusItem(NeedLiquid, drowningStatusGuid, drowning, this);

            if (effects != null)
            {
                if (drowning)
                {
                    effects.Add(drowningEffect, false);
                    return;
                }
                else
                {
                    effects.Remove(drowningEffect);
                }
            }
        }

        private static bool CellSafeTest(int testCell, object data)
        {
            int num = Grid.CellAbove(testCell);

            GameObject obj = Grid.Objects[testCell, (int)ObjectLayer.Building];
            if (obj != null && obj.PrefabID() == EggIncubatorConfig.ID)
            {
                return true;
            }

            if (!Grid.IsValidCell(testCell) || !Grid.IsValidCell(num))
            {
                return false;
            }
            if (!Grid.IsLiquid(testCell))
            {
                return false;
            }
            if (!Grid.Element[testCell].HasTag(GameTags.AnyWater))
            {
                return false;
            }
            return true;
        }

        public bool IsCellSafe(int cell)
        {
            return occupyArea.TestArea(cell, this, CellSafeTestDelegate);
        }

        WiltCondition.Condition[] IWiltCause.Conditions => new WiltCondition.Condition[]
                {
                WiltCondition.Condition.Drowning
                };

        public string WiltStateString => CREATURES.STATUSITEMS.DROWNING.NAME;


        private void OnLiquidChanged(object data)
        {
            CheckDrowning();
        }

        public void SlicedSim1000ms(float dt)
        {
            CheckDrowning();
            if (drowning)
            {
                if (!drowned)
                {
                    timeToDrown -= dt;
                    if (timeToDrown <= 0f)
                    {
                        CheckDrowning();
                        return;
                    }
                }
            }
            else
            {
                timeToDrown += dt * 5f;
                timeToDrown = Mathf.Clamp(timeToDrown, 0f, 75f);
            }
        }

        [MyCmpReq]
        private readonly KSelectable selectable;

        [MyCmpGet]
        private readonly Effects effects;

        private OccupyArea _occupyArea;

        [Serialize]
        [SerializeField]
        private float timeToDrown;

        [Serialize]
        private bool drowned;

        private bool drowning;

        public bool canDrownToDeath = true;
        public SimHashes[] liquidsLivable = null;

        private Guid drowningStatusGuid;

        private HandleVector<int>.Handle partitionerEntry;

        public static Effect drowningEffect;

        private static readonly Func<int, object, bool> CellSafeTestDelegate = (int testCell, object data) => CellSafeTest(testCell, data);
    }
}