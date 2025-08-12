using System.ComponentModel.DataAnnotations;

namespace Core.Models;

/// <summary>
/// Standard error response model for API errors
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Error message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Error details (optional)
    /// </summary>
    public string? Details { get; set; }

    /// <summary>
    /// Error code (optional)
    /// </summary>
    public string? ErrorCode { get; set; }

    /// <summary>
    /// Timestamp when error occurred
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Validation errors (for validation exceptions)
    /// </summary>
    public IEnumerable<ValidationError>? ValidationErrors { get; set; }
}

/// <summary>
/// Validation error details
/// </summary>
public class ValidationError
{
    /// <summary>
    /// Field name that has validation error
    /// </summary>
    public string Field { get; set; } = string.Empty;

    /// <summary>
    /// Error message for the field
    /// </summary>
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Custom business logic exception
/// </summary>
public class BusinessException : Exception
{
    public string ErrorCode { get; }

    public BusinessException(string message, string errorCode = "BUSINESS_ERROR")
        : base(message)
    {
        ErrorCode = errorCode;
    }

    public BusinessException(string message, Exception innerException, string errorCode = "BUSINESS_ERROR")
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}

/// <summary>
/// Resource not found exception
/// </summary>
public class NotFoundException : BusinessException
{
    public NotFoundException(string message)
        : base(message, "NOT_FOUND")
    {
    }
}

/// <summary>
/// Validation exception
/// </summary>
public class ValidationException : BusinessException
{
    public IEnumerable<ValidationError> ValidationErrors { get; }

    public ValidationException(string message, IEnumerable<ValidationError> validationErrors)
        : base(message, "VALIDATION_ERROR")
    {
        ValidationErrors = validationErrors;
    }
}
