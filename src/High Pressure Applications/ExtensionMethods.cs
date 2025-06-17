using HarmonyLib;
using System.Reflection;

namespace High_Pressure_Applications
{
    public static class ExtensionMethods
    {
        private static readonly FieldInfo maxMass = AccessTools.Field(typeof(ConduitFlow), "MaxMass");

        public static float MaxMass(this ConduitFlow manager)
        {
            if (manager == null)
                throw new System.ArgumentNullException("manager");
            return (float)maxMass.GetValue(manager);
        }
    }
}
