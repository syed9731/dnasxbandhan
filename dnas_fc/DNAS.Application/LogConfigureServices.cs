using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace DNAS.Application
{
    public static class LogConfigureServices
    {
        public static void AddLogApplication(this WebApplicationBuilder builder, IConfiguration configuration)
        {
            #region Serilog Config
            string logPath = configuration.GetSection("AppConfig:LogWritePaths").Value!.ToString();
            var _logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Map(
                    keyPropertyName: "CustId",
                    defaultKey: "anonymous",
                    (userName, logConfiguration) => logConfiguration.File($"{logPath}{DateTime.Now.ToString("yyyy")}\\{DateTime.Now.ToString("MM")}\\log_{userName}_{DateTime.Now:yyyy-MM-dd}.log",rollingInterval:RollingInterval.Hour))
                .CreateLogger();

            builder.Logging.AddSerilog(_logger);

            builder.Host.UseSerilog(_logger);
            #endregion
        }
    }
}