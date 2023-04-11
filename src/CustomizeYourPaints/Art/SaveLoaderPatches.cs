using HarmonyLib;
using System;

namespace CustomizeYourPaints.Art
{
    public class SaveLoaderPatch
    {
        public SaveLoaderPatch()
        {
        }

        [HarmonyPatch(typeof(SaveLoader), "Save", new Type[]
        {
            typeof(string),
            typeof(bool),
            typeof(bool)
        })]
        public class SaveLoader_Save_Patch
        {
            public static void Prefix()
            {
                foreach (object obj in CustomizeYourPaints.artRestorers)
                {
                    ArtOverrideRestorer artOverrideRestorer = (ArtOverrideRestorer)obj;
                    artOverrideRestorer.OnSaveGame();
                }
            }

            public static void Postfix()
            {
                foreach (object obj in CustomizeYourPaints.artRestorers)
                {
                    ArtOverrideRestorer artOverrideRestorer = (ArtOverrideRestorer)obj;
                    artOverrideRestorer.Restore();
                }
            }

            public SaveLoader_Save_Patch()
            {
            }
        }
    }
}
