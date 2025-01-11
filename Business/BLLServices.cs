using Business.Interfaces;
using Business.Services;
using Business.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Business
{
    public static class BusinessServices
    {
        public static IServiceCollection RegisterBusinessServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddSingleton(typeof(ConnectionMapping<string>));

            return services;
        }
    }
}
