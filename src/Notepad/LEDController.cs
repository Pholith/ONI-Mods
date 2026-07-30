namespace Notepad
{
    public class LEDController : GameStateMachine<LEDController, LEDController.Instance>
    {
        public override void InitializeStates(out BaseState default_state)
        {
            default_state = this.off;
            off.PlayAnim("off").EventTransition(GameHashes.OperationalChanged, on, (Instance smi) => OnCondition(smi));

            on.PlayAnim("on").EventTransition(GameHashes.OperationalChanged, off, (Instance smi) => !OnCondition(smi))
                .Enter("SetActive", delegate (Instance smi)
                {
                    smi.Operational.SetActive(true);
                });
        }

        private bool OnCondition(Instance smi)
        {
            return smi.Operational.IsOperational && smi.LogicPorts.IsPortConnected(LogicOperationalController.PORT_ID);
        }

        public LEDController()
        {
        }

        public State off;

        public State on;

        public class Def : BaseDef
        {
            public Def()
            {
            }
        }

        public new class Instance : GameInstance
        {
            public Instance(IStateMachineTarget master, Def def) : base(master, def)
            {
                // Cache component so it's even better then Klei's code.
                Operational = GetComponent<Operational>();
                LogicPorts = GetComponent<LogicPorts>();
            }

            public Operational Operational;
            public LogicPorts LogicPorts;
        }
    }
}