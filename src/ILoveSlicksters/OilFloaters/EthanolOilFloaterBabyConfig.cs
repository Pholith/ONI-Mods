using UnityEngine;

namespace ILoveSlicksters
{
    public class EthanolOilfloaterBabyConfig : IEntityConfig
    {
        public GameObject CreatePrefab()
        {
            GameObject gameObject = EthanolOilfloaterConfig.CreateOilfloater(ID, StringsPatch.VARIANT_ETHANOL.BABY.NAME, StringsPatch.VARIANT_ETHANOL.BABY.DESC, "custom_baby_oilfloater", true);
            EntityTemplates.ExtendEntityToBeingABaby(gameObject, "EthanolOilfloater", null);
            return gameObject;
        }

        public void OnPrefabInit(GameObject prefab)
        {
        }

        public void OnSpawn(GameObject inst)
        {
        }

        public const string ID = "EthanolOilfloaterBaby";

    }
}
