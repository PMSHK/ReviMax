using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReviMax.Config;

namespace ReviMax.Core.Utils.Config
{
    internal class PathManager
    {
        private static string _appDataPath;
        private static string _iconsPath;
        private static string _familiesPath;
        private static string _logsPath;
        private static bool _initialized = false;

        public static void Initialize()
        {
            if (_initialized) return;
            _appDataPath = initAppDataPath();
            _familiesPath = initFamiliesPath();
            _iconsPath = initIconsPath();
            _initialized = true;
        }

        public static string GetAppDataPath()
        {
          if (IsNotInitialized()) Initialize();

          return _appDataPath;
        }

        public static string GetIconsPath()
        {
            if (IsNotInitialized()) Initialize();

            return _iconsPath;
        }

        public static string GetFamiliesPath()
        {
            if (IsNotInitialized()) Initialize();

            return _familiesPath;
        }
        public static string GetLogsPath()
        {
            if (IsNotInitialized()) Initialize();

            return _logsPath;
        }

        public static void CreateDirectoryIfNotExist(string path)
        {
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
        }

        public static string GetFilePathInDirectory(string folderPath, string fileName) 
        {
            string filePath;
            if (string.IsNullOrEmpty(folderPath) || string.IsNullOrEmpty(fileName)) 
            { 
                filePath = string.Empty;
                ReviMaxLog.Warning("Folder path is null or empty.");
                return filePath; 
            }

            filePath = System.IO.Path.Combine(folderPath, fileName);
            if (!System.IO.File.Exists(filePath))
            {
                ReviMaxLog.Warning($"File {fileName} does not exist in directory {folderPath}");
                filePath = string.Empty;
            }
            return filePath;
        }

        private static bool IsNotInitialized()
        {
            return string.IsNullOrEmpty(_appDataPath)
                && string.IsNullOrEmpty(_iconsPath)
                && string.IsNullOrEmpty(_familiesPath);
        }

        private static string initAppDataPath()
        {
            string appDataPath = System.IO.Path.Combine(
          Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
          "ReviMax");
            CreateDirectoryIfNotExist(appDataPath);
            return appDataPath;
        }

        private static string initFamiliesPath()
        {
            string familiesPath = System.IO.Path.Combine(_appDataPath, "Families");
            CreateDirectoryIfNotExist(familiesPath);
            return familiesPath;
        }

        private static string initIconsPath()
        {
            string iconsPath = System.IO.Path.Combine(_appDataPath, "Icons");
            CreateDirectoryIfNotExist(iconsPath);
            return iconsPath;
        }

        private static string initLogsPath()
        {
            string logPath = System.IO.Path.Combine(_appDataPath, "Logs");
            CreateDirectoryIfNotExist(logPath);
            return logPath;
        }
    }
}
