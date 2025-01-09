using UnityEngine;

namespace ILoveSlicksters
{

    public class SlicksterDance : GameStateMachine<SlicksterDance, SlicksterDance.Instance, IStateMachineTarget, SlicksterDance.Def>
    {
        public override void InitializeStates(out BaseState default_state)
        {
            default_state = normal;

            normal.Enter("SetNavType", delegate (Instance smi) { })
                .Update("SetNavType", delegate (Instance smi, float dt) { }, UpdateRate.SIM_4000ms, false).Transition(dancing, (Instance smi) => smi.MustDance(), UpdateRate.SIM_4000ms);


            dancing.Enter("SetNavType", delegate (Instance smi)
            {
                smi.GetComponent<Navigator>().SetCurrentNavType(NavType.Swim);
            })
                .Update("SetNavType", delegate (Instance smi, float dt)
                {
                    smi.GetComponent<Navigator>().SetCurrentNavType(NavType.Swim);
                }, UpdateRate.SIM_1000ms, false).Transition(normal, (Instance smi) => !smi.MustDance(), UpdateRate.SIM_1000ms);
        }

        public State normal;
        public State dancing;

        public class Def : BaseDef
        {
        }

        public new class Instance : GameInstance
        {
            public Instance(IStateMachineTarget master, Def def) : base(master, def)
            {
            }

            public bool MustDance()
            {
                GameObject obj = Grid.Objects[Grid.PosToCell(transform.GetPosition()), (int)ObjectLayer.Building];
                if (obj == null || obj.PrefabID() != PhonoboxConfig.ID) return false;
                Phonobox box = obj.AddOrGet<Phonobox>();
                return box.smi.GetCurrentState() == box.smi.sm.operational.playing;
            }
        }
    }
}
