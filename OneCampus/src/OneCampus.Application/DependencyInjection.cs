using Microsoft.Extensions.DependencyInjection;
using OneCampus.Application.Providers;
using OneCampus.Application.Services;
using OneCampus.Domain.Providers;
using OneCampus.Domain.Services;

namespace OneCampus.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.ThrowIfNull();

        return services
            .AddScoped<IDateTimeProvider, DateTimeProvider>()
            .AddScoped<IGroupService, GroupService>()
            .AddScoped<IMessageService, MessageService>();

    }
}
