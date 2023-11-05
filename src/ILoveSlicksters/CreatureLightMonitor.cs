using Klei.AI;
using STRINGS;

namespace ILoveSlicksters
{

    public class CreatureLightMonitor : GameStateMachine<CreatureLightMonitor, CreatureLightMonitor.Instance, IStateMachineTarget, CreatureLightMonitor.Def>
    {
        public override void InitializeStates(out BaseState default_state)
        {

            default_state = satisfiedState;

            // Create and configure the light effect
            Effect needLightEffect = new Effect("NeedLight", "Need Light", "This creature needs light to live properly.", 0f, true, true, true);
            needLightEffect.Add(new AttributeModifier(Db.Get().CritterAttributes.Metabolism.Id, -40f, CREATURES.MODIFIERS.MISERABLE.NAME));
            needLightEffect.Add(new AttributeModifier(Db.Get().CritterAttributes.Happiness.Id, -10f, CREATURES.MODIFIERS.MISERABLE.NAME));

            satisfiedState.Transition(needLightState, (Instance smi) => smi.IsInDark(), UpdateRate.SIM_1000ms);
            needLightState.Transition(satisfiedState, (Instance smi) => !smi.IsInDark(), UpdateRate.SIM_1000ms);
            
            satisfiedState.Enter(delegate (Instance smi)
            {
            });

            needLightState.Enter(delegate (Instance smi)
            {
            });

            // Toggle the effects when enter or ewit un needLight state
            needLightState.ToggleEffect((Instance smi) => needLightEffect);
        }

        public State satisfiedState;
        public State needLightState;

        public class Def : BaseDef
        {
        }

        public new class Instance : GameInstance
        {
            public Instance(IStateMachineTarget master, Def def) : base(master, def)
            {
            }

            public bool IsInDark()
            {
                return !(Grid.LightIntensity[Grid.PosToCell(transform.GetPosition())] > 0);
            }

        }
    }
}
