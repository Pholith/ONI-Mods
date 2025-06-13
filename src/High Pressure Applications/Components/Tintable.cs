using UnityEngine;
using System.Collections.Generic;

namespace High_Pressure_Applications.Components
{
    public class Tintable : KMonoBehaviour
    {
        [MyCmpGet]
        private KAnimControllerBase controller;
        public Color32 TintColour
        {
            get;
            private set;
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();
            InitTintColour();
            SetTint();
        }
        public void SetTint()
        {
            controller.TintColour = TintColour;
        }

        private void InitTintColour()
        {
            string id = GetComponent<Building>()?.Def.PrefabID;
            if (TintTable.ContainsKey(id))
                TintColour = TintTable[id];
            else
                TintColour = new Color32(255, 255, 255, 255);
        }

        private static Dictionary<string, Color32> TintTable = new Dictionary<string, Color32>()
        {
        };
        public static bool AddToTintTable(string id, Color32 color)
        {
            if (!TintTable.ContainsKey(id))
            {
                TintTable.Add(id, color);
                return true;
            }
            return false;
        }
    }
}
