using OneCampus;
using OneCampus.Api;
using OneCampus.Api.Middlewares;
using OneCampus.Api.Monitoring;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddServices(builder.Configuration);

builder.Services.AddHealthChecks()
    .AddCheck<HealthCheck>(nameof(HealthCheck));

// Only for debug data
builder.Services.AddScoped<DebugDataService>();

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

app.MapHealthChecks("/healthz");

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<UserAuthMiddleware>();

// Add mock data
using (var scope = app.Services.CreateScope())
{
    var debugDataService = scope.ServiceProvider.GetRequiredService<DebugDataService>();
    await debugDataService.CreateDefaultDataAsync();
}

app.Run();

// only for mock data
