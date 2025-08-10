namespace Core.Models;

/// <summary>
/// Represents a type of disaster that can be monitored and alerted
/// </summary>
public class DisasterType
{
    /// <summary>
    /// The unique identifier of the disaster type
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The English name of the disaster type
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Whether this disaster type is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// The timestamp when the disaster type was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// The timestamp when the disaster type was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual ICollection<Region> Regions { get; set; } = new List<Region>();
    public virtual ICollection<AlertSetting> AlertSettings { get; set; } = new List<AlertSetting>();
    public virtual ICollection<DisasterRisk> DisasterRisks { get; set; } = new List<DisasterRisk>();
}
