namespace Core.Models;

/// <summary>
/// Represents an alert that has been detected and stored in the database
/// </summary>
public class Alert
{
    /// <summary>
    /// The unique identifier of the risk alert
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The ID of the region where the risk was detected
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
    /// The calculated risk score that triggered the alert
    /// </summary>
    public double RiskScore { get; set; }

    /// <summary>
    /// The risk level of the disaster (Low, Medium, High)
    /// </summary>
    public RiskLevel RiskLevel { get; set; }

    /// <summary>
    /// The threshold value that was exceeded
    /// </summary>
    public double ThresholdValue { get; set; }

    /// <summary>
    /// Flag indicating whether email has been sent for this alert
    /// </summary>
    public bool EmailSent { get; set; }

    /// <summary>
    /// Timestamp when the email was sent (null if not sent yet)
    /// </summary>
    public DateTime? EmailSentAt { get; set; }

    /// <summary>
    /// The message content of the alert
    /// </summary>
    public string AlertMessage { get; set; } = string.Empty;

    /// <summary>
    /// External API data that was used for risk calculation
    /// </summary>
    public string? ExternalApiData { get; set; }

    /// <summary>
    /// The timestamp when the risk was detected
    /// </summary>
    public DateTime DetectedAt { get; set; }

    /// <summary>
    /// The timestamp when the risk alert expires
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Additional metadata about the alert
    /// </summary>
    public string? Metadata { get; set; }
}
