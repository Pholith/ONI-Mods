using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Database;
using PeterHan.PLib.Options;
using ProcGenGame;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EmptyWorlds
{

    public class EmptyWorlds : UserMod2
    {
        public static EmptyWorldsOptions Settings { get; private set; }

        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            new POptions().RegisterOptions(this, typeof(EmptyWorldsOptions));

            // Init PLib and settings
            PUtil.InitLibrary();

            Settings = POptions.ReadSettings<EmptyWorldsOptions>();
            if (Settings == null)
            {
                Settings = new EmptyWorldsOptions();
            }
            new PLocalization().Register();
            Pholib.Utilities.GenerateStringsTemplate(typeof(PHO_STRINGS));
        }
    }

    [HarmonyPatch(typeof(TemplateCache), "GetTemplate")]
    public class TemplateCache_GetTemplate
    {
        public static void Postfix(ref TemplateContainer __result)
        {
            if (EmptyWorlds.Settings.RemoveBunkerAtWorldgen)
            {
                if (__result.name == "poi_bunker_skyblock")
                {
                    __result = new TemplateContainer();
                    List<TemplateClasses.Cell> list = new List<TemplateClasses.Cell>
                    {
                        new TemplateClasses.Cell(0, 0, SimHashes.Vacuum, 1, 0, "", 0)
                    };
                    __result.Init(list, new List<TemplateClasses.Prefab>(), new List<TemplateClasses.Prefab>(), new List<TemplateClasses.Prefab>(), new List<TemplateClasses.Prefab>());
                }
            }
        }
    }

    [HarmonyPatch(typeof(WorldGen), "DrawWorldBorder")]
    public class WorldGen_DrawWorldBorder
    {
        private static void Prefix(WorldGen __instance, Sim.Cell[] cells, Chunk world, SeededRandom rnd, ref HashSet<int> borderCells, ref List<RectInt> poiBounds, WorldGen.OfflineCallbackFunction updateProgressFn)
        {
            bool boolSetting = __instance.Settings.GetBoolSetting("DrawWorldBorderForce");
            int intSetting = __instance.Settings.GetIntSetting("WorldBorderThickness");
            int intSetting2 = __instance.Settings.GetIntSetting("WorldBorderRange");

            byte new_elem_idx = (byte)ElementLoader.elements.IndexOf(WorldGen.unobtaniumElement);
            float temperature = WorldGen.unobtaniumElement.defaultValues.temperature;
            float mass = WorldGen.unobtaniumElement.defaultValues.mass;
            try
            {
                for (int l = 0; l < world.size.x; l++)
                {
                    updateProgressFn(UI.WORLDGEN.DRAWWORLDBORDER.key, l / world.size.x * 0.66f + 0.33f, WorldGenProgressStages.Stages.DrawWorldBorder);
                    int num9 = Mathf.Max(-intSetting2, Mathf.Min(rnd.RandomRange(-2, 2), intSetting2));
                    for (int m = 0; m < intSetting + num9; m++)
                    {
                        int num13 = Grid.XYToCell(l, m);
                        if (boolSetting)
                        {
                            borderCells.Add(num13);
                            cells[num13].SetValues(new_elem_idx, temperature, mass);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Debug.LogWarning(ex.Message);
            }
            
        }
    }
}