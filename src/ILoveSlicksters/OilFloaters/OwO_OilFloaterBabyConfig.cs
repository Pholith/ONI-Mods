using UnityEngine;

namespace ILoveSlicksters
{

    public class OwO_OilFloaterBabyConfig : IEntityConfig
    {
        public GameObject CreatePrefab()
        {
            GameObject gameObject = OwO_OilFloaterConfig.CreateOilFloater(ID, StringsPatch.VARIANT_OWO.BABY.NAME, StringsPatch.VARIANT_OWO.BABY.DESC, "custom_baby_oilfloater_kanim", true);
            EntityTemplates.ExtendEntityToBeingABaby(gameObject, "OwO_Oilfloater", null);
            return gameObject;
        }

        public void OnPrefabInit(GameObject prefab)
        {
        }

        public void OnSpawn(GameObject inst)
        {
        }

        public const string ID = "OwO_OilfloaterBaby";

    }
}
