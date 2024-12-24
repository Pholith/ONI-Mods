using System;
using System.Collections.Generic;

namespace DuplicantRoomSensor
{
    public class DuplicantMonitor : GameStateMachine<DuplicantMonitor, DuplicantMonitor.Instance, IStateMachineTarget, DuplicantMonitor.Def>
    {
        public override void InitializeStates(out BaseState default_state)
        {
            default_state = this.root;

            State root = this.root;
            if (action == null)
            {
                action = new Action<Instance, float>(UpdateState);
            }

            root.Update(nameof(DuplicantMonitor), action, UpdateRate.SIM_1000ms, true);
        }

        private static Action<Instance, float> action;

        private static void UpdateCavity(Instance smi, float dt)
        {
            CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(Grid.PosToCell(smi));

            // If duplicant changed of cavity
            if (cavityForCell != smi.cavity)
            {
                KPrefabID component = smi.GetComponent<KPrefabID>();
                if (smi.cavity != null)
                {
                    if (!CavityInfoDuplicants.map.ContainsKey(smi.cavity))
                    {
                        CavityInfoDuplicants.map.Add(cavityForCell, new List<KPrefabID>());
                    }

                    CavityInfoDuplicants.map[smi.cavity].Remove(component);
                    Game.Instance.roomProber.UpdateRoom(cavityForCell);
                }
                smi.cavity = cavityForCell;
                if (smi.cavity != null)
                {
                    if (!CavityInfoDuplicants.map.ContainsKey(smi.cavity))
                    {
                        CavityInfoDuplicants.map.Add(cavityForCell, new List<KPrefabID>());
                    }

                    // don't add the duplicant if he's dead
                    if (smi.deathMonitor == null)
                    {
                        smi.deathMonitor = smi.GetSMI<DeathMonitor.Instance>();
                    }

                    if (smi.deathMonitor != null && smi.deathMonitor.IsDead())
                    {
                        return;
                    }

                    CavityInfoDuplicants.map[smi.cavity].Add(component);
                    Game.Instance.roomProber.UpdateRoom(smi.cavity);
                }
            }
            // Remove the duplicant if he died in this room
            /*else {
                if (smi.deathMonitor == null) smi.deathMonitor = smi.GetSMI<DeathMonitor.Instance>();
                if (smi.deathMonitor != null && smi.deathMonitor.IsDead()) {
                    KPrefabID component = smi.GetComponent<KPrefabID>();
                    Debug.Log(component.name);
                    if (!CavityInfoDuplicants.map.ContainsKey(smi.cavity)) CavityInfoDuplicants.map.Add(smi.cavity, new List<KPrefabID>());
                    if (CavityInfoDuplicants.map[smi.cavity].Contains(component)) CavityInfoDuplicants.map[smi.cavity].Remove(component);
                }
            }*/
        }

        private static void UpdateState(Instance smi, float dt)
        {
            UpdateCavity(smi, dt);
        }

        public new class Instance : GameInstance
        {
            public Instance(IStateMachineTarget master, Def def) : base(master, def)
            {
            }
            public CavityInfo cavity;
            public DeathMonitor.Instance deathMonitor;
        }

        public class Def : BaseDef
        {
        }
    }
}