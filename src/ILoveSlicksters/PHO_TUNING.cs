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

            }
        }
        public static class EGG_MODIFIER_PER_SECOND
        {
            private static float EGG_CONVERTER = 600 * 100;
            public static float SLOW = 2f / EGG_CONVERTER;
            public static float NORMAL = 5f / EGG_CONVERTER;
            public static float FAST = 10f / EGG_CONVERTER;
        }


        // adapted from temperaturemodifier
        // Idea: set a minimum light needed 
        public static System.Action CreateLightModifier(string id, Tag eggTag, float modifierPerSecond, bool alsoInvert)
        {
            return delegate ()
            {
                string name = StringsPatch.FERTILITY_MODIFIERS.LIGHT.NAME;
                Db.Get().CreateFertilityModifier(id, eggTag, name, "description test", (string src) => string.Format(
                    StringsPatch.FERTILITY_MODIFIERS.LIGHT.DESC),
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
}