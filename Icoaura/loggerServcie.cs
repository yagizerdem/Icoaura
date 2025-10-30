using Icoaura.Context;
using Icoaura.Enum;
using System.Diagnostics;
using System.IO;

namespace Icoaura
{
    public class loggerServcie
    {
        public loggerServcie()
        {
            
        }

        public void Log(
            string message,
            string traceId = "-",
            LogLevel logLevel = LogLevel.Info)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string levelName = logLevel.ToString();

            var stackTrace = new StackTrace();
            var frame = stackTrace.GetFrame(1); 
            var method = frame?.GetMethod();
            string methodName = method?.Name ?? "UnknownMethod";
            string className = method?.DeclaringType?.Name ?? "UnknownClass";
            string namespaceName = method?.DeclaringType?.Namespace ?? "UnknownNamespace";

            string callerInfo = $"{namespaceName}.{className}.{methodName}()";

            string formatted = $"[{timestamp}] [{levelName}] [TraceId: {traceId}] [{callerInfo}] {message}";

            Console.WriteLine(formatted);

            try
            {
                if (File.Exists(GlobalContext.ActiveLogFilePath))
                {
                    File.AppendAllText(GlobalContext.ActiveLogFilePath, formatted + Environment.NewLine);
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"[LOG ERROR] Failed to write log: {ex.Message}");
            }
        }

    }
}
