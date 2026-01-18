using UnityEngine;

namespace ILoveSlicksters
{
    [EntityConfigOrder(4)]
    public class FrozenOilfloaterBabyConfig : IEntityConfig, IHasDlcRestrictions
    {
        public GameObject CreatePrefab()
        {
            GameObject gameObject = FrozenOilfloaterConfig.CreateOilfloater(ID, PHO_STRINGS.VARIANT_FROZEN.BABY.NAME, PHO_STRINGS.VARIANT_FROZEN.BABY.DESC, kanim_id, true);
            EntityTemplates.ExtendEntityToBeingABaby(gameObject, "FrozenOilfloater");
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

        private const string kanim_id = "custom_baby_oilfloater2_kanim";
        public const string ID = "FrozenOilfloaterBaby";

    }
}