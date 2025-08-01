using DNAS.Application.IService;
using DNAS.Shared.Service;
using Microsoft.Extensions.DependencyInjection;

namespace DNAS.Shared
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddShared(this IServiceCollection services)
        {
            services.AddScoped<IMailService, MailService>();
            services.AddScoped<ICaptchaService, CaptchaService>();
            services.AddScoped<IOtpService, OtpService>();
            return services;
        }
    }
}
