using UnityEngine;

namespace ILoveSlicksters
{
    public class LeafyOilfloaterBabyConfig : IEntityConfig
    {
        public GameObject CreatePrefab()
        {
            GameObject gameObject = LeafyOilfloaterConfig.CreateOilfloater(ID, StringsPatch.VARIANT_LEAFY.BABY.NAME, StringsPatch.VARIANT_LEAFY.BABY.DESC, "oilfloater_kanim", true);
            EntityTemplates.ExtendEntityToBeingABaby(gameObject, "LeafyOilfloater");
            return gameObject;
        }

        public void OnPrefabInit(GameObject prefab)
        {
        }

        public void OnSpawn(GameObject inst)
        {
        }

        public const string ID = "LeafyOilfloaterBaby";

    }
}