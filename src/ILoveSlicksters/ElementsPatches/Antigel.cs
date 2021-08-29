using ILoveSlicksters.elements;
using UnityEngine;

namespace ILoveSlicksters
{
    public class Antigel
    {

        public const string Id = "Antigel";
        public static readonly SimHashes SimHash = (SimHashes)Hash.SDBMLower(Id);
        public static string Name = PHO_STRINGS.ELEMENTS.ANTIGEL.NAME;
        public static string Description = PHO_STRINGS.ELEMENTS.ANTIGEL.DESC;
        public static Color32 color = new Color32(190, 220, 160, 255);

        public static Substance CreateSubstance(Substance source)
        {
            return ModUtil.CreateSubstance(
              name: Id,
              state: Element.State.Liquid,
              kanim: source.anim,
              material: source.material,
              colour: color,
              ui_colour: color,
              conduit_colour: color
            );
        }

        public static void RegisterSubstance()
        {

            SubstanceUtils.CreateRegisteredSubstance(
              name: Id,
              state: Element.State.Liquid,
              kanim: SubstanceUtils.FindAnim("liquid_tank_kanim"),
              material: Assets.instance.substanceTable.liquidMaterial,
              colour: color
            );
        }

    }
}
