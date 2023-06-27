using System;
using System.Diagnostics;
using System.IO;

namespace CandyCrush.Core.Services
{
    public enum LogLevel
    {
        Info,
        Warning,
        Error
    }

    public static class Logger
    {
        private static string logFilePath = "./log.txt";

        public static void Initialize(string filePath)
        {
            logFilePath = filePath;
        }

        public static void Log(LogLevel level, string message)
        {
            if (string.IsNullOrEmpty(logFilePath))
            {
                throw new InvalidOperationException("Logger has not been initialized. Call Initialize method before using the logger.");
            }

            string logEntry = $"{DateTime.Now} [{level.ToString().ToUpper()}] {message}";

            StackTrace stackTrace = new();
            StackFrame frame = stackTrace.GetFrame(1);
            string caller = $"{frame.GetMethod().DeclaringType}.{frame.GetMethod().Name}";

            logEntry += $" (Caller: {caller})";

            using (StreamWriter writer = new(logFilePath, true))
            {
                writer.WriteLine(logEntry);
            }

            Console.WriteLine(logEntry);
        }
    }

}
