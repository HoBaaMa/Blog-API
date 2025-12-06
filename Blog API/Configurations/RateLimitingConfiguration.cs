using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace Blog_API.Configurations
{
    /// <summary>
    /// Configures rate limiting policies to protect the API from abuse.
    /// </summary>
    public static class RateLimitingConfiguration
    {
        public const string GeneralPolicy = "general";
        public const string AuthPolicy = "auth";

        /// <summary>
        /// Adds rate limiting services with configurable limits.
        /// Configuration is read from the "RateLimiting" section in appsettings.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">The application configuration.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddRateLimitingServices(this IServiceCollection services, IConfiguration configuration)
        {
            var generalPermitLimit = configuration.GetValue("RateLimiting:GeneralPermitLimit", 100);
            var generalWindowMinutes = configuration.GetValue("RateLimiting:GeneralWindowMinutes", 1);
            var authPermitLimit = configuration.GetValue("RateLimiting:AuthPermitLimit", 10);
            var authWindowMinutes = configuration.GetValue("RateLimiting:AuthWindowMinutes", 1);

            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                // General API rate limiter - 100 requests per minute by default
                options.AddFixedWindowLimiter(GeneralPolicy, config =>
                {
                    config.PermitLimit = generalPermitLimit;
                    config.Window = TimeSpan.FromMinutes(generalWindowMinutes);
                    config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    config.QueueLimit = 0;
                });

                // Stricter rate limiter for auth endpoints - 10 requests per minute by default
                options.AddFixedWindowLimiter(AuthPolicy, config =>
                {
                    config.PermitLimit = authPermitLimit;
                    config.Window = TimeSpan.FromMinutes(authWindowMinutes);
                    config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    config.QueueLimit = 0;
                });

                // Global limiter as fallback
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                {
                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: context.Request.Headers.Host.ToString(),
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = generalPermitLimit * 2,
                            Window = TimeSpan.FromMinutes(generalWindowMinutes)
                        });
                });

                options.OnRejected = async (context, cancellationToken) =>
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    
                    if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                    {
                        context.HttpContext.Response.Headers.RetryAfter = retryAfter.TotalSeconds.ToString();
                    }

                    await context.HttpContext.Response.WriteAsJsonAsync(new
                    {
                        type = "https://tools.ietf.org/html/rfc6585#section-4",
                        title = "Too Many Requests",
                        status = 429,
                        detail = "Rate limit exceeded. Please try again later."
                    }, cancellationToken);
                };
            });

            return services;
        }
    }
}
