using System;

namespace Ergonaut.Core.Utilities
{
    /// <summary>
    /// Provides logging functionality for the Ergonaut framework.
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// Logs an informational message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void LogInfo(string message)
        {
            LogMessage("INFO", message);
        }

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void LogWarning(string message)
        {
            LogMessage("WARN", message);
        }

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void LogError(string message)
        {
            LogMessage("ERROR", message);
        }

        /// <summary>
        /// Logs an error message with exception details.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">The exception to include in the log.</param>
        public static void LogError(string message, Exception exception)
        {
            LogMessage("ERROR", $"{message}: {exception.Message}");
        }

        /// <summary>
        /// Logs a debug message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void LogDebug(string message)
        {
            LogMessage("DEBUG", message);
        }

        /// <summary>
        /// Internal method to format and output log messages.
        /// </summary>
        /// <param name="level">The log level.</param>
        /// <param name="message">The message to log.</param>
        private static void LogMessage(string level, string message)
        {
            var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            var formattedMessage = $"[{timestamp}] [{level}] {message}";
            
            Console.WriteLine(formattedMessage);
        }
    }
}