using Blog_API.Configurations;
using Blog_API.Middlewares;

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
    .AddAutoMapper(typeof(Program).Assembly);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseRouting();

// Authentication middleware order is critical
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
