namespace HighTechIndustry
{
    public class NeutronicTransmutationChamber : ComplexFabricator
    {

        public HashedString IsWorking;
        private LogicPorts ports;
        private RadiationEmitter radiationEmitter;

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            ports = GetComponent<LogicPorts>();
            radiationEmitter = GetComponent<RadiationEmitter>();
        }

        public override void Sim1000ms(float dt)
        {
            base.Sim1000ms(dt);
            radiationEmitter.SetEmitting(operational.IsActive);
            ports.SendSignal(IsWorking, operational.IsActive ? 1 : 0);
        }
    }
}
    