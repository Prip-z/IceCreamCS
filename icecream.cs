using System;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Colorful; 
using Console = Colorful.Console;

namespace IceCreamCSharp
{
    public enum LogLevel
    {
        INFO,
        WARN,
        ERROR
    }

    public delegate string ArgToStringDelegate(object obj);

    public static class IceCream
    {
        private static LogLevel _logLevel = LogLevel.INFO;
        private static string _logFilePath = "logs.txt";
        private static bool _isLoggingEnabled = true;
        private static string _prefix = "ic| ";
        private static ArgToStringDelegate _argToStringFunction = DefaultArgToString;
        private static bool _includeContext = true;
        private static bool _contextAbsPath = false;

        private static readonly Color InfoColor = Color.Green;
        private static readonly Color WarnColor = Color.Yellow;
        private static readonly Color ErrorColor = Color.Red;
        private static readonly Color DefaultColor = Color.White;

        private static readonly StyleSheet LogStyleSheet;

        static IceCream()
        {
            LogStyleSheet = new StyleSheet(Color.White);
            LogStyleSheet.AddStyle("INFO", InfoColor);
            LogStyleSheet.AddStyle("WARN", WarnColor);
            LogStyleSheet.AddStyle("ERROR", ErrorColor);
            LogStyleSheet.AddStyle("WARNING", WarnColor);
        }

        public static void SetLogLevel(LogLevel level)
        {
            _logLevel = level;
        }

        public static void SetLogFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                PrintWarning("SetLogFile: Путь к лог-файлу не может быть пустым.");
                return;
            }

