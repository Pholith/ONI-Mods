using System;

namespace HighTechIndustry
{
    /// <summary>
    /// Code that handle the meter animation, thanks SGT_Imalas
    /// </summary>
    public class HEPStorageMeterHandler : KMonoBehaviour
    {
        protected override void OnSpawn()
        {
            base.OnSpawn();
            hepMeter = new MeterController(kbac, "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[]
            {
                "meter_target",
                "meter_fill",
                "meter_frame",
                "meter_OL"
            });
            Subscribe(GameHashes.OnParticleStorageChanged.GetHashCode(), new Action<object>(OnParticleStorageChanged));
            OnParticleStorageChanged(null);
        }

        protected override void OnCleanUp()
        {
            Unsubscribe(GameHashes.OnParticleStorageChanged.GetHashCode(), new Action<object>(OnParticleStorageChanged));
            base.OnCleanUp();
        }

        private void OnParticleStorageChanged(object data)
        {
            hepMeter.SetPositionPercent(hepStorage.Particles / hepStorage.Capacity());
        }

        public HEPStorageMeterHandler()
        {
        }

        [MyCmpReq]
        private HighEnergyParticleStorage hepStorage;

        [MyCmpReq]
        private KBatchedAnimController kbac;

        private MeterController hepMeter;
    }
}