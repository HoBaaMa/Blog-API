using Blog_API.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Blog_API.Configurations
{
    /// <summary>
    /// Configures health check endpoints for monitoring application health.
    /// </summary>
    public static class HealthChecksConfiguration
    {
        /// <summary>
        /// Adds health check services including database connectivity check.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddHealthCheckServices(this IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddDbContextCheck<BlogDbContext>(
                    name: "database",
                    failureStatus: HealthStatus.Unhealthy,
                    tags: new[] { "db", "sql", "sqlserver" })
                .AddCheck("self", () => HealthCheckResult.Healthy("API is running"), 
                    tags: new[] { "api" });

            return services;
        }
    }
}
