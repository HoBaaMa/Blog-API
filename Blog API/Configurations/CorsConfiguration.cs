namespace Blog_API.Configurations
{
    /// <summary>
    /// Configures Cross-Origin Resource Sharing (CORS) policies.
    /// </summary>
    public static class CorsConfiguration
    {
        public const string DefaultPolicyName = "BlogApiCorsPolicy";

        /// <summary>
        /// Adds CORS services with a configurable, restrictive policy.
        /// Origins are read from the "Cors:AllowedOrigins" configuration section.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">The application configuration.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddCorsServices(this IServiceCollection services, IConfiguration configuration)
        {
            var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() 
                                 ?? Array.Empty<string>();

            services.AddCors(options =>
            {
                options.AddPolicy(DefaultPolicyName, builder =>
                {
                    if (allowedOrigins.Length > 0)
                    {
                        builder.WithOrigins(allowedOrigins)
                               .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE", "OPTIONS")
                               .WithHeaders("Authorization", "Content-Type", "Accept", "X-Correlation-ID")
                               .AllowCredentials();
                    }
                    else
                    {
                        // Fallback for development when no origins configured
                        builder.AllowAnyOrigin()
                               .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE", "OPTIONS")
                               .WithHeaders("Authorization", "Content-Type", "Accept", "X-Correlation-ID");
                    }
                });
            });

            return services;
        }
    }
}
