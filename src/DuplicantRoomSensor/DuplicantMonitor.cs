using System;
using System.Collections.Generic;

namespace DuplicantRoomSensor
{
    public class DuplicantMonitor : GameStateMachine<DuplicantMonitor, DuplicantMonitor.Instance, IStateMachineTarget, DuplicantMonitor.Def>
    {
        public override void InitializeStates(out StateMachine.BaseState default_state)
        {
            default_state = this.root;
            GameStateMachine<DuplicantMonitor, DuplicantMonitor.Instance, IStateMachineTarget, DuplicantMonitor.Def>.State root = this.root;
            if (DuplicantMonitor.action == null) DuplicantMonitor.action = new Action<DuplicantMonitor.Instance, float>(DuplicantMonitor.UpdateState);

            root.Update(DuplicantMonitor.action, UpdateRate.SIM_1000ms, true);
        }

        private static Action<DuplicantMonitor.Instance, float> action;

        private static void UpdateCavity(DuplicantMonitor.Instance smi, float dt)
        {
            CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(Grid.PosToCell(smi));
            if (cavityForCell != smi.cavity)
            {
                KPrefabID component = smi.GetComponent<KPrefabID>();
                if (smi.cavity != null)
                {
                    if (!CavityInfoDuplicants.map.ContainsKey(smi.cavity)) CavityInfoDuplicants.map.Add(cavityForCell, new List<KPrefabID>());
                    CavityInfoDuplicants.map[smi.cavity].Remove(component);
                    //OvercrowdingMonitor.GetCreatureCollection(smi, smi.cavity).Remove(component);
                    Game.Instance.roomProber.UpdateRoom(cavityForCell);
                }
                smi.cavity = cavityForCell;
                if (smi.cavity != null)
                {
                    if (!CavityInfoDuplicants.map.ContainsKey(smi.cavity)) CavityInfoDuplicants.map.Add(cavityForCell, new List<KPrefabID>());
                    CavityInfoDuplicants.map[smi.cavity].Add(component);
                    //OvercrowdingMonitor.GetCreatureCollection(smi, smi.cavity).Add(component);
                    Game.Instance.roomProber.UpdateRoom(smi.cavity);
                }
            }
        }

        private static void UpdateState(DuplicantMonitor.Instance smi, float dt)
        {
            DuplicantMonitor.UpdateCavity(smi, dt);
        }

        public new class Instance : GameStateMachine<DuplicantMonitor, DuplicantMonitor.Instance, IStateMachineTarget, DuplicantMonitor.Def>.GameInstance
        {
            public Instance(IStateMachineTarget master, DuplicantMonitor.Def def) : base(master, def)
            {
            }
            public CavityInfo cavity;
        }

        public class Def : StateMachine.BaseDef
        {
        }
    }
}