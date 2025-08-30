using Serilog;

namespace Blog_API.Configurations
{
    public static class SerilogConfiguration
    {
        public static IHostBuilder UseConfiguredSerilog(this IHostBuilder hostBuilder)
        {
            return hostBuilder.UseSerilog((context, services, loggerConfiguration) =>
            {
                loggerConfiguration.ReadFrom.Configuration(context.Configuration);
                loggerConfiguration.ReadFrom.Services(services);
            });
        }
    }
}
