using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace API.Middleware;

public class GlobalExceptionHandler
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var errorResponse = new ErrorResponse();
        var statusCode = HttpStatusCode.InternalServerError;

        switch (exception)
        {
            case ValidationException validationEx:
                statusCode = HttpStatusCode.BadRequest;
                errorResponse.Message = "Validation failed";
                errorResponse.ErrorCode = validationEx.ErrorCode;
                errorResponse.ValidationErrors = validationEx.ValidationErrors;
                _logger.LogWarning(exception, "Validation error occurred: {Message}", exception.Message);
                break;

            case NotFoundException notFoundEx:
                statusCode = HttpStatusCode.NotFound;
                errorResponse.Message = notFoundEx.Message;
                errorResponse.ErrorCode = notFoundEx.ErrorCode;
                _logger.LogInformation(exception, "Resource not found: {Message}", exception.Message);
                break;

            case BusinessException businessEx:
                statusCode = HttpStatusCode.BadRequest;
                errorResponse.Message = businessEx.Message;
                errorResponse.ErrorCode = businessEx.ErrorCode;
                _logger.LogWarning(exception, "Business logic error: {Message}", exception.Message);
                break;

            case ArgumentException argEx:
                statusCode = HttpStatusCode.BadRequest;
                errorResponse.Message = "Invalid argument provided";
                errorResponse.Details = argEx.Message;
                errorResponse.ErrorCode = "INVALID_ARGUMENT";
                _logger.LogWarning(exception, "Invalid argument: {Message}", exception.Message);
                break;

            case UnauthorizedAccessException:
                statusCode = HttpStatusCode.Unauthorized;
                errorResponse.Message = "Access denied";
                errorResponse.ErrorCode = "UNAUTHORIZED";
                _logger.LogWarning(exception, "Unauthorized access attempt");
                break;

            case InvalidOperationException invalidOpEx:
                statusCode = HttpStatusCode.BadRequest;
                errorResponse.Message = "Invalid operation";
                errorResponse.Details = invalidOpEx.Message;
                errorResponse.ErrorCode = "INVALID_OPERATION";
                _logger.LogWarning(exception, "Invalid operation: {Message}", exception.Message);
                break;

            default:
                // Log unexpected exceptions with full details
                _logger.LogError(exception, "An unexpected error occurred: {Message}", exception.Message);

                // In development, include exception details
                if (context.RequestServices.GetService<IWebHostEnvironment>()?.EnvironmentName == "Development")
                {
                    errorResponse.Message = "An unexpected error occurred";
                    errorResponse.Details = exception.Message;
                    errorResponse.ErrorCode = "INTERNAL_ERROR";
                }
                else
                {
                    errorResponse.Message = "An unexpected error occurred. Please try again later.";
                    errorResponse.ErrorCode = "INTERNAL_ERROR";
                }
                break;
        }

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}
