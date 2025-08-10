using Core.Models;

namespace Core.DTOs;

/// <summary>
/// Response model for alerts
/// </summary>
public class AlertResponse
{
    /// <summary>
    /// The unique identifier of the alert
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The ID of the region where the alert was sent
    /// </summary>
    public int RegionId { get; set; }

    /// <summary>
    /// The name of the region where the alert was sent
    /// </summary>
    public string RegionName { get; set; } = string.Empty;

    /// <summary>
    /// The ID of the disaster type that triggered the alert
    /// </summary>
    public int DisasterTypeId { get; set; }

    /// <summary>
    /// The name of the disaster type that triggered the alert
    /// </summary>
    public string DisasterTypeName { get; set; } = string.Empty;

    /// <summary>
    /// The risk level of the disaster
    /// </summary>
    public RiskLevel RiskLevel { get; set; }

    /// <summary>
    /// The calculated risk score
    /// </summary>
    public double RiskScore { get; set; }

    /// <summary>
    /// The message content of the alert
    /// </summary>
    public string AlertMessage { get; set; } = string.Empty;

    /// <summary>
    /// The status of the alert
    /// </summary>
    public AlertStatus Status { get; set; }

    /// <summary>
    /// When the alert was sent
    /// </summary>
    public DateTime SentAt { get; set; }

    /// <summary>
    /// Additional metadata about the alert
    /// </summary>
    public string? Metadata { get; set; }
}
