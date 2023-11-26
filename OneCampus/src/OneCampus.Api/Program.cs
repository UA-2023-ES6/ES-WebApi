using Microsoft.EntityFrameworkCore;
using OneCampus;
using OneCampus.Api.Middlewares;
using OneCampus.Application.Services;
using OneCampus.Domain.Repositories;
using OneCampus.Domain.Services;
using OneCampus.Infrastructure.Data;
using OneCampus.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

//builder.Configuration.AddEnvironmentVariables();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddServices(builder.Configuration);

builder.Services.AddScoped<IQuestionService, QuestionService>();
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();

builder.Services.AddScoped<IAnswerService, AnswerService>();
builder.Services.AddScoped<IAnswerRepository, AnswerRepository>();



builder.Services.AddCors();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<ErrorHandlingMiddleware>();

// MockData
using (var scope = app.Services.CreateScope())
{
    var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<OneCampusDbContext>>();

    const string DefaultUserId = "b8e7f65a-f6ca-4211-a562-1fb022636e87";
    const int DefaultInstitutionId = 1;

    var defaultUserId = Guid.Parse(DefaultUserId);

    using (var context = await factory.CreateDbContextAsync())
    {
        // Add mock Users
        await AddUserAsync(context, "Default User", "defaultUser@cenas", DefaultUserId);
        // await AddUserAsync(context, "Default User", "defaultUser@cenas");

        // Add mock institutions
        var institution = await AddInstitutionAsync(context, "Default Institution", DefaultInstitutionId);
        // await AddInstitutionAsync(context, "Default Institution");
    }
}

app.Run();

// only for mock data
async Task<OneCampus.Infrastructure.Data.Entities.User> AddUserAsync(
    OneCampusDbContext context,
    string name,
    string email,
    string? id = null)
{
    if (!Guid.TryParse(id, out var userId))
    {
        userId = Guid.NewGuid();
    }

    var user = await context.Users.FindAsync(userId);
    if (user is null)
    {
        var result = await context.Users
                .AddAsync(new OneCampus.Infrastructure.Data.Entities.User
                {
                    Id = userId,
                    Name = name,
                    Email = email,
                    Password = "in the future",
                    CreateDate = DateTime.UtcNow
                });

        await context.SaveChangesAsync();

        user = result.Entity;
    }

    return user;
}

async Task<OneCampus.Infrastructure.Data.Entities.Institution> AddInstitutionAsync(
    OneCampusDbContext context,
    string name,
    int? id = null)
{
    OneCampus.Infrastructure.Data.Entities.Institution? institution = null;
    if (id.HasValue)
    {
        institution = await context.Institutions.FindAsync(id.Value);
    }

    if (institution is null)
    {
        var defaultGroup = new OneCampus.Infrastructure.Data.Entities.Group
        {
            Name = name,
            CreateDate = DateTime.UtcNow
        };

        var result = await context.Institutions
                .AddAsync(new OneCampus.Infrastructure.Data.Entities.Institution
                {
                    Id = id ??0,
                    Name = name,
                    CreateDate = DateTime.UtcNow,
                    Group = defaultGroup
                });

        await context.SaveChangesAsync();

        institution = result.Entity;
    }

    return institution;
}
