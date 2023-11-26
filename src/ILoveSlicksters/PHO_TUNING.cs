using System;

namespace ILoveSlicksters
{
    public class PHO_TUNING
    {
        public static class OILFLOATER
        {
            public static class KG_ORE_EATEN_PER_CYCLE
            {
                public static float LOW = 10f;
                public static float ORIGINAL = 20f;
                public static float HIGH = 40f;
                public static float HIGH2 = 60f;
                public static float MEGA = 100f;

            }
            public static float STANDARD_CALORIES_PER_CYCLE = 120000f;
        }
        public static class EGG_MODIFIER_PER_SECOND
        {
            private static float EGG_CONVERTER = 600 * 100;
            public static float SLOW = 2f / EGG_CONVERTER;
            public static float NORMAL = 5f / EGG_CONVERTER;
            public static float FAST = 10f / EGG_CONVERTER;
            public static float FAST2 = 20f / EGG_CONVERTER;
            public static float FAST3 = 40f / EGG_CONVERTER;
        }

        public static class CONVERSION_EFFICIENCY
        {
            public static float NORMAL_LOW = 0.4f;

            public static float NORMAL_HIGH = 0.6f;
            
            public static float HIGH = 0.8f;

        }
        // adapted from temperaturemodifier
        // Idea: set a minimum light needed 
        public static System.Action CreateLightModifier(string id, Tag eggTag, float modifierPerSecond, bool alsoInvert)
        {
            return delegate ()
            {
                string name = PHO_STRINGS.FERTILITY_MODIFIERS.LIGHT.NAME;
                Db.Get().CreateFertilityModifier(id, eggTag, name, "description test", (string src) => string.Format(
                    PHO_STRINGS.FERTILITY_MODIFIERS.LIGHT.DESC),
                        delegate (FertilityMonitor.Instance inst, Tag eggType)
                        {

                            LightVulnerable component = inst.master.GetComponent<LightVulnerable>();
                            if (component != null)
                            {

                                component.OnLight += delegate (float dt)
                                {

                                    // get light intensity on the creature
                                    int lux = Grid.LightIntensity[Grid.PosToCell(inst.transform.position)];
                                    if (lux > 0)
                                    {
                                        inst.AddBreedingChance(eggType, dt * modifierPerSecond);
                                    }
                                    else if (alsoInvert)
                                    {
                                        inst.AddBreedingChance(eggType, dt * -modifierPerSecond);
                                    }
                                };
                            }
                            else
                            {
                                DebugUtil.LogErrorArgs(new object[]
                                {
                                    "Ack! Trying to add light modifier",
                                    id,
                                    "to",
                                    inst.master.name,
                                    "but it's not light vulnerable!"
                                });
                            }
                        });
            };
        }

        public static System.Action CreatePressureModifier(string id, Tag eggTag, float maxPressure, float modifierPerSecond, bool alsoInvert)
        {
            return delegate ()
            {
                string name = PHO_STRINGS.FERTILITY_MODIFIERS.PRESSURE.NAME;
                Db.Get().CreateFertilityModifier(id, eggTag, name, "description test", (string src) => string.Format(
                    PHO_STRINGS.FERTILITY_MODIFIERS.PRESSURE.DESC, maxPressure),
                        delegate (FertilityMonitor.Instance inst, Tag eggType)
                        {

                            SimplePressureVulnerable component = inst.master.GetComponent<SimplePressureVulnerable>();
                            if (component != null)
                            {

                                component.OnLowPressure += delegate (float dt)
                                {

                                    // get light intensity on the creature
                                    float pressure = Grid.Pressure[Grid.PosToCell(inst.transform.position)];
                                    if (pressure < maxPressure / 10)
                                    {
                                        inst.AddBreedingChance(eggType, dt * modifierPerSecond);
                                    }
                                    else if (alsoInvert)
                                    {
                                        inst.AddBreedingChance(eggType, dt * -modifierPerSecond);
                                    }
                                };
                            }
                            else
                            {
                                DebugUtil.LogErrorArgs(new object[]
                                    {
                                    "Ack! Trying to add pressure modifier",
                                    id,
                                    "to",
                                    inst.master.name,
                                    "but it's not pressure vulnerable!"
                        });
                            }
                        });
            };
        }

        public static System.Action CreateElementModifier(string id, Tag eggTag, SimHashes element, float modifierPerSecond, bool alsoInvert)
        {
            return delegate ()
            {
                string name = PHO_STRINGS.FERTILITY_MODIFIERS.ELEMENT.NAME;
                Db.Get().CreateFertilityModifier(id, eggTag, name, "description test", (string src) => string.Format(
                    PHO_STRINGS.FERTILITY_MODIFIERS.ELEMENT.DESC, element),
                        delegate (FertilityMonitor.Instance inst, Tag eggType)
                        {

                            ElementVulnerable component = inst.master.GetComponent<ElementVulnerable>();
                            if (component != null)
                            {

                                component.InElement += delegate (float dt)
                                {

                                    // get light intensity on the creature
                                    Element pressure = Grid.Element[Grid.PosToCell(inst.transform.position)];
                                    if (pressure.substance.elementID == element)
                                    {
                                        inst.AddBreedingChance(eggType, dt * modifierPerSecond);
                                    }
                                    else if (alsoInvert)
                                    {
                                        inst.AddBreedingChance(eggType, dt * -modifierPerSecond);
                                    }
                                };
                            }
                            else
                            {
                                DebugUtil.LogErrorArgs(new object[]
                                    {
                                    "Ack! Trying to add element modifier",
                                    id,
                                    "to",
                                    inst.master.name,
                                    "but it's not element vulnerable!"
                        });
                            }
                        });
            };
        }
    }

    //adapted from temperature vulnerable
    public class LightVulnerable : StateMachineComponent<TemperatureVulnerable.StatesInstance>, ISim1000ms
    {
        public event Action<float> OnLight;

        public void Sim1000ms(float dt)
        {
            int cell = Grid.PosToCell(gameObject);

            if (!Grid.IsValidCell(cell))
            {
                return;
            }
            OnLight?.Invoke(dt);
        }
    }
    public class SimplePressureVulnerable : StateMachineComponent<TemperatureVulnerable.StatesInstance>, ISim1000ms
    {
        public event Action<float> OnLowPressure;

        public void Sim1000ms(float dt)
        {
            int cell = Grid.PosToCell(gameObject);

            if (!Grid.IsValidCell(cell))
            {
                return;
            }
            OnLowPressure?.Invoke(dt);
        }
    }
    public class ElementVulnerable : StateMachineComponent<TemperatureVulnerable.StatesInstance>, ISim1000ms
    {
        public event Action<float> InElement;

        public void Sim1000ms(float dt)
        {
            int cell = Grid.PosToCell(gameObject);

            if (!Grid.IsValidCell(cell))
            {
                return;
            }
            InElement?.Invoke(dt);
        }
    }
}