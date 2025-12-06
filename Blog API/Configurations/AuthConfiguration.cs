using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Blog_API.Configurations
{
    public static class AuthConfiguration
    {
        /// <summary>
        /// Configures JWT Bearer authentication with environment variable support for the secret key.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">The application configuration.</param>
        /// <returns>The service collection for chaining.</returns>
        /// <exception cref="InvalidOperationException">Thrown when JWT secret key is not configured.</exception>
        public static IServiceCollection AddAuthServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Read JWT key from environment variable first, then fallback to configuration
            var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") 
                         ?? configuration["Jwt:Key"];

            if (string.IsNullOrWhiteSpace(jwtKey))
            {
                throw new InvalidOperationException(
                    "JWT secret key is not configured. Set the 'JWT_SECRET_KEY' environment variable " +
                    "or configure 'Jwt:Key' in appsettings.json for development.");
            }

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };
            });
            
            return services;
        }
    }
}
