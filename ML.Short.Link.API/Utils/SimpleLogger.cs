using System;
using System.IO;

namespace ML.Short.Link.API.Utils
{
    public class SimpleLogger
    {
        private static string _logPath;
        private static readonly object LockObj = new object();

        // Método de inicialización que debe llamarse al inicio de la aplicación
        public static void Initialize(string contentRootPath)
        {
            _logPath = Path.Combine(contentRootPath, "App_Data", "Logs");

            // Crear directorio si no existe
            if (!Directory.Exists(_logPath))
                Directory.CreateDirectory(_logPath);
        }

        public static void LogInfo(string message, string controllerName = "General")
        {
            WriteLog("INFO", message, controllerName);
        }

        public static void LogError(string message, Exception ex = null, string controllerName = "General")
        {
            var fullMessage = message;
            if (ex != null)
            {
                fullMessage += $"\nException: {ex.Message}\nStackTrace: {ex.StackTrace}";
            }
            WriteLog("ERROR", fullMessage, controllerName);
        }

        public static void LogWarning(string message, string controllerName = "General")
        {
            WriteLog("WARNING", message, controllerName);
        }

        private static void WriteLog(string level, string message, string controllerName)
        {
            lock (LockObj)
            {
                try
                {
                    // Si no se inicializó, usar ruta temporal
                    var logPath = _logPath ?? Path.GetTempPath();

                    var fileName = $"API_Log_{DateTime.Now:yyyy-MM-dd}.txt";
                    var filePath = Path.Combine(logPath, fileName);

                    var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] [{controllerName}] {message}{Environment.NewLine}";

                    File.AppendAllText(filePath, logEntry);
                }
                catch
                {
                    // Si falla, intentar escribir en una ubicación alternativa
                    try
                    {
                        var tempPath = Path.GetTempPath();
                        var fileName = $"API_Emergency_Log_{DateTime.Now:yyyy-MM-dd}.txt";
                        var filePath = Path.Combine(tempPath, fileName);
                        var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] [{controllerName}] {message}{Environment.NewLine}";
                        File.AppendAllText(filePath, logEntry);
                    }
                    catch { /* Último recurso: no hacer nada */ }
                }
            }
        }
    }
}