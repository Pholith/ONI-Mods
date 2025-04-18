﻿using UnityEngine;

namespace ILoveSlicksters
{
    [EntityConfigOrder(2)]
    public class LeafyOilfloaterBabyConfig : IEntityConfig, IHasDlcRestrictions
    {
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
        public GameObject CreatePrefab()
        {
            GameObject gameObject = LeafyOilfloaterConfig.CreateOilfloater(ID, PHO_STRINGS.VARIANT_LEAFY.BABY.NAME, PHO_STRINGS.VARIANT_LEAFY.BABY.DESC, kanim_id, true);
            EntityTemplates.ExtendEntityToBeingABaby(gameObject, LeafyOilfloaterConfig.ID);
            return gameObject;
        }

        public void OnPrefabInit(GameObject prefab)
        {
        }

        public void OnSpawn(GameObject inst)
        {
        }

        public const string ID = LeafyOilfloaterConfig.ID + "Baby";
        private const string kanim_id = "custom_baby_oilfloater2_kanim";
    }
}