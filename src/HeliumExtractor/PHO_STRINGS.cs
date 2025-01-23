using Pholib;
using STRINGS;

namespace HeliumExtractor
{
    public class PHO_STRINGS
    {
        public static class HELIUMEXTRACTOR
        {
            public static LocString NAME = "Helium Extractor";
            public static LocString DESC = $"{"Helium".FormatColored("9C2724")} has the lowest liquefaction temperature of all elements and the highest {UI.FormatAsKeyWord("thermal conductivity")} of gases.";
            public static LocString EFFECT = $"Decompose {UI.FormatAsLink("Natural Gas", "METHANE")} into {UI.FormatAsLink("Helium", "HELIUM")}, {UI.FormatAsLink("Propane", "PROPANE")} and {UI.FormatAsLink("Sulfur", "SULFUR")}."
                + "\n\n" +
                $"{"Helium".FormatColored("9C2724")} is a useful gas with interesting physical properties.\n\n{"Propane".FormatColored("7990D1")} can be used the same way than {"Natural gas".FormatColored("FF6E0F")}, in a {UI.FormatAsLink("Gourmet Cooking Station", GourmetCookingStationConfig.ID)}, {UI.FormatAsLink("Natural Gas Generator", MethaneGeneratorConfig.ID)} or in a {UI.FormatAsLink("Food Dehydrator", FoodDehydratorConfig.ID)}.";
        }
        public static LocString COMBUSTIBLEGAS = "Combustible Gas";

    }
}
