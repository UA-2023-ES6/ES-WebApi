using ES6WebApi.Database;
using ES6WebApi.Database.Data;
using ES6WebApi.Providers;
using ES6WebApi.Providers.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ES6WebApi;

internal static class DependencyInjection
{
    internal static IServiceCollection AddServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        // SQL
        services.AddDbContext<ES6DbContext>(options => options.UseMySQL(configuration.GetConnectionString("ES6Db")));

        return services
            .AddScoped<IDateTimeProvider, DateTimeProvider>();
    }
}