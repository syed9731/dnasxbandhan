using DNAS.Application.Business.Implementation;
using DNAS.Application.Business.Interface;
using DNAS.Application.Common.Filter;
using DNAS.Application.Common.Implementation;
using DNAS.Application.Common.Interface;
using DNAS.Application.Middleware;
using DNAS.Domian.Common;

using FluentValidation;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System.Reflection;

namespace DNAS.Application
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        { 
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddUserSecrets(Assembly.GetEntryAssembly()!)
                .Build();
            var assembly = typeof(ConfigureServices).Assembly;
            services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssembly(assembly));
            services.AddValidatorsFromAssembly(assembly);
            services.Configure<AppConfig>(configuration.GetSection(AllDefaultConstants.AppConfig));
            services.Configure<ConnectionStrings>(configuration.GetSection(AllDefaultConstants.ConnectionStrings));
            services.AddScoped<ICustomLogger, CustomLogger>();
            services.AddScoped<IEncryption, Encryption>();
            services.AddScoped<IEncryptionSha, EncryptionSha>();
            services.AddScoped<ICheckExtension, CheckExtension>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddSingleton<RateLimitMiddleware>();
            services.AddScoped<BlockBurpSuiteMiddleware>();
            services.AddScoped<IFileValidation, FileValidation>();
            services.AddScoped<UserCurrentAuth>();
            services.AddScoped<LogContextMiddleware>();
            services.AddScoped<IFileValidation, FileValidation>();
            services.AddScoped<OtpVaptMiddleware>();
            return services;
        }
    }
}
