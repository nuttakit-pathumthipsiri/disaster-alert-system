using Core.Models;

namespace Core.DTOs;

/// <summary>
/// Response DTO for disaster alert region data
/// </summary>
public class RegionResponse
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
    /// List of disaster types monitored in this region
    /// </summary>
    public List<DisasterTypeInfo> MonitoredDisasterTypes { get; set; } = new();

    /// <summary>
    /// The timestamp when the region was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// The timestamp when the region was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Simplified disaster type information for responses
/// </summary>
public class DisasterTypeInfo
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
