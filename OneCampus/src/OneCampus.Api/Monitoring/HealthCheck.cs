using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace OneCampus.Api.Monitoring;

public class HealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(HealthCheckResult.Healthy("healthy"));
    }
}

