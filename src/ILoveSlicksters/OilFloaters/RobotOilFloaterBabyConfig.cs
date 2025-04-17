using UnityEngine;

namespace ILoveSlicksters
{
    [EntityConfigOrder(2)]
    public class RobotOilfloaterBabyConfig : IEntityConfig, IHasDlcRestrictions
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
        public string[] GetDlcIds()
        {
            return DlcManager.AVAILABLE_ALL_VERSIONS;
        }

        public string[] GetRequiredDlcIds()
        {
            return new string[0];
        }

        public string[] GetForbiddenDlcIds()
        {
            return new string[0];
        }
        public const string ID = "RobotOilfloaterBaby";

    }
}