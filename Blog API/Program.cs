using Blog_API.Configurations;
using Blog_API.Middlewares;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseConfiguredSerilog();

// Services
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddDatabase(builder.Configuration)
    .AddIdentityServices()
    .AddApplicationServices()
    .AddRepositories()
    .AddAutoMapper(typeof(Program).Assembly); // Register AutoMapper (scans the assembly for Profile)
// This scans the assembly containing Program class for any classes inheriting Profile 
// and registers them automatic

// Prevent redirects on 401/403 for API
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = ctx =>
    {
        ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };
    options.Events.OnRedirectToAccessDenied = ctx =>
    {
        ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    };
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

// Ensure authentication runs before authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();
