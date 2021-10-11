/* Copyright (c) 2021 ExT (V.Sigalkin) */

using StolenNetwork.Logging;

namespace StolenNetwork
{
    public static class Logs
    {
        #region Public Vars

        public static LogLevel LogLevel = LogLevel.None;

        public static ILogger Logger = new ConsoleLogger();

        #endregion

        #region Public Methods

        public static void Debug(string message)
        {
            if (Logger == null || LogLevel > LogLevel.Debug)
                return;

            Logger.Debug(message);
        }

        public static void Info(string message)
        {
            if (Logger == null || LogLevel > LogLevel.Info)
                return;

            Logger.Info(message);
        }

        public static void Warning(string message)
        {
            if (Logger == null || LogLevel > LogLevel.Warning)
                return;

            Logger.Warning(message);
        }

        #endregion
    }
}
