using HarmonyLib;
using KMod;
using Pholib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace SolarSystemWorlds
{
    public class CustomizeYourPaints : UserMod2
    {

        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            //Traverse.Create(Assets.instance).Method("LoadAnims").GetValue();
        }
    }
    [HarmonyPatch(typeof(KAnimGroupFile), nameof(KAnimGroupFile.MapNamesToAnimFiles))]
    public class ModAnimPatch
    {
        private static readonly string ORIGINALS_PATH = Path.Combine("src", "originals");

        public static void Prefix()
        {

            var anim = new KAnimFile.Mod();

            Logs.Log("test");

            anim.anim = File.ReadAllBytes(Path.Combine(ImageUtil.ModPath(), ORIGINALS_PATH, "painting_art_a_anim.bytes"));
            anim.build = File.ReadAllBytes(Path.Combine(ImageUtil.ModPath(), ORIGINALS_PATH, "painting_art_a_build.bytes"));
            anim.textures = new List<Texture2D>();

            Texture2D normalPainting = ImageUtil.LoadPNG(Path.Combine(ImageUtil.ModPath(), ORIGINALS_PATH, "painting_art_a_0.png"));
            Texture2D normalPainting2 = ImageUtil.MergeImage(normalPainting, ImageUtil.LoadPNG(Path.Combine(ImageUtil.ModPath(), "src", "test.png")).ScaleTexture(118, 118), 178, 144);
            normalPainting2 = ImageUtil.MergeImage(normalPainting, ImageUtil.LoadPNG(Path.Combine(ImageUtil.ModPath(), "src", "test.png")).ScaleTexture(78, 78), 217, 148);
            anim.textures.Add(normalPainting2);

            ModUtil.AddKAnimMod("art_custom1_kanim", anim);

        }
    }
    [HarmonyPatch(typeof(Db), "Initialize")]
    public class DbPatch
    {
        public static void Postfix(Db __instance)
        {
            Logs.Log("test2");
            __instance.Permits.ArtableStages.Add("id", "name", "desc", Database.PermitRarity.Universal, "art_custom1_kanim", "art_b", 15, true, "LookingGreat", "Canvas", "canvas");
            
            /*
            Dictionary<HashedString, KAnimFile> table = Traverse.Create<Assets>().Field("AnimTable").GetValue<Dictionary<HashedString, KAnimFile>>();

            table["painting_kanim"] = Assets.GetAnim("mod_painting_kanim");
            table["painting_tall_kanim"] = Assets.GetAnim("mod_painting_tall_kanim");
            table["painting_wide_kanim"] = Assets.GetAnim("mod_painting_wide_kanim");*/

        }
    }
}
