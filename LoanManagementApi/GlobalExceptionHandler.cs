using LoanManagementApi.Model;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Data.Common;

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
            var (statusCode, title) = exception switch
            {
                Microsoft.EntityFrameworkCore.DbUpdateException => (StatusCodes.Status500InternalServerError, "Some error occured at database side"),
                UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized Access"),
                KeyNotFoundException => (StatusCodes.Status404NotFound, "Resource Not Found"),
                InvalidOperationException => (StatusCodes.Status400BadRequest, "Invalid State/Operation Request"),
                _ => (StatusCodes.Status500InternalServerError, "Server Error Encountered")
               
            };

            var problemDetails = new Error
            {
                Status = statusCode,
                Title = title,
                Detail = exception.Message,
                InnerException = exception?.InnerException?.ToString(),
                Instance = httpContext.Request.Path
            };

            httpContext.Response.StatusCode = statusCode;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}
