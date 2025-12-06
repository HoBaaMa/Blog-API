using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;

namespace Blog_API.Configurations
{
    /// <summary>
    /// Configures response caching and compression for improved API performance.
    /// </summary>
    public static class ResponseCachingConfiguration
    {
        /// <summary>
        /// Adds response caching and compression services.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddResponseCachingServices(this IServiceCollection services)
        {
            // Response caching
            services.AddResponseCaching(options =>
            {
                options.MaximumBodySize = 64 * 1024 * 1024; // 64 MB
                options.UseCaseSensitivePaths = false;
            });

            // Response compression
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
                {
                    "application/json",
                    "application/problem+json",
                    "text/plain"
                });
            });

            services.Configure<BrotliCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest;
            });

            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.SmallestSize;
            });

            return services;
        }
    }
}
