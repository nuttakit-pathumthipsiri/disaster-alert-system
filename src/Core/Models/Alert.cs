namespace Core.Models;

/// <summary>
/// Represents a disaster alert that has been sent to a region
/// </summary>
public class Alert
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
    /// The ID of the disaster type that triggered the alert
    /// </summary>
    public int DisasterTypeId { get; set; }

    /// <summary>
    /// Navigation property to the region
    /// </summary>
    public virtual Region Region { get; set; } = null!;

    /// <summary>
    /// Navigation property to the disaster type
    /// </summary>
    public virtual DisasterType DisasterType { get; set; } = null!;

    /// <summary>
    /// The risk level of the disaster (Low, Medium, High)
    /// </summary>
    public RiskLevel RiskLevel { get; set; }

    /// <summary>
    /// The calculated risk score that triggered the alert
    /// </summary>
    public double RiskScore { get; set; }

    /// <summary>
    /// The message content of the alert
    /// </summary>
    public string AlertMessage { get; set; } = string.Empty;

    /// <summary>
    /// The timestamp when the alert was sent
    /// </summary>
    public DateTime SentAt { get; set; }

    /// <summary>
    /// The status of the alert (Sent, Delivered, Failed)
    /// </summary>
    public AlertStatus Status { get; set; }

    /// <summary>
    /// Additional metadata about the alert (e.g., external API response)
    /// </summary>
    public string? Metadata { get; set; }
}

/// <summary>
/// Risk levels for disaster assessment
/// </summary>
public enum RiskLevel
{
    /// <summary>
    /// Low risk - minimal threat
    /// </summary>
    Low,

    /// <summary>
    /// Medium risk - moderate threat
    /// </summary>
    Medium,

    /// <summary>
    /// High risk - significant threat
    /// </summary>
    High
}

/// <summary>
/// Status of alert delivery
/// </summary>
public enum AlertStatus
{
    /// <summary>
    /// Alert has been created and queued for sending
    /// </summary>
    Pending,

    /// <summary>
    /// Alert has been sent successfully
    /// </summary>
    Sent,

    /// <summary>
    /// Alert has been delivered to the recipient
    /// </summary>
    Delivered,

    /// <summary>
    /// Alert failed to send
    /// </summary>
    Failed
}
