using System.Text.Json;
using AuthAPI.Data;
using AuthAPI.Models.Response;
using AuthAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuthAPI.Middleware;

public class RequestValidationMiddleware
    {
        private readonly RequestDelegate _next;
        
        public RequestValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        
        public async Task InvokeAsync(HttpContext context, IRequestValidationService validationService)
        {
            if (context.Request.Path.StartsWithSegments("/api/auth"))
            {
                var applicationId = context.Request.Headers["ApplicationId"].FirstOrDefault();
                var traceId = context.Request.Headers["TraceId"].FirstOrDefault();
                var securityToken = context.Request.Headers["SecurityToken"].FirstOrDefault();
                
                if (string.IsNullOrEmpty(applicationId) || string.IsNullOrEmpty(traceId) || string.IsNullOrEmpty(securityToken))
                {
                    await WriteErrorResponse(context, "Missing required headers");
                    return;
                }
                
                var isValid = await validationService.ValidateRequestHeaders(applicationId, traceId, securityToken);
                if (!isValid)
                {
                    context.Response.StatusCode = 403;
                    await WriteErrorResponse(context, "Invalid request headers");
                    return;
                }
            }
            
            await _next(context);
        }
        
        private async Task WriteErrorResponse(HttpContext context, string message)
        {
            context.Response.ContentType = "application/json";
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
            
            await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }
    }