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
            Logs.Log("IsCustomStage " + id + " " + (id != null && CustomizeYourPaints.myOverrides.Contains(id)));
            return id != null && CustomizeYourPaints.myOverrides.Contains(id);
        }

        public void UpdateOverride(string newId)
        {
            overrideStage = IsCustomPaintStage(newId) ? newId : null;
            Logs.Log("UpdateOverride " + overrideStage);
        }

        [Serialize]
        public string overrideStage;

        [SerializeField]
        public List<string> customExtraStages;

    }
}
