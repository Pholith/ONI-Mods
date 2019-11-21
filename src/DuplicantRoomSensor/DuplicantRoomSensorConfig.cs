using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TUNING;
using UnityEngine;

namespace DuplicantRoomSensor 
{
    class DuplicantRoomSensorConfig : IBuildingConfig
    {
        public override BuildingDef CreateBuildingDef()
        {
            string id = LogicCritterCountSensorConfig.ID;
            int width = 1;
            int height = 1;
            string anim = "critter_sensor_kanim";
            int hitpoints = 30;
            float construction_time = 30f;
            float[] tier = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER0;
            string[] refined_METALS = MATERIALS.REFINED_METALS;
            float melting_point = 1600f;
            BuildLocationRule build_location_rule = BuildLocationRule.Anywhere;
            EffectorValues none = NOISE_POLLUTION.NONE;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tier, refined_METALS, melting_point, build_location_rule, TUNING.BUILDINGS.DECOR.PENALTY.TIER0, none, 0.2f);
            buildingDef.Overheatable = false;
            buildingDef.Floodable = false;
            buildingDef.Entombable = false;
            buildingDef.ViewMode = OverlayModes.Logic.ID;
            buildingDef.AudioCategory = "Metal";
            buildingDef.SceneLayer = Grid.SceneLayer.Building;
            SoundEventVolumeCache.instance.AddVolume("switchgaspressure_kanim", "PowerSwitch_on", NOISE_POLLUTION.NOISY.TIER3);
            SoundEventVolumeCache.instance.AddVolume("switchgaspressure_kanim", "PowerSwitch_off", NOISE_POLLUTION.NOISY.TIER3);
            GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, LogicCritterCountSensorConfig.ID);
            return buildingDef;
        }

        // Token: 0x06006077 RID: 24695 RVA: 0x001D9C5C File Offset: 0x001D805C
        public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
        {
            GeneratedBuildings.RegisterLogicPorts(go, LogicCritterCountSensorConfig.OUTPUT_PORT);
        }

        // Token: 0x06006078 RID: 24696 RVA: 0x001D9C69 File Offset: 0x001D8069
        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
            GeneratedBuildings.RegisterLogicPorts(go, LogicCritterCountSensorConfig.OUTPUT_PORT);
        }

        // Token: 0x06006079 RID: 24697 RVA: 0x001D9C78 File Offset: 0x001D8078
        public override void DoPostConfigureComplete(GameObject go)
        {
            GeneratedBuildings.MakeBuildingAlwaysOperational(go);
            GeneratedBuildings.RegisterLogicPorts(go, LogicCritterCountSensorConfig.OUTPUT_PORT);
            LogicCritterCountSensor logicCritterCountSensor = go.AddOrGet<LogicCritterCountSensor>();
            logicCritterCountSensor.manuallyControlled = false;
        }

        // Token: 0x040067A3 RID: 26531
        public static string ID = "LogicCritterCountSensor";

        // Token: 0x040067A4 RID: 26532
        public static readonly LogicPorts.Port OUTPUT_PORT = LogicPorts.Port.OutputPort(LogicSwitch.PORT_ID, new CellOffset(0, 0), STRINGS.BUILDINGS.PREFABS.LOGICCRITTERCOUNTSENSOR.LOGIC_PORT, STRINGS.BUILDINGS.PREFABS.LOGICCRITTERCOUNTSENSOR.LOGIC_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.LOGICCRITTERCOUNTSENSOR.LOGIC_PORT_INACTIVE, true, false);
    }


}
