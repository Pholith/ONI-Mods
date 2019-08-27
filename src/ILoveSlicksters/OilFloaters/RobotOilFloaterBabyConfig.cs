using UnityEngine;

namespace ILoveSlicksters
{
    public class RobotOilfloaterBabyConfig : IEntityConfig
    {
        public GameObject CreatePrefab()
        {
            GameObject gameObject = RobotOilfloaterConfig.CreateOilfloater(ID, StringsPatch.VARIANT_ROBOT.BABY.NAME, StringsPatch.VARIANT_ROBOT.BABY.DESC, "custom_baby_oilfloater", true);
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