using KSerialization;
using UnityEngine;

namespace High_Pressure_Applications.Components
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class Pressurized : KMonoBehaviour, ISaveLoadable
    {
        [SerializeField]
        public ConduitType ConduitType => conduit == null ? bridge.type : conduit.ConduitType;

        public bool IsBridge => bridge != null;

        private PressurizedInfo _Info;
        public PressurizedInfo Info
        {
            get
            {
                if (_Info == null || !_loadedInfo)
                {
                    _Info = PressurizedTuning.GetPressurizedInfo(building.Def.PrefabID);
                    if (_Info == null)
                        Debug.LogError("Could not retrieve pressurized configuration information!");
                    else
                        _loadedInfo = true;
                }
                return _Info;
            }
        }

        [MyCmpReq]
        private Building building;
        [MyCmpGet]
        private Conduit conduit;
        [MyCmpGet]
        private ConduitBridge bridge;

        private bool _loadedInfo = false;

        protected override void OnSpawn()
        {
            base.OnSpawn();
            if (conduit == null && bridge == null)
                Debug.LogError($"[Pressurized] Pressurized component should not be added unless there is an accomponying Conduit or ConduitBridge component.");
        }

        public int GetLayer()
        {
            if (IsBridge)
                return Integration.connectionLayers[(int)ConduitType];
            else
                return Integration.layers[(int)ConduitType];
        }

        public static float GetMaxCapacity(Pressurized pressure)
        {
            ConduitFlow manager = Conduit.GetFlowManager(pressure.ConduitType);
            if (IsDefault(pressure))
                return manager.MaxMass();
            else
                return pressure.Info.IncreaseMultiplier * 1f;
        }

        public static bool IsDefault(Pressurized pressure)
        {
            return pressure == null || pressure.Info == null || pressure.Info.IsDefault;
        }

    }
}
