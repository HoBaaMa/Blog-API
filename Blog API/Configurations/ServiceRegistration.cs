using Blog_API.Data;
using Blog_API.Models.Entities;
using Blog_API.Repositories.Implementations;
using Blog_API.Repositories.Interfaces;
using Blog_API.Services.Implementation;
using Blog_API.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Blog_API.Configurations
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services )
        {
            services.AddScoped<IBlogPostService, BlogPostService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ILikeService, LikeService>();
            return services;
        }
    }
}
