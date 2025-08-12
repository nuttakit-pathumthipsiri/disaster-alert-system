using System.Text.Json.Serialization;

namespace Core.Models;

/// <summary>
/// Represents a geographical region for disaster monitoring and alerting
/// </summary>
public class Region
{
    /// <summary>
    /// The unique identifier of the region
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The name of the region
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The latitude coordinate of the region center
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    /// The longitude coordinate of the region center
    /// </summary>
    public double Longitude { get; set; }

    /// <summary>
    /// JSON string of monitored disaster types (for database storage)
    /// </summary>
    public string MonitoredDisasterTypes { get; set; } = "[]";

    /// <summary>
    /// List of disaster types monitored in this region (navigation property)
    /// </summary>
    [JsonIgnore]
    public virtual ICollection<DisasterType> DisasterTypes { get; set; } = new List<DisasterType>();

    /// <summary>
    /// The timestamp when the region was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// The timestamp when the region was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    [JsonIgnore]
    public virtual ICollection<AlertSetting> AlertSettings { get; set; } = new List<AlertSetting>();

}
