using DNAS.Application.Common.Interface;
using DNAS.Domian.Common;
using Microsoft.Extensions.Options;
using Serilog;

namespace DNAS.Application.Common.Implementation
{
    internal class CustomLogger(IOptions<AppConfig> appConfig) : ICustomLogger
    {
        private readonly AppConfig _appConfig = appConfig.Value;
        private const string logSeparator = "------------------------------------------------------";
        
        public void LogwriteInfo(string message, string filename)
        {
            string path = $"{_appConfig.LogWritePaths}\\{DateTime.Now.Date:yyyyMMdd}\\{filename}.log";
            using var log = new LoggerConfiguration().MinimumLevel.Information().WriteTo.File(path).CreateLogger();
            log.Information(Environment.NewLine + message + Environment.NewLine + logSeparator);
        }
        public void LogwriteError(string message, string filename)
        {
            string path = $"{_appConfig.LogWritePaths}\\{DateTime.Now.Date:yyyyMMdd}\\{filename}.log";
            using var log = new LoggerConfiguration().MinimumLevel.Error().WriteTo.File(path).CreateLogger();
            log.Error(Environment.NewLine + message + Environment.NewLine + logSeparator);
        }
        public void LogwriteWarning(string message, string filename)
        {
            string path = $"{_appConfig.LogWritePaths}\\{DateTime.Now.Date:yyyyMMdd}\\{filename}.log";
            using var log = new LoggerConfiguration().MinimumLevel.Warning().WriteTo.File(path).CreateLogger();
            log.Warning(Environment.NewLine + message + Environment.NewLine + logSeparator);
        }
    }


}
