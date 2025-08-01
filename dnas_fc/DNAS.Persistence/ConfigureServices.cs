using DNAS.Application.IDapperRepository;
using DNAS.Application.IEntityRepository;
using DNAS.Application.IRepository;
using DNAS.Persistence.DapperRepository;
using DNAS.Persistence.DataAccessContents;
using DNAS.Persistence.EntityRepository;
using DNAS.Persistence.Repository;
using DNAS.Persistence.SqlFactory;

using Microsoft.Extensions.DependencyInjection;

namespace DNAS.Persistence
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services)
        {
            services.AddScoped<DataContext>();
            services.AddScoped<IDapperFactory, DapperFactory>();
            services.AddScoped<ISqlDapper, SqlDapper>();
            services.AddScoped<ILogin, Login>();
            services.AddScoped<IMailSend, MailSend>();
            services.AddScoped<INote, Repository.Note>();
            services.AddScoped<IUpdate, Update>();
            services.AddScoped<ISave, Save>();
            services.AddScoped<IDelete, Delete>();
            services.AddScoped<IDashboard, Dashboard>();
            services.AddScoped<INotificationRep, NotificationRep>();
            services.AddScoped<IConfiguration, Repository.Configuration>();
            services.AddScoped<ITemplate, Template>();
            services.AddScoped<IFetch, Fetch>();
            services.AddScoped<ILdapCheck, LdapCheck>();
            return services;
        }
    }
}
