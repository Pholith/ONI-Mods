using UnityEngine;

namespace ILoveSlicksters
{

    [EntityConfigOrder(2)]
    public class OwO_OilFloaterBabyConfig : IEntityConfig
    {
        public GameObject CreatePrefab()
        {
            GameObject gameObject = OwO_OilfloaterConfig.CreateOilFloater(ID, PHO_STRINGS.VARIANT_OWO.BABY.NAME, PHO_STRINGS.VARIANT_OWO.BABY.DESC, "custom_baby_oilfloater_kanim", true);
            EntityTemplates.ExtendEntityToBeingABaby(gameObject, OwO_OilfloaterConfig.ID, null);
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

        public const string ID = OwO_OilfloaterConfig.ID+"Baby";

    }
}
