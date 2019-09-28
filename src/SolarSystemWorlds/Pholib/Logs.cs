namespace Pholib
{
    public class Logs
    {
        private static readonly string version = "1.1.0";

        public static bool DebugLog = false;
        private static bool initiated = false;

        public static void InitIfNot()
        {
            if (initiated)
            {
                return;
            }
            Debug.Log("== Game Launched with Pholib " + version + "  " + System.DateTime.Now);
            initiated = true;
        }


        public static void Log(string informations)
        {
            InitIfNot();
            Debug.Log("Pholib: " + informations);
        }

        public static void LogIfDebugging(string informations)
        {
            InitIfNot();
            Debug.Log("Pholib: " + informations);
        }
    }
}