            _logFilePath = path;
            PrintInfo($"Log file path set to: {_logFilePath}");
        }

        public static void EnableLogging(bool enable)
        {
            _isLoggingEnabled = enable;
            PrintInfo($"Logging has been {(enable ? "enabled" : "disabled")}.");
        }

        public static void SetPrefix(string prefix)
        {
            _prefix = prefix;
            PrintInfo($"Log prefix set to: {_prefix}");
        }

        public static void SetArgToStringFunction(ArgToStringDelegate func)
        {
            if (func != null)
            {
                _argToStringFunction = func;
                PrintInfo("ArgToStringFunction has been updated.");
            }
            else
            {
                PrintWarning("SetArgToStringFunction: Передана пустая функция.");
            }
        }

        public static void SetIncludeContext(bool include)
        {
            _includeContext = include;
            PrintInfo($"IncludeContext set to: {_includeContext}");
        }
        public static void SetContextAbsPath(bool absPath)
        {
            _contextAbsPath = absPath;
            PrintInfo($"ContextAbsPath set to: {_contextAbsPath}");
        }

        private static void LogToFile(string message)
        {
            try
            {
                string directory = Path.GetDirectoryName(_logFilePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using (StreamWriter writer = new StreamWriter(_logFilePath, append: true))
                {
                    writer.WriteLine(message);  
                    writer.Flush();  
                }
            }
            catch (Exception ex)
            {
                PrintWarning($"Error writing to log file: {ex.Message}");
            }
        }

        private static void PrintMessage(string message, LogLevel level)
        {
            if (_isLoggingEnabled)
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                string logMessage = $"{_prefix}{timestamp} - {message}";

                try
                {
                   
                    Console.WriteLineStyled(logMessage, LogStyleSheet);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to write styled message: {ex.Message}");
                    Console.WriteLine(logMessage);
                }

                LogToFile(logMessage);
            }
        }

        private static void PrintInfo(string message)
        {
            if (_isLoggingEnabled)
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                string logMessage = $"{_prefix}{timestamp} - INFO: {message}";

                try
                {
                    Console.WriteLineStyled(logMessage, LogStyleSheet);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to write styled message: {ex.Message}");
                    Console.WriteLine(logMessage);
                }

                LogToFile(logMessage);
            }
        }

        private static void PrintWarning(string message)
        {
            if (_isLoggingEnabled)
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                string logMessage = $"{_prefix}{timestamp} - WARNING: {message}";

                try
                {
                    Console.WriteLineStyled(logMessage, LogStyleSheet);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to write styled message: {ex.Message}");
                    Console.WriteLine(logMessage);
                }

                LogToFile(logMessage);
            }
        }

        private static string GetFileName(string filePath)
        {
            return string.IsNullOrEmpty(filePath) ? "UnknownFile" : Path.GetFileName(filePath);
        }

        private static string DefaultArgToString(object obj)
        {
            if (obj == null)
                return "null";

            try
            {
                return JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = false });
            }
            catch
            {
                return obj.ToString();
            }
        }

        public static void ic(object value, LogLevel level = LogLevel.INFO,
            [CallerMemberName] string callerName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            if (!_isLoggingEnabled || level < _logLevel) return;

            string fileName = GetFileName(filePath);
            string context = _includeContext ? $"{fileName}:{lineNumber} in {callerName}()" : "";
            string valueStr = _argToStringFunction(value);
            string logMessage = string.IsNullOrEmpty(context)
                ? $"{level}: {valueStr}"
                : $"{level}: {context}: {valueStr}";

            PrintMessage(logMessage, level);
        }

        public static void ic(LogLevel level = LogLevel.INFO,
            [CallerMemberName] string callerName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0,
            params object[] values)
        {
            if (!_isLoggingEnabled || level < _logLevel || values == null || values.Length == 0) return;

            string fileName = GetFileName(filePath);
            string context = _includeContext ? $"{fileName}:{lineNumber} in {callerName}()" : "";
            string allValues = string.Join(", ", Array.ConvertAll(values, v => _argToStringFunction(v)));
            string logMessage = string.IsNullOrEmpty(context)
                ? $"{level}: {allValues}"
                : $"{level}: {context}: {allValues}";

            PrintMessage(logMessage, level);
        }

        public static void icExpression<T>(string expression, T value, LogLevel level = LogLevel.INFO,
            [CallerMemberName] string callerName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            if (!_isLoggingEnabled || level < _logLevel) return;

            string fileName = GetFileName(filePath);
            string context = _includeContext ? $"{fileName}:{lineNumber} in {callerName}()" : "";
            string valueStr = _argToStringFunction(value);
            string logMessage = string.IsNullOrEmpty(context)
                ? $"{level}: {expression} = {valueStr}"
                : $"{level}: {context}: {expression} = {valueStr}";

            PrintMessage(logMessage, level);
        }

        public static void PrintDetailed(object value, LogLevel level = LogLevel.INFO,
            [CallerMemberName] string callerName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            if (!_isLoggingEnabled || level < _logLevel) return;

            string fileName = GetFileName(filePath);
            string context = _includeContext ? $"{fileName}:{lineNumber} in {callerName}()" : "";
            string typeName = value?.GetType().Name ?? "null";
            string valueStr = _argToStringFunction(value);
            string logMessage = string.IsNullOrEmpty(context)
                ? $"{level}: [{typeName}] {valueStr}"
                : $"{level}: {context} [{typeName}]: {valueStr}";

            PrintMessage(logMessage, level);
        }

        public static void icPretty(object value, LogLevel level = LogLevel.INFO,
            [CallerMemberName] string callerName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            if (!_isLoggingEnabled || level < _logLevel) return;

            string fileName = GetFileName(filePath);
            string context = _includeContext ? $"{fileName}:{lineNumber} in {callerName}()" : "";
            string jsonString;

            try
            {
                jsonString = JsonSerializer.Serialize(value, new JsonSerializerOptions { WriteIndented = true });
            }
            catch (Exception ex)
            {
                jsonString = $"Error serializing object: {ex.Message}";
            }

            string logMessage = string.IsNullOrEmpty(context)
                ? $"{level}: {jsonString}"
                : $"{level}: {context}:\n{jsonString}";

            PrintMessage(logMessage, level);
        }

        public static async Task PrintAsync(object value, LogLevel level = LogLevel.INFO,
            [CallerMemberName] string callerName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0,
            CancellationToken cancellationToken = default)
        {
            if (!_isLoggingEnabled || level < _logLevel) return;

            await Task.Run(() =>
            {
                if (cancellationToken.IsCancellationRequested) return;

                ic(value, level, callerName, filePath, lineNumber);
            }, cancellationToken);
        }

        public static async Task PrintMultipleAsync(LogLevel level = LogLevel.INFO,
            [CallerMemberName] string callerName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0,
            params object[] values)
        {
            if (!_isLoggingEnabled || level < _logLevel || values == null || values.Length == 0) return;

            await Task.Run(() =>
            {
                ic(level: level, callerName: callerName, filePath: filePath, lineNumber: lineNumber, values: values);
            });
        }

        public static async Task PrintExpressionAsync<T>(string expression, T value, LogLevel level = LogLevel.INFO,
            [CallerMemberName] string callerName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0,
            CancellationToken cancellationToken = default)
        {
            if (!_isLoggingEnabled || level < _logLevel) return;

            await Task.Run(() =>
            {
                if (cancellationToken.IsCancellationRequested) return;

                icExpression(expression, value, level, callerName, filePath, lineNumber);
            }, cancellationToken);
        }


        public static async Task PrintDetailedAsync(object value, LogLevel level = LogLevel.INFO,
            [CallerMemberName] string callerName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0,
            CancellationToken cancellationToken = default)
        {
            if (!_isLoggingEnabled || level < _logLevel) return;

            await Task.Run(() =>
            {
                if (cancellationToken.IsCancellationRequested) return;

                PrintDetailed(value, level, callerName, filePath, lineNumber);
            }, cancellationToken);
        }

        public static async Task PrintPrettyAsync(object value, LogLevel level = LogLevel.INFO,
            [CallerMemberName] string callerName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0,
            CancellationToken cancellationToken = default)
        {
            if (!_isLoggingEnabled || level < _logLevel) return;

            await Task.Run(() =>
            {
                if (cancellationToken.IsCancellationRequested) return;

                icPretty(value, level, callerName, filePath, lineNumber);
            }, cancellationToken);
        }
    }
}
