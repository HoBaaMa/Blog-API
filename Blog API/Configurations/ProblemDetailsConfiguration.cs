using Microsoft.AspNetCore.Mvc;

namespace Blog_API.Configurations
{
    /// <summary>
    /// Configures RFC 7807 ProblemDetails for standardized error responses.
    /// </summary>
    public static class ProblemDetailsConfiguration
    {
        /// <summary>
        /// Adds ProblemDetails services for standardized error responses.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddProblemDetailsServices(this IServiceCollection services)
        {
            services.AddProblemDetails(options =>
            {
                options.CustomizeProblemDetails = context =>
                {
                    // Add correlation ID if available
                    if (context.HttpContext.Items.TryGetValue("CorrelationId", out var correlationId))
                    {
                        context.ProblemDetails.Extensions["correlationId"] = correlationId;
                    }

                    // Add instance (request path)
                    context.ProblemDetails.Instance = context.HttpContext.Request.Path;

                    // Add trace ID for debugging
                    context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
                };
            });

            return services;
        }
    }
}
