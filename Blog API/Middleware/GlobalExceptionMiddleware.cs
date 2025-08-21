namespace Blog_API.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private ILogger<GlobalExceptionMiddleware> _logger;
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
        /// If an exception is thrown by downstream middleware, the exception is logged and handled so an appropriate HTTP response is produced.
        /// </summary>
        /// <param name="context">The current HTTP context for the request.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }
        /// <summary>
        /// Maps an exception to an HTTP status code and JSON message, sets the response status, and writes the message to the response body.
        /// </summary>
        /// <remarks>
        /// Exception-to-response mapping:
        /// - KeyNotFoundException => uses the exception message, 404 Not Found
        /// - UnauthorizedAccessException => "Access denied", 403 Forbidden
        /// - ArgumentException => "Invalid request.", 400 Bad Request
        /// - default => "Internal server error", 500 Internal Server Error
        /// The response body is written as JSON. The response Content-Type is not set by this method.
        /// </remarks>
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            //context.Response.ContentType = "application/json";

            var (message, statusCode) = exception switch
            {
                KeyNotFoundException => (exception.Message, 404),
                UnauthorizedAccessException => ("Access denied", 403),
                ArgumentException => ("Invalid request.", 400),
                _ => ("Internal server error", 500)
            };
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(message);
        }
    }
}
