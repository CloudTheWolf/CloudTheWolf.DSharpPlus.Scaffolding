using Microsoft.Extensions.Logging;
using System;

namespace CloudTheWolf.DSharpPlus.Scaffolding.Logging
{
    /// <summary>
    /// Logger for CloudTheWolf.DSharpPlus.Scaffolding 
    /// </summary>
    public class Logger
    {
        /// <summary>
        /// Logger Factory for CloudTheWolf.DSharpPlus.Scaffolding
        /// </summary>
        public static ILoggerFactory LoggerFactory;

        /// <summary>
        /// ILogger for CloudTheWolf.DSharpPlus.Scaffolding
        /// </summary>
        public static ILogger<Logger> ConsoleLogger;

        /// <summary>
        /// Create a new ILogger for CloudTheWolf.DSharpPlus.Scaffolding
        /// </summary>
        /// <param name="logger"></param>
        public Logger(ILogger<Logger> logger)
        {
            ConsoleLogger = logger;
        }

        /// <summary>
        /// Log a message as Information
        /// </summary>
        /// <param name="message"></param>
        public static void LogInfo(string message)
        {
            ConsoleLogger.LogInformation(message);
        }

        /// <summary>
        /// Log a message as Warning
        /// </summary>
        /// <param name="message"></param>
        public static void LogWarning(string message)
        {
            ConsoleLogger.LogWarning(message);
        }

        /// <summary>
        /// Log a message as Error and pass an <see cref="Exception"/>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void LogError(string message, Exception exception)
        {
            ConsoleLogger.LogError(message, exception);
        }

    }
}
