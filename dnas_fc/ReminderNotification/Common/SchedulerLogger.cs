using Serilog;

namespace ReminderNotification.Common
{

    public static class SchedulerLogger
    {        
        private const string logSeparator = "------------------------------------------------------";
        public static void LogwriteInfo(string message, string filename)
        {
            string path = $"Logs\\{DateTime.Now.Date:yyyyMMdd}\\{filename}.log";
            using var log = new LoggerConfiguration().MinimumLevel.Information().WriteTo.File(path).CreateLogger();
            log.Information(Environment.NewLine + message + Environment.NewLine + logSeparator);
        }
        public static void LogwriteError(string message, string filename)
        {
            string path = $"Logs\\{DateTime.Now.Date:yyyyMMdd}\\{filename}.log";
            using var log = new LoggerConfiguration().MinimumLevel.Error().WriteTo.File(path).CreateLogger();
            log.Error(Environment.NewLine + message + Environment.NewLine + logSeparator);
        }
        public static void LogwriteWarning(string message, string filename)
        {
            string path = $"Logs\\{DateTime.Now.Date:yyyyMMdd}\\{filename}.log";
            using var log = new LoggerConfiguration().MinimumLevel.Warning().WriteTo.File(path).CreateLogger();
            log.Warning(Environment.NewLine + message + Environment.NewLine + logSeparator);
        }
    }
}
