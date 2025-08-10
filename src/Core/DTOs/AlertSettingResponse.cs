using Core.Models;

namespace Core.DTOs;

/// <summary>
/// Response model for alert settings
/// </summary>
public class AlertSettingResponse
{
    /// <summary>
    /// The unique identifier of the alert setting
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The ID of the region being monitored
    /// </summary>
    public int RegionId { get; set; }

    /// <summary>
    /// The name of the region being monitored
    /// </summary>
    public string RegionName { get; set; } = string.Empty;

    /// <summary>
    /// The ID of the disaster type being monitored
    /// </summary>
    public int DisasterTypeId { get; set; }

    /// <summary>
    /// The name of the disaster type being monitored
    /// </summary>
    public string DisasterTypeName { get; set; } = string.Empty;

    /// <summary>
    /// The threshold risk score that triggers an alert
    /// </summary>
    public double ThresholdRiskScore { get; set; }

    /// <summary>
    /// Whether this alert setting is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// When the alert setting was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the alert setting was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
