
using Data.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Data
{
    public static class DBServices
    {
        public static IServiceCollection RegisterDBServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<Context>(x =>
            {
                x.UseSqlite(configuration.GetConnectionString("DBConnection"));
            });

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
