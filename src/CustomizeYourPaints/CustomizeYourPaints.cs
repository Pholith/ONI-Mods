using CustomizeYourPaints.Art;
using Database;
using HarmonyLib;
using KMod;
using Pholib;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static CustomizeYourPaints.CustomizeYourPaints;

namespace CustomizeYourPaints
{
    public class CustomizeYourPaints : UserMod2
    {
        public const string CUSTOM_PAINT_ID = "CustomizeYourPaints";

        public enum CanvasSize
        {
            Normal,
            Tall,
            Wide
        }

        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);

        }

        public static Components.Cmps<ArtOverrideRestorer> artRestorers = new Components.Cmps<ArtOverrideRestorer>();

        public static List<string> myOverrides = new List<string>();

    }



    [HarmonyPatch(typeof(Manager), nameof(Manager.Load))]
    public class ModManager_Load_AnimPatch
    {
        private static readonly string ORIGINALS_PATH = Path.Combine("src", "originals");

        public static void Prefix(Content content)
        {
            if (content != Content.Animation) return;

            byte[] normal_painting_anim = File.ReadAllBytes(Path.Combine(ImageUtil.ModPath(), ORIGINALS_PATH, "painting_art_b_anim.bytes"));
            byte[] normal_painting_build = File.ReadAllBytes(Path.Combine(ImageUtil.ModPath(), ORIGINALS_PATH, "painting_art_b_build.bytes"));

            byte[] tall_painting_anim = File.ReadAllBytes(Path.Combine(ImageUtil.ModPath(), ORIGINALS_PATH, "painting_tall_art_a_anim.bytes"));
            byte[] tall_painting_build = File.ReadAllBytes(Path.Combine(ImageUtil.ModPath(), ORIGINALS_PATH, "painting_tall_art_a_build.bytes"));

            byte[] wide_painting_anim = File.ReadAllBytes(Path.Combine(ImageUtil.ModPath(), ORIGINALS_PATH, "painting_wide_art_a_anim.bytes"));
            byte[] wide_painting_build = File.ReadAllBytes(Path.Combine(ImageUtil.ModPath(), ORIGINALS_PATH, "painting_wide_art_a_build.bytes"));

            ArtableStages_Constructor_Patch.IdsToAdds = new List<Tuple<string, CanvasSize>>();

            int counter = 0;
            foreach (string filePath in Directory.EnumerateFiles(Path.Combine(ImageUtil.ModPath(), "src")))
            {
                counter++;
                string[] splitedFile = Path.GetFileNameWithoutExtension(filePath).Split('_','-');
                string prefix = splitedFile[0].ToLower();
                string suffix = counter + splitedFile[1];
                CanvasSize canvasSize = CanvasSize.Normal;
                if (prefix.Contains("wide")) canvasSize = CanvasSize.Wide;
                if (prefix.Contains("tall")) canvasSize = CanvasSize.Tall;

                var anim = new KAnimFile.Mod
                {
                    textures = new List<Texture2D>()
                };

                string artToReplace = "painting_art_b_0.png";
                Vector2Int bigImageToReplace = new Vector2Int(120, 136);
                Vector2Int littleImageToReplace = new Vector2Int(75, 86);
                Vector2Int bigImagePosition = new Vector2Int(547, 555);
                Vector2Int littleImagePosition = new Vector2Int(556, 713);
                switch (canvasSize)
                {
                    case CanvasSize.Normal:
                        anim.anim = normal_painting_anim;
                        anim.build = normal_painting_build;
                        break;
                    case CanvasSize.Tall:
                        anim.anim = tall_painting_anim;
                        anim.build = tall_painting_build;
                        artToReplace = "painting_tall_art_a_0.png";
                        bigImageToReplace = new Vector2Int(130, 215);
                        littleImageToReplace = new Vector2Int(55, 95);
                        bigImagePosition = new Vector2Int(588, 11);
                        littleImagePosition = new Vector2Int(430, 296);
                        break;
                    case CanvasSize.Wide:
                        anim.anim = wide_painting_anim;
                        anim.build = wide_painting_build;
                        artToReplace = "painting_wide_art_a_0.png";
                        bigImageToReplace = new Vector2Int(213, 128);
                        littleImageToReplace = new Vector2Int(112, 68);
                        bigImagePosition = new Vector2Int(812, 1);
                        littleImagePosition = new Vector2Int(830, 154);
                        break;
                }
                Texture2D textureToReplace = ImageUtil.LoadPNG(filePath);
                Texture2D normalPainting = ImageUtil.LoadPNG(Path.Combine(ImageUtil.ModPath(), ORIGINALS_PATH, artToReplace));
                Texture2D normalPainting2 = ImageUtil.MergeImage(normalPainting, textureToReplace.ScaleTexture(bigImageToReplace.x, bigImageToReplace.y), bigImagePosition.x, bigImagePosition.y);
                normalPainting2 = ImageUtil.MergeImage(normalPainting2, textureToReplace.ScaleTexture(littleImageToReplace.x, littleImageToReplace.y), littleImagePosition.x, littleImagePosition.y);

                anim.textures.Add(normalPainting2);

                //File.WriteAllBytes(Path.Combine(ImageUtil.ModPath(), ORIGINALS_PATH, $"image{suffix}.png"), normalPainting2.EncodeToPNG());

                string kanimId = $"{CUSTOM_PAINT_ID}_{suffix}_kanim";
                //Logs.Log("Adding " + kanimId);
                ModUtil.AddKAnimMod(kanimId, anim);
                ArtableStages_Constructor_Patch.IdsToAdds.Add(new Tuple<string, CanvasSize>(kanimId, canvasSize));
            }
        }
    }
    [HarmonyPatch(typeof(ArtableStages), MethodType.Constructor, typeof(ResourceSet))]
    public class ArtableStages_Constructor_Patch
    {

        public static List<Tuple<string, CanvasSize>> IdsToAdds;

        public static void Postfix(ArtableStages __instance)
        {
            foreach (Tuple<string, CanvasSize> tuple in IdsToAdds)
            {
                Logs.Log("Adding " + tuple.first);
                AddCustomPaint(__instance,
                    tuple.first.Replace("_kanim", "").Replace($"{CUSTOM_PAINT_ID}_", "").Substring(1), 
                    "A custom paint added using the CustomizeYourPaints mod", 
                    tuple.first, tuple.first, tuple.second);
            }

            //__instance.Permits.ArtableStages.Add("id", "CustomizedPaint", "desc", Database.PermitRarity.Universal, "art_custom1_kanim", "art_b", 15, true, "LookingGreat", "Canvas", "canvas");
        }

        private static void AddCustomPaint(ArtableStages __instance, string name, string description, string id, string kanim, CanvasSize canvasSize)
        {
            string targetPrefabId = CanvasConfig.ID;
            int decor = 15;
            switch (canvasSize)
            {
                case CanvasSize.Tall:
                    targetPrefabId = CanvasTallConfig.ID;
                    break;
                case CanvasSize.Wide:
                    targetPrefabId = CanvasWideConfig.ID;
                    break;
            }

            myOverrides.Add(CUSTOM_PAINT_ID + "_" + id);
            __instance.Add(
                id,
                name,
                description,
                PermitRarity.Universal,
                kanim,
                "art_b", // leftover from when these were one merged animation file
                decor,
                true,
                ArtableStatuses.ArtableStatusType.LookingGreat.ToString(),
                targetPrefabId
                );
        }
    }
}
