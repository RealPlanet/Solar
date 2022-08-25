using System;
using System.Collections.Generic;

namespace SolarXUtility.Console.Logging
{
    public sealed class ConsoleLogger
    {
        private static Dictionary<int, ConsoleLogger> m_Loggers = null;
        private readonly string m_LogName;

        public ConsoleLogger(string name)
        {
            m_LogName = name;
            if (m_Loggers is null)
                m_Loggers = new Dictionary<int, ConsoleLogger>(2);

            m_Loggers.Add(m_LogName.GetHashCode(), this);
        }

        public void Log(string message) => LogLineImpl(message, ConsoleColor.DarkGreen);

        public void LogWarning(string message) => LogLineImpl(message, ConsoleColor.DarkYellow);

        public void LogError(string message) => LogLineImpl(message, ConsoleColor.DarkRed);

        public static ConsoleLogger Get(string loggerName)
        {
            int hash = loggerName.GetHashCode();
            if (m_Loggers.ContainsKey(hash))
                return m_Loggers[hash];

            return null;
        }

        private void LogLineImpl(string message, ConsoleColor color)
        {
            System.Console.ForegroundColor = color;
            System.Console.WriteLine($"[[ - {m_LogName} - ]] :: {message}");
            System.Console.ResetColor();
        }

        private void LogImpl(string message, ConsoleColor color)
        {
            System.Console.ForegroundColor = color;
            System.Console.WriteLine($"[[ - {m_LogName} - ]] :: {message}");
            System.Console.ResetColor();
        }
    }
}
