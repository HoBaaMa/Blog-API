using Blog_API.Data;
using Blog_API.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace Blog_API.Configurations
{
    public static class IdentityConfiguration
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<BlogDbContext>()
                .AddDefaultTokenProviders();
            return services;
        }
    }
}
