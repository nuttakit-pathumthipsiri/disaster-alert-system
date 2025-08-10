namespace Core.Models;

/// <summary>
/// Represents a disaster risk assessment for a specific region and disaster type
/// </summary>
public class DisasterRisk
{
    /// <summary>
    /// The unique identifier of the risk assessment
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The ID of the region being assessed
    /// </summary>
    public int RegionId { get; set; }

    /// <summary>
    /// The ID of the disaster type being assessed
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
    /// The calculated risk score (0.0 - 100.0)
    /// </summary>
    public double RiskScore { get; set; }

    /// <summary>
    /// The risk level based on the score
    /// </summary>
    public RiskLevel RiskLevel { get; set; }

    /// <summary>
    /// Whether an alert should be triggered based on threshold
    /// </summary>
    public bool ShouldTriggerAlert { get; set; }

    /// <summary>
    /// The threshold value that determines alert triggering
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
