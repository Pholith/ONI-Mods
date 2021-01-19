﻿using UnityEngine;

namespace ILoveSlicksters
{
    public class EthanolOilfloaterBabyConfig : IEntityConfig
    {
        public GameObject CreatePrefab()
        {
            GameObject gameObject = EthanolOilfloaterConfig.CreateOilfloater(ID, PHO_STRINGS.VARIANT_ETHANOL.BABY.NAME, PHO_STRINGS.VARIANT_ETHANOL.BABY.DESC, "custom_baby_oilfloater_kanim", true);
            EntityTemplates.ExtendEntityToBeingABaby(gameObject, "EthanolOilfloater", null);
            return gameObject;
        }

        public void OnPrefabInit(GameObject prefab)
        {
        }

        public void OnSpawn(GameObject inst)
        {
        }
        public string GetDlcId()
        {
            return "";
        }

        public const string ID = "EthanolOilfloaterBaby";

    }
}
