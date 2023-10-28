using Microsoft.EntityFrameworkCore;
using OneCampus.Application.Providers;
using OneCampus.Infrastructure.Data;
using Throw;

namespace OneCampus;

internal static class DependencyInjection
{
    internal static IServiceCollection AddServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.ThrowIfNull();
        configuration.ThrowIfNull();

        var bdConfiguration = configuration.GetConnectionString("OneCampusDb").ThrowIfNull().Value;

        return services
            .AddDbContext<OneCampusDbContext>(options => options.UseMySQL(bdConfiguration))
            .AddScoped<IDateTimeProvider, DateTimeProvider>();
    }
}