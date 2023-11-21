using Microsoft.EntityFrameworkCore;
using OneCampus;
using OneCampus.Api.Middlewares;
using OneCampus.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddServices(builder.Configuration);

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
app.UseMiddleware<UserAuthMiddleware>();



// MockData
using (var scope = app.Services.CreateScope())
{
    var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<OneCampusDbContext>>();

    const int DefaultInstitutionId = 1;

    using (var context = await factory.CreateDbContextAsync())
    {
        // Add mock institutions
        var institution = await AddInstitutionAsync(context, "Default Institution", DefaultInstitutionId);
    }
}

app.Run();

// only for mock data

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
