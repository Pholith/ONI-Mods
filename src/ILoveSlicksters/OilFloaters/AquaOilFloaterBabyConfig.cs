using UnityEngine;

namespace ILoveSlicksters
{
    public class AquaOilfloaterBabyConfig : IEntityConfig, IHasDlcRestrictions
    {
        public GameObject CreatePrefab()
        {
            GameObject gameObject = AquaOilfloaterConfig.CreateOilfloater(ID, PHO_STRINGS.VARIANT_AQUA.BABY.NAME, PHO_STRINGS.VARIANT_AQUA.BABY.DESC, "baby_aqua_oilfloater_kanim", true);
            EntityTemplates.ExtendEntityToBeingABaby(gameObject, "AquaOilfloater");
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
        public const string ID = "AquaOilfloaterBaby";

    }
}