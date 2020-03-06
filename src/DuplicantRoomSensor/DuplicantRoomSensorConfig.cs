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
            string anim = "duplicant_room_sensor_kanim";
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
            buildingDef.AlwaysOperational = true;
            buildingDef.LogicOutputPorts = new List<LogicPorts.Port>
            {
                LogicPorts.Port.OutputPort(LogicSwitch.PORT_ID, new CellOffset(0, 0), 
                STRINGS.BUILDINGS.PREFABS.LOGICCRITTERCOUNTSENSOR.LOGIC_PORT,
                STRINGS.BUILDINGS.PREFABS.LOGICCRITTERCOUNTSENSOR.LOGIC_PORT_ACTIVE, 
                STRINGS.BUILDINGS.PREFABS.LOGICCRITTERCOUNTSENSOR.LOGIC_PORT_INACTIVE, true, false)
            };

            SoundEventVolumeCache.instance.AddVolume("switchgaspressure_kanim", "PowerSwitch_on", NOISE_POLLUTION.NOISY.TIER3);
            SoundEventVolumeCache.instance.AddVolume("switchgaspressure_kanim", "PowerSwitch_off", NOISE_POLLUTION.NOISY.TIER3);
            GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, ID);
            return buildingDef;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go.AddOrGet<LogicDuplicantCountSensor>().manuallyControlled = false;
            go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayInFrontOfConduits, false);

        }

        public static string ID = "DuplicantRoomSensor";
        public static string NAME = UI.FormatAsLink("Duplicant Room Sensor", "DUPLICANTROOMSENSOR");
        public static string DESC = "Detecting duplicant populations can help adjust their automated needs.";
        public static string EFFECT = string.Concat(new string[]
                {
                    "Sends a ",
                    UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active),
                    " or a ",
                    UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby),
                    " based on the number of duplicants in a room."
                });
    }

}
