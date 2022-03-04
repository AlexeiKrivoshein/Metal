using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;

namespace MetalDiagnostic.Logger
{
    public static class LogService
    {
        private static int _initial = 0;
        private static ConcurrentDictionary<string, ILog> _loggers = new ConcurrentDictionary<string, ILog>();

        public static void InitLogger(LoggerType type)
        {
            var old = Interlocked.CompareExchange(ref _initial, 1, 0);

            if (old != 0) return;

            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();

            PatternLayout patternLayout = new PatternLayout();
            patternLayout.ConversionPattern = "%date %thread %level %logger %message%newline";
            patternLayout.ActivateOptions();

            RollingFileAppender roller = new RollingFileAppender();
            var date = DateTime.Now.ToString("dd-mm-yyyy");
            roller.File = $@"Logs\{Enum.GetName(typeof(LoggerType), type)}_{date}.log";
            roller.AppendToFile = true;
            roller.MaxSizeRollBackups = 10;
            roller.MaximumFileSize = "5MB";
            roller.LockingModel = new FileAppender.MinimalLock();
            roller.Layout = patternLayout;
            roller.RollingStyle = RollingFileAppender.RollingMode.Size;
            roller.StaticLogFileName = true;
            roller.Encoding = Encoding.UTF8;

            roller.ActivateOptions();

            hierarchy.Root.AddAppender(roller);

            ConsoleAppender console = new ConsoleAppender();
            console.Layout = patternLayout;

            console.ActivateOptions();

            hierarchy.Root.AddAppender(console);

            hierarchy.Root.Level = Level.Debug;
            hierarchy.Configured = true;
        }

        public static ILog GetLogger(string name)
        {
            if (_initial == 0) throw new InvalidOperationException("Логгер не инициализирован");

            if (string.IsNullOrEmpty(name)) throw new ArgumentException("Не указанно имя логгера");

            if (_loggers.ContainsKey(name)) return _loggers[name];

            var logger = LogManager.GetLogger(name);
            return _loggers.GetOrAdd(name, logger);
        }
    }
}
