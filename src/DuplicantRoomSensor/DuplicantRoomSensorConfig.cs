using STRINGS;
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
            string id = ID;
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
            GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, ID);
            return buildingDef;
        }

        public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
        {
            GeneratedBuildings.RegisterLogicPorts(go, OUTPUT_PORT);
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
            GeneratedBuildings.RegisterLogicPorts(go, OUTPUT_PORT);
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            GeneratedBuildings.MakeBuildingAlwaysOperational(go);
            GeneratedBuildings.RegisterLogicPorts(go, OUTPUT_PORT);
            LogicDuplicantCountSensor sensor = go.AddOrGet<LogicDuplicantCountSensor>();
            sensor.manuallyControlled = false;
        }

        public static string ID = "DuplicantRoomSensor";
        public static readonly LogicPorts.Port OUTPUT_PORT = LogicPorts.Port.OutputPort(LogicSwitch.PORT_ID, new CellOffset(0, 0), STRINGS.BUILDINGS.PREFABS.LOGICCRITTERCOUNTSENSOR.LOGIC_PORT, STRINGS.BUILDINGS.PREFABS.LOGICCRITTERCOUNTSENSOR.LOGIC_PORT_ACTIVE,STRINGS.BUILDINGS.PREFABS.LOGICCRITTERCOUNTSENSOR.LOGIC_PORT_INACTIVE,true, false);
        public static string NAME = "Duplicant Room Sensor";
        public static string DESC = "1.2";
        public static string EFFECT = "";
    }

}
