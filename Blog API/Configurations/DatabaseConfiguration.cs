using Blog_API.Data;
using Microsoft.EntityFrameworkCore;

namespace Blog_API.Configurations
{
    public static class DatabaseConfiguration
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<BlogDbContext>(o =>
                o.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            return services;
        }
    }
}
