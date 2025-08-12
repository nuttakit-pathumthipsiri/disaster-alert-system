namespace Core.DTOs;

/// <summary>
/// Response DTO for disaster risk report
/// </summary>
public class DisasterRiskReportResponse
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
    /// The ID of the disaster type
    /// </summary>
    public int DisasterTypeId { get; set; }

    /// <summary>
    /// The name of the disaster type
    /// </summary>
    public string DisasterTypeName { get; set; } = string.Empty;

    /// <summary>
    /// The calculated risk score (0.0 - 100.0)
    /// </summary>
    public double RiskScore { get; set; }

    /// <summary>
    /// The risk level based on the score (Low/Medium/High)
    /// </summary>
    public string RiskLevel { get; set; } = string.Empty;

    /// <summary>
    /// Whether an alert should be triggered based on threshold
    /// </summary>
    public bool AlertTriggered { get; set; }

    /// <summary>
    /// The threshold value from AlertSettings that determines alert triggering
    /// </summary>
    public double ThresholdValue { get; set; }

    /// <summary>
    /// Raw data from external APIs used in calculation
    /// </summary>
    public string? ExternalApiData { get; set; }

    /// <summary>
    /// The timestamp when this risk assessment was calculated
    /// </summary>
    public DateTime CalculatedAt { get; set; }

    /// <summary>
    /// The timestamp when this data expires (for caching)
    /// </summary>
    public DateTime ExpiresAt { get; set; }
}
