using System.ComponentModel.DataAnnotations;
using Core.Models;

namespace Core.DTOs;

/// <summary>
/// Request DTO for updating a disaster alert region
/// </summary>
public class UpdateRegionRequest
{
    /// <summary>
    /// The name of the region
    /// </summary>
    [Required(ErrorMessage = "Region name is required")]
    [StringLength(100, ErrorMessage = "Region name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The latitude coordinate of the region center
    /// </summary>
    [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
    public double? Latitude { get; set; }

    /// <summary>
    /// The longitude coordinate of the region center
    /// </summary>
    [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 90")]
    public double? Longitude { get; set; }

    /// <summary>
    /// List of disaster type IDs to monitor in this region
    /// </summary>
    public List<int>? MonitoredDisasterTypeIds { get; set; }
}
