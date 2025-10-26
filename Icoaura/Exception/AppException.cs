using System;
using System.Runtime.CompilerServices;
using Icoaura.Context;
using Icoaura.Enum;

namespace Icoaura.Exception
{
    public class AppException : System.Exception
    {
        public bool IsOperational { get; }

        public string TraceId { get; } = TraceContext.TraceId;

        public string UserMessage { get; } = "An unexpected error occurred.";

        public string LogMessage { get; } = string.Empty;

        public DateTime TimestampUtc { get; } = DateTime.UtcNow;

        public LogLevel LogLevel { get; } = LogLevel.Error;

        /// The method or function name where the exception originated.
        public string SourceName { get; } = string.Empty;

        // --- Constructors ---

        public AppException() { }

        public AppException(
            string userMessage,
            bool isOperational = true,
            LogLevel logLevel = LogLevel.Error,
            [CallerMemberName] string sourceName = "")
            : base(userMessage)
        {
            UserMessage = userMessage;
            LogMessage = userMessage;
            IsOperational = isOperational;
            LogLevel = logLevel;
            SourceName = sourceName;
        }

        public AppException(
            string userMessage,
            string logMessage,
            bool isOperational,
            LogLevel logLevel = LogLevel.Error,
            [CallerMemberName] string sourceName = "")
            : base(userMessage)
        {
            UserMessage = userMessage;
            LogMessage = logMessage;
            IsOperational = isOperational;
            LogLevel = logLevel;
            SourceName = sourceName;
        }

        public AppException(
            string userMessage,
            System.Exception innerException,
            bool isOperational = false,
            LogLevel logLevel = LogLevel.Fatal,
            [CallerMemberName] string sourceName = "")
            : base(userMessage, innerException)
        {
            UserMessage = userMessage;
            LogMessage = innerException.Message;
            IsOperational = isOperational;
            LogLevel = logLevel;
            SourceName = sourceName;
        }

        // --- Static factory methods ---

        public static AppException Operational(
            string userMessage,
            string? logMessage = null,
            LogLevel level = LogLevel.Error,
            [CallerMemberName] string sourceName = "")
            => new(userMessage, logMessage ?? userMessage, true, level, sourceName);

        public static AppException Critical(
            string logMessage,
            System.Exception? inner = null,
            LogLevel level = LogLevel.Fatal,
            [CallerMemberName] string sourceName = "")
            => new(logMessage, inner ?? new System.Exception(logMessage), false, level, sourceName);

        public override string ToString()
        {
            return $"[{TimestampUtc:u}] TraceId={TraceId}, " +
                   $"Level={LogLevel}, IsOperational={IsOperational}, " +
                   $"Source={SourceName}, " +
                   $"UserMessage='{UserMessage}', LogMessage='{LogMessage}'";
        }
    }
}
