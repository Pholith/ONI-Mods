using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ILoveSlicksters
{
    class Ethylen_Glycol
    {
        public const string Data = @"elements:
  - elementId: Ethylen_Glycol
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
    toxicity: 0.1
    lightAbsorptionFactor: 0.6
    isDisabled: false
    state: Liquid
    localizationID: STRINGS.ELEMENTS.DIRTYWATER.NAME
";


        public const string Id = "Ethylen_Glycol";
        public static readonly SimHashes SimHash = (SimHashes)Hash.SDBMLower(Id);
        public static string Name = UI.FormatAsLink("Ethylen Glycol", Id.ToUpper());
        public static string Description = $"A mixture of water and Ethylen Glycol.";
        public static Color32 color = new Color32(99, 22, 222, 255);

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
