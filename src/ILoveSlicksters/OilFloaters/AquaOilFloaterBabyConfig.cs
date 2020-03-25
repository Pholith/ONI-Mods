using UnityEngine;

namespace ILoveSlicksters
{
    public class AquaOilfloaterBabyConfig : IEntityConfig
    {
        public GameObject CreatePrefab()
        {
            GameObject gameObject = AquaOilfloaterConfig.CreateOilfloater(ID, PHO_STRINGS.VARIANT_AQUA.BABY.NAME, PHO_STRINGS.VARIANT_AQUA.BABY.DESC, "custom_baby_oilfloater_kanim", true);
            EntityTemplates.ExtendEntityToBeingABaby(gameObject, "AquaOilfloater");
            return gameObject;
        }

        public void OnPrefabInit(GameObject prefab)
        {
        }

        public void OnSpawn(GameObject inst)
        {
        }

        public const string ID = "AquaOilfloaterBaby";

    }
}