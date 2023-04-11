using CustomizeYourPaints;
using Pholib;
using System.Reflection;
using UnityEngine;

namespace CustomizeYourPaints.Art
{
    // This class makes it so that removing the mod won't soft lock the save file.
    // It switches out the artable currentStage id to some vanilla ID just before saving, and then reverts it once saving is complete.
    public class ArtOverrideRestorer : KMonoBehaviour
    {
        [SerializeField]
        public string fallback;

        [MyCmpReq]
        private Artable artable;

        [MyCmpReq]
        private ArtOverride artOverride;

        private FieldInfo f_currentStage;

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            f_currentStage = typeof(Artable).GetField("currentStage", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();
            Subscribe((int)GameHashes.SaveGameReady, OnLoadGame);
        }

        private void OnLoadGame(object obj)
        {
            Restore();
        }

        public void Restore()
        {
            if (!artOverride.overrideStage.IsNullOrWhiteSpace() && artable != null)
            {
                f_currentStage.SetValue(artable, artOverride.overrideStage);
                Logs.Log("ON OnLoadGame " + artable.CurrentStage);
            }
        }

        public void OnSaveGame()
        {
            Logs.Log("save game: " + artable.CurrentStage + " to " + fallback);
            if (!artOverride.overrideStage.IsNullOrWhiteSpace() && artable != null)
            {
                f_currentStage.SetValue(artable, fallback);
            }
        }
    }
}