//using System;
//using System.IO;
//using Microsoft.Extensions.Logging;
//using Serilog;

//namespace ReviMax.Config
//{
//    public class MicrosoftLogger
//    {
//        public static ILoggerFactory LoggerFactory { get; }
//        static MicrosoftLogger()
//        {

//            string logDirectory = Path.Combine(
//                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
//                "ReviMax",
//                "Logs");
//            Directory.CreateDirectory(logDirectory);
//            string logPath = Path.Combine(logDirectory, "revit.log");
//            Log.Logger = new LoggerConfiguration()
//                .MinimumLevel.Debug()
//                .WriteTo.File(logPath,
//                    rollingInterval: RollingInterval.Day,
//                    retainedFileCountLimit: 7,
//                    outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
//                .CreateLogger();
//            LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
//            {
//                builder
//                    .ClearProviders()
//                    .SetMinimumLevel(LogLevel.Debug)
//                    .AddDebug()          // <-- Debug Output
//                    .AddSerilog();       // <-- Файл
//            });
//        }
//        public static ILogger<T> CreateLogger<T>() => LoggerFactory.CreateLogger<T>();
//    }
//}
