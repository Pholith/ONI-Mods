namespace HighTechIndustry
{
    public class NeutronicTransmutationChamber : ComplexFabricator
    {

        public HashedString IsWorking;
        private LogicPorts ports;

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            ports = GetComponent<LogicPorts>();
        }

        public override void Sim1000ms(float dt)
        {
            ports.SendSignal(IsWorking, operational.IsOperational ? 1 : 0);
        }

    }
}
