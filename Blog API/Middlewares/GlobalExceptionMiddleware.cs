namespace Blog_API.Middlewares
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
        /// If a downstream middleware throws, the exception is logged and handled by <see cref="HandleExceptionAsync(HttpContext, Exception)"/>.
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
        /// Maps an exception to an HTTP status code and writes a JSON response message to the given <see cref="HttpContext.Response"/>.
        /// </summary>
        /// <param name="context">The current HTTP context whose response will be written.</param>
        /// <param name="exception">The exception to translate into an HTTP status and message.</param>
        /// <returns>A task that completes when the response has been written.</returns>
        /// <remarks>
        /// Exception-to-status mapping:
        /// - <see cref="KeyNotFoundException"/> => 404, returns the exception's message.
        /// - <see cref="UnauthorizedAccessException"/> => 403, message "Access denied".
        /// - <see cref="ArgumentException"/> => 400, message "Invalid request.".
        /// - <see cref="InvalidOperationException">=> 400, returns the exception's message</see>
        /// - all other exceptions => 500, message "Internal server error".
        /// The method sets <see cref="HttpResponse.StatusCode"/> and writes the message as JSON.
        /// </remarks>
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            //context.Response.ContentType = "application/json";

            var (message, statusCode) = exception switch
            {
                KeyNotFoundException => (exception.Message, 404),
                UnauthorizedAccessException => ("Access denied", 403),
                ArgumentException => ("Invalid request.", 400),
                InvalidOperationException => (exception.Message, 400),
                _ => ("Internal server error", 500)
            };
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(message);
        }
    }
}
