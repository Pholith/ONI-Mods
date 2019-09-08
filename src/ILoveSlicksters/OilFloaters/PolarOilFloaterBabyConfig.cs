using UnityEngine;

namespace ILoveSlicksters
{
    public class PolarOilfloaterBabyConfig : IEntityConfig
    {
        public GameObject CreatePrefab()
        {
            GameObject gameObject = PolarOilfloaterConfig.CreateOilfloater(ID, StringsPatch.VARIANT_POLAR.BABY.NAME, StringsPatch.VARIANT_POLAR.BABY.DESC, kanim_id, true);
            EntityTemplates.ExtendEntityToBeingABaby(gameObject, "PolarOilfloater");
            return gameObject;
        }

        public void OnPrefabInit(GameObject prefab)
        {
        }

        public void OnSpawn(GameObject inst)
        {
        }

        private const string kanim_id = "baby_oilfloater_kanim";
        public const string ID = "PolarOilfloaterBaby";

    }
}