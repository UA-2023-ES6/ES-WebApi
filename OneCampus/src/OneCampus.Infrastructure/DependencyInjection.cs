﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OneCampus.Domain.Repositories;
using OneCampus.Infrastructure.Data;
using OneCampus.Infrastructure.Repositories;

namespace OneCampus.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.ThrowIfNull();
        configuration.ThrowIfNull();

        var bdConnectionString = configuration.GetConnectionString("OneCampusDb").ThrowIfNull().Value;

        return services
            .AddDbContextFactory<OneCampusDbContext>(options => options.UseMySQL(bdConnectionString))
            .AddTransient<IGroupRepository, GroupRepository>()
            .AddTransient<IUserRepository, UserRepository>()
            .AddTransient<IMessageRepository, MessageRepository>()
            .AddTransient<IInstitutionRepository, InstitutionRepository>()
            .AddTransient<IQuestionRepository, QuestionRepository>()
            .AddTransient<IAnswerRepository, AnswerRepository>()
            .AddTransient<IPermissionRepository, PermissionsRepository>();
    }
}
