using UnityEngine;

namespace ILoveSlicksters
{
    [EntityConfigOrder(2)]
    public class AquaOilfloaterBabyConfig : IEntityConfig, IHasDlcRestrictions
    {
        public GameObject CreatePrefab()
        {
            GameObject gameObject = AquaOilfloaterConfig.CreateOilfloater(ID, PHO_STRINGS.VARIANT_AQUA.BABY.NAME, PHO_STRINGS.VARIANT_AQUA.BABY.DESC, "baby_aqua_oilfloater_kanim", true);
            EntityTemplates.ExtendEntityToBeingABaby(gameObject, AquaOilfloaterConfig.ID);
            return gameObject;
        }

        public void OnPrefabInit(GameObject prefab)
        {
        }

        public void OnSpawn(GameObject inst)
        {
        }

        public string[] GetAnyRequiredDlcIds()
        {
            return null;
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
        public const string ID = AquaOilfloaterConfig.ID+"Baby";

    }
}