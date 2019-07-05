using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Reflection;

namespace Pholib
{
    public class JsonReader
    {

        private static string fileName = "config.json";

        private JObject configValues;

        public JsonReader()
        {
            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var configPath = Path.Combine(directory, fileName);
            try
            {
                using (StreamReader r = new StreamReader(configPath))
                {
                    configValues = JObject.Parse(r.ReadToEnd());
                }
            }
            catch (Exception)
            {
                Logs.Log("Error during json reading.");
            }
        }

        // Write the Json 
        private void WriteJson()
        {
            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var configPath = Path.Combine(directory, fileName);
            try
            {
                using (StreamWriter r = new StreamWriter(configPath))
                {
                    r.Write(configValues.ToString());
                }
            }
            catch (Exception)
            {
                Logs.Log("Error during json writing.");
            }
        }
        // Get the property of the Json config of this mod
        public T GetProperty<T>(string propertyName)
        {
            try
            {
                JToken token = configValues.GetValue(propertyName);
                return token.ToObject<T>();
            }
            catch (Exception)
            {
                Logs.Log("Error during json reading. Property not found. Try to default value.");
                return default;
            }
        }

        /// <summary>
        /// Get the property of the Json config of this mod
        /// This method should be used instead of GetProperty<>(string) for more stability.
        /// </summary>
        /// <typeparam name="T"> Type of the json propertu </typeparam>
        /// <param name="propertyName"> name of the json field </param>
        /// <param name="defaultValue"> value to return and write if json not found </param>
        /// <returns></returns>
        public T GetProperty<T>(string propertyName, T defaultValue)
        {
            try
            {
                JToken token = configValues.GetValue(propertyName);
                return token.ToObject<T>();
            }
            catch (FormatException e)
            {
                Logs.Log(e.GetType() + " in GetProperty. Returning default value");
            }
            catch (NullReferenceException e)
            {
                Logs.Log(e.GetType() + " in GetProperty. Returning default value and save in config");
                configValues.Add(propertyName, JToken.FromObject(defaultValue));
                WriteJson();
            }
            catch (Exception e)
            {
                Logs.Log(e.GetType() + " in GetProperty. Returning default value and save in config");
            }
            return defaultValue;

        }
    }
}
