namespace Core.DTOs;

/// <summary>
/// Request DTO for sending disaster alerts to high-risk areas
/// </summary>
public class SendDisasterAlertRequest
{
    /// <summary>
    /// The ID of the region to send the alert to (required)
    /// </summary>
    public int RegionId { get; set; }

    /// <summary>
    /// The ID of the disaster type (optional)
    /// </summary>
    public int? DisasterTypeId { get; set; }
}
