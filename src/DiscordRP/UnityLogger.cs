using DiscordRPC.Logging;
using Pholib;

namespace DiscordRPMod
{
    public class UnityLogger : ILogger
    {
        public UnityLogger(LogLevel level = LogLevel.Warning)
        {
            Level = level;
        }

        public LogLevel Level { get; set; }

        public void Error(string message, params object[] args)
        {
            if (Level <= LogLevel.Error)
                Logs.LogFormat(message, args);
        }

        public void Info(string message, params object[] args)
        {
            if (Level <= LogLevel.Info)
                Logs.LogFormat(message, args);
        }

        public void Trace(string message, params object[] args)
        {
            if (Level <= LogLevel.Trace)
                Logs.LogFormat(message, args);
        }

        public void Warning(string message, params object[] args)
        {
            if (Level <= LogLevel.Warning)
                Logs.LogFormat(message, args);
        }
    }
}
