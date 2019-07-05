using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace Pholib
{
    public class Logs
    {
        private static readonly string fileName = "Pholib_output.txt";
        private static readonly string version = "1.0.0";

        private static bool initiated = false;
        public static void InitIfNot()
        {
            if (initiated)
            {
                return;
            }
            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var logsPath = Path.Combine(directory, fileName);

            if (!File.Exists(logsPath))
            {
                var stream = File.Create(logsPath);
                stream.Close();
            }
            try
            {
                StreamWriter writer = File.AppendText(logsPath);
                writer.WriteLine("== Game Launched with Pholib " + version + "  " + DateTime.Now);
                writer.Close();
            }
            catch (Exception e)
            {
                Debug.Log("Pholib: Error to log" + e.ToString());
            }
            Debug.Log("Pholib: static log.");
            initiated = true;
        }


        public static void Log(string information)
        {
            InitIfNot();
            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var logsPath = Path.Combine(directory, fileName);
            try
            {
                StreamWriter writer = File.AppendText(logsPath);
                writer.WriteLine("Pholib: " + information);
                writer.Close();
            }
            catch (Exception)
            {
                Debug.Log("Pholib: Error during log.");
            }

        }
    }
}
