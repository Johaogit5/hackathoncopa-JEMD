using CopaPassenger.Infrastructure.Persistence;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CopaPassenger.API.Middleware;

public class DeepHealthCheck(
    CopaDbContext dbContext,
    IConfiguration configuration,
    ILogger<DeepHealthCheck> logger) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var mode = configuration["HealthCheck:Mode"] ?? "basic";

        if (mode != "deep")
        {
            logger.LogWarning(
                "Deep health check skipped: mode is {Mode}, expected \"deep\". ErrorCode={ErrorCode}", mode, "E4");
            return HealthCheckResult.Degraded(
                "Error E4",
                data: new Dictionary<string, object>
                {
                    ["errorCode"] = "E4",
                    ["mode"] = mode,
                    ["reason"] = "El modo de validacion profunda no esta configurado correctamente"
                });
        }

        try
        {
            var canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);
            var data = new Dictionary<string, object> { ["mode"] = mode, ["database"] = canConnect ? "ok" : "fail" };
            return canConnect
                ? HealthCheckResult.Healthy("Deep check passed", data)
                : HealthCheckResult.Unhealthy("Database unavailable", data: data);
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Deep check failed", ex);
        }
    }
}
