using Core.Models;
using System.ComponentModel.DataAnnotations;

namespace Core.DTOs;

/// <summary>
/// Request DTO for creating a new disaster alert region
/// </summary>
public class CreateRegionRequest
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
    [Required(ErrorMessage = "Latitude is required")]
    [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
    public double Latitude { get; set; }

    /// <summary>
    /// The longitude coordinate of the region center
    /// </summary>
    [Required(ErrorMessage = "Longitude is required")]
    [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
    public double Longitude { get; set; }

    /// <summary>
    /// List of disaster type IDs to monitor in this region
    /// </summary>
    [Required(ErrorMessage = "At least one disaster type must be specified")]
    public List<int> MonitoredDisasterTypes { get; set; } = new();
}
