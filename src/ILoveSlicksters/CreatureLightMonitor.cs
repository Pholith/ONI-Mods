using Klei.AI;
using STRINGS;

namespace ILoveSlicksters
{

    public class CreatureLightMonitor : GameStateMachine<CreatureLightMonitor, CreatureLightMonitor.Instance, IStateMachineTarget, CreatureLightMonitor.Def>

    {
        public override void InitializeStates(out StateMachine.BaseState default_state)
        {

            default_state = this.satisfiedState;

            // Create and configure the light effect
            Effect needLightEffect = new Effect("NeedLight", "Need Light", "This creature needs light to live properly.", 0f, true, true, true);
            needLightEffect.Add(new AttributeModifier(Db.Get().CritterAttributes.Metabolism.Id, -40f, CREATURES.MODIFIERS.UNHAPPY.NAME));
            needLightEffect.Add(new AttributeModifier(Db.Get().CritterAttributes.Happiness.Id, -10f, CREATURES.MODIFIERS.UNHAPPY.NAME));


            satisfiedState.Transition(needLightState, (CreatureLightMonitor.Instance smi) => smi.IsInDark(), UpdateRate.SIM_1000ms);
            needLightState.Transition(satisfiedState, (CreatureLightMonitor.Instance smi) => !smi.IsInDark(), UpdateRate.SIM_1000ms);

            satisfiedState.Enter(delegate (CreatureLightMonitor.Instance smi)
            {
            });

            needLightState.Enter(delegate (CreatureLightMonitor.Instance smi)
            {
            });

            // Toggle the effects when enter or ewit un needLight state
            needLightState.ToggleEffect((CreatureLightMonitor.Instance smi) => needLightEffect);
        }

        public GameStateMachine<CreatureLightMonitor, CreatureLightMonitor.Instance, IStateMachineTarget, CreatureLightMonitor.Def>.State satisfiedState;
        public GameStateMachine<CreatureLightMonitor, CreatureLightMonitor.Instance, IStateMachineTarget, CreatureLightMonitor.Def>.State needLightState;

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
                return !(Grid.LightIntensity[Grid.PosToCell(transform.GetPosition())] > 0);
            }

        }
    }
}
