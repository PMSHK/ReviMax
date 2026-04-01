//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Reflection;
//using log4net;
//using log4net.Config;

//namespace ReviMax.Config
//{
//    public class Log4NetConfig
//    {
//        public void Init()
//        {
//            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
//            var logsDir = Path.Combine(dir, "Logs");
//            if (!Directory.Exists(logsDir))
//            {
//                Directory.CreateDirectory(logsDir);
//            }
//            var configFile = new FileInfo(Path.Combine(
//                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
//                    "Log4Net.config"));
//            if (!configFile.Exists) {
//                string path = Path.Combine(
//                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
//                    "Log4Net.config");
//                configFile = CreateAndLoadDefaultConfig(path);
//            }
//                XmlConfigurator.Configure(configFile);
//        }

//        private void CreateDefaultConfig(string configPath)
//        {
//            string defaultConfig = @"<?xml version='1.0' encoding='utf-8' ?>
//<configuration>
//    <log4net>
//      <appender name='FileAppender' type='log4net.Appender.RollingFileAppender'>
//        <file value='ReviMax.log' />
//        <appendToFile value='true' />
//        <rollingStyle value='Size' />
//        <maxSizeRollBackups value='5' />
//        <maximumFileSize value='10MB' />
//        <staticLogFileName value='true' />
//        <layout type='log4net.Layout.PatternLayout'>
//          <conversionPattern value='%date [%thread] %-5level %logger - %message%newline' />
//        </layout>
//      </appender>
//      <root>
//        <level value='DEBUG' />
//        <appender-ref ref='FileAppender' />
//      </root>
//    </log4net>
//</configuration>";

//            File.WriteAllText(configPath, defaultConfig);
//        }

//        private FileInfo CreateAndLoadDefaultConfig(string path)
//        {
//            CreateDefaultConfig(path);
//            return new FileInfo(path);
//        }

//    }

//}
