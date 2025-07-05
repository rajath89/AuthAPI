using System.Net;
using System.Text.Json;
using AuthAPI.Models.Response;

namespace AuthAPI.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var errorResponse = new ErrorResponse
            {
                ErrorInfo = new List<ErrorInfo>
                {
                    new ErrorInfo
                    {
                        Code = "system_error",
                        Description = "We're sorry, but we can't complete your request at this time. Please try again later."
                    }
                }
            };

            var response = JsonSerializer.Serialize(errorResponse);
            await context.Response.WriteAsync(response);
        }
    }
}