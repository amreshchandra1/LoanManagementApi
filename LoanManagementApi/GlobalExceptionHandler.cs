using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LoanManagementApi
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(
            exception,
            "An unhandled execution failure occurred: {Message}",
            exception.Message);
            //var (statusCode, title) = exception switch
            //{
            //    UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized Access"),
            //    KeyNotFoundException => (StatusCodes.Status404NotFound, "Resource Not Found"),
            //    InvalidOperationException => (StatusCodes.Status400BadRequest, "Invalid State/Operation Request"),
            //    _ => (StatusCodes.Status500InternalServerError, "Server Error Encountered")
            //};

            //// 3. Build a production-ready RFC 7807 Problem Details object
            //var problemDetails = new ProblemDetails
            //{
            //    Status = statusCode,
            //    Title = title,
            //    Detail = exception.Message,
            //    Instance = httpContext.Request.Path
            //};

            //// 4. Trace identifiers help developers link UI errors directly to server-side logs
            //problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

            //// 5. Commit payload directly to output stream
            //httpContext.Response.StatusCode = statusCode;
            //await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            // Return true to indicate that this exception has been successfully handled
            return true;
        }
    }
}
