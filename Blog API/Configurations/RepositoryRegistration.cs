using Blog_API.Repositories.Implementations;
using Blog_API.Repositories.Interfaces;

namespace Blog_API.Configurations
{
    public static class RepositoryRegistration
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IBlogPostRepository, BlogPostRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<ILikeRepository, LikeRepository>();
            services.AddScoped<ITagRepository, TagRepository>();
            return services;
        }
    }
}
