using UnityEngine;

namespace ILoveSlicksters
{
    public class LeafyOilfloaterBabyConfig : IEntityConfig
    {
        public GameObject CreatePrefab()
        {
            GameObject gameObject = LeafyOilfloaterConfig.CreateOilfloater(ID, PHO_STRINGS.VARIANT_LEAFY.BABY.NAME, PHO_STRINGS.VARIANT_LEAFY.BABY.DESC, kanim_id, true);
            EntityTemplates.ExtendEntityToBeingABaby(gameObject, "LeafyOilfloater");
            return gameObject;
        }

        public void OnPrefabInit(GameObject prefab)
        {
        }

        public void OnSpawn(GameObject inst)
        {
        }
        public string GetDlcId()
        {
            return "";
        }

        public const string ID = "LeafyOilfloaterBaby";
        private const string kanim_id = "custom_baby_oilfloater2_kanim";
    }
}