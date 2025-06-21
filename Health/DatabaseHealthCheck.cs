using Microsoft.Extensions.Diagnostics.HealthChecks;
using Movies.Application.Database;

namespace Movies.Api.Health
{
    public class DatabaseHealthCheck : IHealthCheck
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        private readonly ILogger<DatabaseHealthCheck> _logger;

        public DatabaseHealthCheck(IDbConnectionFactory dbConnectionFactory, ILogger<DatabaseHealthCheck> logger)
        {
            _dbConnectionFactory = dbConnectionFactory;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            { 
                _ = await _dbConnectionFactory.CreateAsync(cancellationToken);
                return HealthCheckResult.Healthy("Database connection is healthy.");

            }
            catch (Exception e)
            {
                _logger.LogError("Database health check failed.",e);
                return HealthCheckResult.Unhealthy();
            }

        }
    }
}
