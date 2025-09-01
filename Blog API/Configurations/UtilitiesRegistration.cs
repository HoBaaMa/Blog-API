using Blog_API.Utilities;

namespace Blog_API.Configurations
{
    public static class UtilitiesRegistration
    {
        public static IServiceCollection AddUtilities(this IServiceCollection services)
        {
            services.AddScoped<JwtTokenGenerator>();
            return services;
        }
    }
}
