using UnityEngine;

namespace ILoveSlicksters
{
    public class Antigel
    {


        public const string Data = @"elements:
  - elementId: Antigel
    maxMass: 1000
    liquidCompression: 1.01
    speed: 125
    minHorizontalFlow: 0.1
    minVerticalFlow: 0.1
    specificHeatCapacity: 5.179
    thermalConductivity: 0.8
    solidSurfaceAreaMultiplier: 1
    liquidSurfaceAreaMultiplier: 25
    gasSurfaceAreaMultiplier: 1
    lowTemp: 188.15
    highTemp: 480.15
    lowTempTransitionTarget: DirtyIce
    highTempTransitionTarget: Steam
    defaultTemperature: 312.5
    defaultMass: 1000
    molarMass: 30
    toxicity: 0.5
    lightAbsorptionFactor: 0.3
    radiationAbsorptionFactor: 0.35
    tags:    
    - AnyWater
    - Mixture
    isDisabled: false
    state: Liquid
    localizationID: ILoveSlicksters.PHO_STRINGS.ELEMENTS.ANTIGEL.NAME
    dlcId: """"
";


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
    }
}
