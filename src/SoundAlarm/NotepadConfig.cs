using TUNING;
using UnityEngine;

namespace Notepad
{
    public class NotepadConfig : IBuildingConfig
    {
        public override BuildingDef CreateBuildingDef()
        {
            string id = ID;
            int width = 1;
            int height = 1;
            string anim = "ceilinglight_kanim";
            int hitpoints = 10;
            float construction_time = 10f;
            float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER1;
            string[] all_METALS = MATERIALS.ALL_METALS;
            float melting_point = 800f;
            BuildLocationRule build_location_rule = BuildLocationRule.Anywhere;
            EffectorValues none = NOISE_POLLUTION.NONE;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tier, all_METALS, melting_point, build_location_rule, BUILDINGS.DECOR.NONE, none, 0.2f);
            buildingDef.ViewMode = OverlayModes.Decor.ID;
            buildingDef.Floodable = false;
            buildingDef.Overheatable = false;
            buildingDef.AudioCategory = "Metal";
            return buildingDef;
        }

        public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
        {
            go.AddOrGet<Notepad>();
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
        }

        public const string ID = "Notepad";
        public const string NAME = "Notepad test ";
        public const string DESC = "Notepad";
        public const string EFFECT = "Notepad";
    }
}
