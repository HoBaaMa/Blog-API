using Blog_API.Configurations;
using Blog_API.Middlewares;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseConfiguredSerilog();

// Services
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddEndpointsApiExplorer();

// Register services in the correct order - Database first, then Identity, then Auth
builder.Services
    .AddSwaggerServices()
    .AddDatabase(builder.Configuration)
    .AddIdentityServices()
    .AddAuthServices(builder.Configuration) // Add auth services after identity to override defaults
    .AddApplicationServices()
    .AddRepositories()
    .AddUtilities()
    .AddCorsServices(builder.Configuration)
    .AddRateLimitingServices(builder.Configuration)
    .AddHealthCheckServices()
    .AddResponseCachingServices()
    .AddAutoMapper(typeof(Program).Assembly);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseHttpsRedirection();

// Response compression should be early in the pipeline
app.UseResponseCompression();

app.UseRouting();

// Security middleware order is critical
app.UseRateLimiter();
app.UseCors(CorsConfiguration.DefaultPolicyName);
app.UseAuthentication();
app.UseAuthorization();

// Response caching after auth
app.UseResponseCaching();

app.MapControllers();

// Health check endpoints
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("db"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("api"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();
