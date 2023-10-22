using UnityEngine;

namespace ILoveSlicksters
{
    [EntityConfigOrder(2)]
    public class FrozenOilfloaterBabyConfig : IEntityConfig
    {
        public GameObject CreatePrefab()
        {
            GameObject gameObject = FrozenOilfloaterConfig.CreateOilfloater(ID, PHO_STRINGS.VARIANT_FROZEN.BABY.NAME, PHO_STRINGS.VARIANT_FROZEN.BABY.DESC, kanim_id, true);
            EntityTemplates.ExtendEntityToBeingABaby(gameObject, "FrozenOilfloater");
            return gameObject;
        }

        public void OnPrefabInit(GameObject prefab)
        {
        }

        public void OnSpawn(GameObject inst)
        {
        }
        public string[] GetDlcIds()
        {
            return DlcManager.AVAILABLE_ALL_VERSIONS;
        }

        private const string kanim_id = "custom_baby_oilfloater2_kanim";
        public const string ID = "FrozenOilfloaterBaby";

    }
}