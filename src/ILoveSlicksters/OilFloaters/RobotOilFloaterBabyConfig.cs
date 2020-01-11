using UnityEngine;

namespace ILoveSlicksters
{
    public class RobotOilfloaterBabyConfig : IEntityConfig
    {
        public GameObject CreatePrefab()
        {
            GameObject gameObject = RobotOilfloaterConfig.CreateOilfloater(ID, PHO_STRINGS.VARIANT_ROBOT.BABY.NAME, PHO_STRINGS.VARIANT_ROBOT.BABY.DESC, "custom_baby_oilfloater_kanim", true);
            EntityTemplates.ExtendEntityToBeingABaby(gameObject, "RobotOilfloater");
            return gameObject;
        }

        public void OnPrefabInit(GameObject prefab)
        {
        }

        public void OnSpawn(GameObject inst)
        {
        }

        public const string ID = "RobotOilfloaterBaby";

    }
}