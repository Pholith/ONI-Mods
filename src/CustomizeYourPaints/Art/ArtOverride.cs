using KSerialization;
using Pholib;
using System.Collections.Generic;
using UnityEngine;

namespace CustomizeYourPaints.Art
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class ArtOverride : KMonoBehaviour
    {

        public bool IsCustomPaintStage(string id)
        {
            return id != null && id.Contains(CustomizeYourPaints.CUSTOM_PAINT_ID);
        }

        public void UpdateOverride(string newId)
        {
            Logs.Log("UpdateOverride " + overrideStage + " into " + newId);
            overrideStage = IsCustomPaintStage(newId) ? newId : null;
        }

        [Serialize]
        public string overrideStage;

        [SerializeField]
        public List<string> customExtraStages;

    }
}
