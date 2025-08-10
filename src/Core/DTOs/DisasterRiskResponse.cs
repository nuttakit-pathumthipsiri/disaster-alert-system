using Core.Models;

namespace Core.DTOs;

/// <summary>
/// Response model for disaster risk assessment
/// </summary>
public class DisasterRiskResponse
{
    /// <summary>
    /// The ID of the region
    /// </summary>
    public int RegionId { get; set; }

    /// <summary>
    /// The name of the region
    /// </summary>
    public string RegionName { get; set; } = string.Empty;

    /// <summary>
    /// The ID of the disaster type being assessed
    /// </summary>
    public int DisasterTypeId { get; set; }

    /// <summary>
    /// The name of the disaster type being assessed
    /// </summary>
    public string DisasterTypeName { get; set; } = string.Empty;

    /// <summary>
    /// The calculated risk score (0-100)
    /// </summary>
    public double RiskScore { get; set; }

    /// <summary>
    /// The risk level based on the score
    /// </summary>
    public RiskLevel RiskLevel { get; set; }

    /// <summary>
    /// Whether an alert should be triggered based on the risk assessment
    /// </summary>
    public bool AlertTriggered { get; set; }

    /// <summary>
    /// The threshold value that triggers an alert
    /// </summary>
    public double ThresholdValue { get; set; }

    /// <summary>
    /// When the risk assessment was calculated
    /// </summary>
    public DateTime CalculatedAt { get; set; }

    /// <summary>
    /// Additional details about the risk assessment
    /// </summary>
    public string Details { get; set; } = string.Empty;
}
