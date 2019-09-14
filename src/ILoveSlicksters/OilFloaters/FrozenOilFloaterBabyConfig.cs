using UnityEngine;

namespace ILoveSlicksters
{
    public class FrozenOilfloaterBabyConfig : IEntityConfig
    {
        public GameObject CreatePrefab()
        {
            GameObject gameObject = FrozenOilfloaterConfig.CreateOilfloater(ID, StringsPatch.VARIANT_FROZEN.BABY.NAME, StringsPatch.VARIANT_FROZEN.BABY.DESC, kanim_id, true);
            EntityTemplates.ExtendEntityToBeingABaby(gameObject, "FrozenOilfloater");
            return gameObject;
        }

        public void OnPrefabInit(GameObject prefab)
        {
        }

        public void OnSpawn(GameObject inst)
        {
        }

        private const string kanim_id = "baby_oilfloater_kanim";
        public const string ID = "FrozenOilfloaterBaby";

    }
}