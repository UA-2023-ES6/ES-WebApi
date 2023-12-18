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
            .AddServiceAndLazyInitialization<IUsersService, UsersService>()
            .AddTransient<IGroupService, GroupService>()
            .AddTransient<IDateTimeProvider, DateTimeProvider>()
            .AddTransient<IMessageService, MessageService>()
            .AddTransient<IQuestionService, QuestionService>()            
            .AddTransient<IAnswerService, AnswerService>()
            .AddTransient<IPermissionService, PermissionService>();
    }

    private static IServiceCollection AddServiceAndLazyInitialization<TService, TImplementation>(this IServiceCollection services)
        where TService : class
        where TImplementation : class, TService
    {
        return services
            .AddTransient<TService, TImplementation>()
            .AddTransient(provider => new Lazy<TService>(() => provider.GetRequiredService<TService>()));
    }
}
