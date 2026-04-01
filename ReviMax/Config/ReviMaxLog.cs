using System;
using System.IO;
using Serilog;


namespace ReviMax.Config
{
    public class ReviMaxLog
    {
        private static bool _initialized = false;
        private static StreamWriter _writer;
        public static void Init()
        {
            if(_initialized) return;
            _initialized = true;

            string logDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "ReviMax",
                "Logs");
            Directory.CreateDirectory(logDirectory);

            var logFile = Path.Combine(logDirectory, "revit.log");

            _writer = new StreamWriter(
            new FileStream(
                logFile,
                FileMode.Append,
                FileAccess.Write,
                FileShare.ReadWrite))
            { AutoFlush = true
            };
            
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.TextWriter(_writer,
                    outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            Log.Information("Logger initialized");

        }

        public static void Information(string message) => Log.Information(message);
        public static void Error(string message, Exception ex = null)
        {
            if (ex != null) Log.Error(ex, message);
            else Log.Error(message);
        }
        public static void Warning(string message) => Log.Warning(message);
        public static void Debug(string message) => Log.Debug(message);
    }
}

