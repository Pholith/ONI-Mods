/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PholithModsTest
{

    public class CustomGameTags
    {
        public static readonly Tag NeedLight = TagManager.Create("NeedLight");
        public static void OnLoad()
        {
            TagManager.Create("NeedLight");
        }
    }
    public class CreatureLightMonitor : GameStateMachine<CreatureLightMonitor, CreatureLightMonitor.Instance, IStateMachineTarget, CreatureLightMonitor.Def>
  
    {
        public override void InitializeStates(out StateMachine.BaseState default_state)
        {
            default_state = this.satisfied;
            this.root.ToggleBehaviour(CustomGameTags.NeedLight, (CreatureLightMonitor.Instance smi) => smi.IsInDark(), null);
            this.satisfied.Enter("SetNavType", delegate (CreatureLightMonitor.Instance smi)
            {
                smi.GetComponent<Navigator>().SetCurrentNavType(NavType.Hover);
            }).Update("SetNavType", delegate (CreatureLightMonitor.Instance smi, float dt)
            {
                smi.GetComponent<Navigator>().SetCurrentNavType(NavType.Hover);
            }, UpdateRate.SIM_1000ms, false).Transition(this.Light, (CreatureLightMonitor.Instance smi) => smi.IsInDark(), UpdateRate.SIM_1000ms);
            this.Light.Enter("SetNavType", delegate (CreatureLightMonitor.Instance smi)
            {
                smi.GetComponent<Navigator>().SetCurrentNavType(NavType.Swim);
            }).Update("SetNavType", delegate (CreatureLightMonitor.Instance smi, float dt)
            {
                smi.GetComponent<Navigator>().SetCurrentNavType(NavType.Swim);
            }, UpdateRate.SIM_1000ms, false).Transition(this.satisfied, (CreatureLightMonitor.Instance smi) => !smi.IsInDark(), UpdateRate.SIM_1000ms);
        }

        public GameStateMachine<CreatureLightMonitor, CreatureLightMonitor.Instance, IStateMachineTarget, CreatureLightMonitor.Def>.State satisfied;

        public GameStateMachine<CreatureLightMonitor, CreatureLightMonitor.Instance, IStateMachineTarget, CreatureLightMonitor.Def>.State Light;

        public class Def : StateMachine.BaseDef
        {
        }

        public new class Instance : GameStateMachine<CreatureLightMonitor, CreatureLightMonitor.Instance, IStateMachineTarget, CreatureLightMonitor.Def>.GameInstance
        {
            public Instance(IStateMachineTarget master, CreatureLightMonitor.Def def) : base(master, def)
            {
            }

            public bool IsInDark()
            {
                return Grid.LightIntensity[Grid.PosToCell(base.transform.GetPosition())] > 0;
            }
        }
    }
}
*/