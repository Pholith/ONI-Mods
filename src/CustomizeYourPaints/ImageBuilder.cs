using Harmony;
using Pholib;
using System;
using System.Collections.Generic;
using System.Text;

namespace SolarSystemWorlds
{
    public class ImageBuilder
    {

        public static void OnLoad()
        {
            ImageUtil.BuildImage();
        }
    }

    [HarmonyPatch(typeof(Db))]
    [HarmonyPatch("Initialize")]
    public class DbPatch
    {
        public static void Postfix()
        {

            Dictionary<HashedString, KAnimFile> table = Traverse.Create<Assets>().Field("AnimTable").GetValue<Dictionary<HashedString, KAnimFile>>();

            table["painting_kanim"] = Assets.GetAnim("mod_painting_kanim");
            table["painting_tall_kanim"] = Assets.GetAnim("mod_painting_tall_kanim");
            table["painting_wide_kanim"] = Assets.GetAnim("mod_painting_wide_kanim");

        }
    }
}
