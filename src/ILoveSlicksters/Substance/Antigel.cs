using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ILoveSlicksters
{
    class Antigel
    {
        public const string Data = @"elements:
  - elementId: Antigel
    maxMass: 1000
    liquidCompression: 1.01
    speed: 110
    minHorizontalFlow: 0.1
    minVerticalFlow: 0.1
    specificHeatCapacity: 4.179
    thermalConductivity: 0.8
    solidSurfaceAreaMultiplier: 1
    liquidSurfaceAreaMultiplier: 25
    gasSurfaceAreaMultiplier: 1
    lowTemp: 208.15
    highTemp: 470.15
    lowTempTransitionTarget: DirtyIce
    highTempTransitionTarget: Steam
    defaultTemperature: 312.5
    defaultMass: 1000
    molarMass: 30
    toxicity: 0.5
    lightAbsorptionFactor: 0.3
    isDisabled: false
    state: Liquid
    localizationID: STRINGS.ELEMENTS.ANTIGEL.NAME
";


        public const string Id = "Antigel";
        public static readonly SimHashes SimHash = (SimHashes)Hash.SDBMLower(Id);
        public static string Name = UI.FormatAsLink("Antigel", Id.ToUpper());
        public static string Description = $"A mixture of water(H<sub>2</sub>O) and Ethylen Glycol (C<sub>2</sub>H<sub>6</sub>O<sub>2</sub>).\n\nIt has been designed by engineers to be a good heat transfer fluid that does not freeze or vaporize easily.";
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
