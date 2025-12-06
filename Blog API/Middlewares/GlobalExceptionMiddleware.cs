using Microsoft.AspNetCore.Mvc;

namespace Blog_API.Middlewares
{
    /// <summary>
    /// Middleware that catches unhandled exceptions and returns RFC 7807 ProblemDetails responses.
    /// </summary>
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="GlobalExceptionMiddleware"/> with the next middleware delegate and a logger.
        /// </summary>
        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Invokes the next middleware in the pipeline and provides global exception handling for the request.
        /// </summary>
        public async Task InvokeAsync(HttpContext context)
        {
            // Generate or extract correlation ID
            if (!context.Request.Headers.TryGetValue("X-Correlation-ID", out var correlationId) 
                || string.IsNullOrWhiteSpace(correlationId))
            {
                correlationId = Guid.NewGuid().ToString();
            }
            context.Items["CorrelationId"] = correlationId.ToString();
            context.Response.Headers["X-Correlation-ID"] = correlationId.ToString();

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred. CorrelationId: {CorrelationId}", correlationId);
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Maps an exception to an RFC 7807 ProblemDetails response.
        /// </summary>
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var (title, detail, statusCode, type) = exception switch
            {
                KeyNotFoundException => (
                    "Resource Not Found",
                    exception.Message,
                    StatusCodes.Status404NotFound,
                    "https://tools.ietf.org/html/rfc7231#section-6.5.4"
                ),
                UnauthorizedAccessException => (
                    "Forbidden",
                    "You do not have permission to access this resource.",
                    StatusCodes.Status403Forbidden,
                    "https://tools.ietf.org/html/rfc7231#section-6.5.3"
                ),
                ArgumentException => (
                    "Bad Request",
                    exception.Message,
                    StatusCodes.Status400BadRequest,
                    "https://tools.ietf.org/html/rfc7231#section-6.5.1"
                ),
                InvalidOperationException => (
                    "Bad Request",
                    exception.Message,
                    StatusCodes.Status400BadRequest,
                    "https://tools.ietf.org/html/rfc7231#section-6.5.1"
                ),
                _ => (
                    "Internal Server Error",
                    "An unexpected error occurred. Please try again later.",
                    StatusCodes.Status500InternalServerError,
                    "https://tools.ietf.org/html/rfc7231#section-6.6.1"
                )
            };

            var problemDetails = new ProblemDetails
            {
                Type = type,
                Title = title,
                Status = statusCode,
                Detail = detail,
                Instance = context.Request.Path
            };

            // Add correlation ID and trace ID
            if (context.Items.TryGetValue("CorrelationId", out var correlationId))
            {
                problemDetails.Extensions["correlationId"] = correlationId;
            }
            problemDetails.Extensions["traceId"] = context.TraceIdentifier;

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/problem+json";
            
            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}

