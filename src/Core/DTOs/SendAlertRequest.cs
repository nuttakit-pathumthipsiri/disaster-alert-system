using Core.Models;

namespace Core.DTOs;

/// <summary>
/// Request model for sending alerts
/// </summary>
public class SendAlertRequest
{
    /// <summary>
    /// The ID of the region to send the alert to
    /// </summary>
    public int RegionId { get; set; }

    /// <summary>
    /// The ID of the disaster type that triggered the alert
    /// </summary>
    public int DisasterTypeId { get; set; }

    /// <summary>
    /// The calculated risk score
    /// </summary>
    public double RiskScore { get; set; }

    /// <summary>
    /// The message content of the alert
    /// </summary>
    public string AlertMessage { get; set; } = string.Empty;

    /// <summary>
    /// Additional metadata about the alert
    /// </summary>
    public string? Metadata { get; set; }
}
