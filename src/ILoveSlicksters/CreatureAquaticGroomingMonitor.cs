using Klei.AI;

namespace ILoveSlicksters
{
    public class CreatureAquaticGroomingMonitor : GameStateMachine<CreatureAquaticGroomingMonitor, CreatureAquaticGroomingMonitor.Instance, IStateMachineTarget, CreatureAquaticGroomingMonitor.Def>

    {
        public override void InitializeStates(out BaseState default_state)
        {

            default_state = satisfiedState;

            Effect aquariumEffect = new Effect("InAquarium", PHO_STRINGS.INAQUARIUM.NAME, "This creature is in an aquarium.", 0f, true, true, false);
            aquariumEffect.Add(new AttributeModifier(Db.Get().CritterAttributes.Happiness.Id, 4f, PHO_STRINGS.INAQUARIUM.NAME));
            aquariumEffect.Add(new AttributeModifier(Db.Get().Amounts.Wildness.deltaAttribute.Id, -0.02f, PHO_STRINGS.INAQUARIUM.NAME));
            Effect notInAquariumEffect = new Effect("NotInAquarium", PHO_STRINGS.NOTINAQUARIUM.NAME, "This creature must be in an aquarium to be tame.\nAn aquarium is a closed room that is between 20 and 120 tiles with any water.", 0f, true, true, true);


            satisfiedState.Transition(notSatisfiedState, (Instance smi) => !smi.IsInAquarium(), UpdateRate.SIM_4000ms);
            notSatisfiedState.Transition(satisfiedState, (Instance smi) => smi.IsInAquarium(), UpdateRate.SIM_4000ms);

            satisfiedState.Enter(delegate (Instance smi)
            {
            });

            notSatisfiedState.Enter(delegate (Instance smi)
            {
            });

            satisfiedState.ToggleEffect((Instance smi) => aquariumEffect);
            notSatisfiedState.ToggleEffect((Instance smi) => notInAquariumEffect);
        }

        public State notSatisfiedState;
        public State satisfiedState;

        public class Def : BaseDef
        {
        }

        public new class Instance : GameInstance
        {
            public Instance(IStateMachineTarget master, Def def) : base(master, def)
            {
            }

            public bool IsInAquarium()
            {
                int cell = Grid.PosToCell(gameObject);
                
                // Must be in liquid (water) to be considered in an aquarium
                if (!Grid.IsLiquid(cell) || !Grid.Element[cell].HasTag(GameTags.AnyWater))
                {
                    return false;
                }
                
                // Check room size (20-120 tiles)
                CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(cell);
                if (cavityForCell == null || cavityForCell.NumCells > 120) return false;
                return cavityForCell.NumCells > 20;
            }

        }
    }
}
