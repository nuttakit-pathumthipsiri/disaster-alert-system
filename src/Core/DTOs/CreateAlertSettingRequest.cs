using Core.Models;

namespace Core.DTOs;

/// <summary>
/// Request model for creating alert settings
/// </summary>
public class CreateAlertSettingRequest
{
    /// <summary>
    /// The ID of the region to monitor
    /// </summary>
    public int RegionId { get; set; }

    /// <summary>
    /// The ID of the disaster type to monitor
    /// </summary>
    public int DisasterTypeId { get; set; }

    /// <summary>
    /// The threshold risk score that triggers an alert
    /// </summary>
    public double ThresholdRiskScore { get; set; }

    /// <summary>
    /// Whether this alert setting is active
    /// </summary>
    public bool IsActive { get; set; } = true;
}
