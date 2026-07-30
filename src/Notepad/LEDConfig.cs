using TUNING;
using UnityEngine;

namespace Notepad
{
    public class LEDConfig : IBuildingConfig
    {
        public override BuildingDef CreateBuildingDef()
        {
            string id = ID;
            int width = 1;
            int height = 1;
            string anim = "black_green_led_kanim";
            int hitpoints = 10;
            float construction_time = 5f;
            float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER_SMALL;
            string[] all_METALS = MATERIALS.REFINED_METALS;
            float melting_point = 800f;
            BuildLocationRule build_location_rule = BuildLocationRule.Anywhere;
            EffectorValues none = NOISE_POLLUTION.NONE;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tier, all_METALS, melting_point, build_location_rule, BUILDINGS.DECOR.NONE, none);
            buildingDef.ViewMode = OverlayModes.Logic.ID;
            buildingDef.Floodable = false;
            buildingDef.RequiresPowerInput = false;
            buildingDef.Overheatable = false;
            buildingDef.DefaultAnimState = "off";
            buildingDef.AudioCategory = "Metal";
            return buildingDef;
        }
        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            base.ConfigureBuildingTemplate(go, prefab_tag);
        }

        public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
        {
            base.DoPostConfigurePreview(def, go);
            GeneratedBuildings.RegisterSingleLogicInputPort(go);
        }
        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
            base.DoPostConfigureUnderConstruction(go);
            GeneratedBuildings.RegisterSingleLogicInputPort(go);
        }
        public override void DoPostConfigureComplete(GameObject go)
        {
            GeneratedBuildings.RegisterSingleLogicInputPort(go);

            // Show a red box telling it's not connected
            go.GetComponent<LogicPorts>().inputPortInfo[0].requiresConnection = true;

            // Make Operational reading logic port.
            go.AddOrGet<LogicOperationalController>(); 

            // Instead of LightController. LedController turn off the lamp when logic port is not connected.
            LEDController.Def ledController = go.AddOrGetDef<LEDController.Def>();
            
        }

        public const string ID = "LED";
    }
}
