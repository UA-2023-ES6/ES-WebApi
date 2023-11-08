using OneCampus.Application;
using OneCampus.Infrastructure;

namespace OneCampus;

internal static class DependencyInjection
{
    internal static IServiceCollection AddServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.ThrowIfNull();
        configuration.ThrowIfNull();

        return services
            .AddApplication()
            .AddInfrastructure(configuration);
    }
}
