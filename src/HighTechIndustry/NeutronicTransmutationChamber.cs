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
            base.Sim1000ms(dt);
            ports.SendSignal(IsWorking, operational.IsActive ? 1 : 0);
        }

    }
}
    