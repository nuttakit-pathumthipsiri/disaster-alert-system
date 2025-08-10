using Core.DTOs;
using Core.Models;

namespace Core.Services;

/// <summary>
/// Service for managing disaster alerts
/// </summary>
public interface IAlertService
{
    /// <summary>
    /// Sends a disaster alert to a region
    /// </summary>
    /// <param name="request">The alert request</param>
    /// <returns>The created alert</returns>
    Task<AlertResponse> SendAlertAsync(SendAlertRequest request);

    /// <summary>
    /// Gets all alerts from the database
    /// </summary>
    /// <returns>List of all alerts</returns>
    Task<IEnumerable<AlertResponse>> GetAllAlertsAsync();

    /// <summary>
    /// Gets alerts for a specific region
    /// </summary>
    /// <param name="regionId">The ID of the region</param>
    /// <returns>List of alerts for the region</returns>
    Task<IEnumerable<AlertResponse>> GetAlertsByRegionAsync(int regionId);

    /// <summary>
    /// Gets alerts by disaster type
    /// </summary>
    /// <param name="disasterTypeId">The ID of the disaster type</param>
    /// <returns>List of alerts for the disaster type</returns>
    Task<IEnumerable<AlertResponse>> GetAlertsByDisasterTypeAsync(int disasterTypeId);

    /// <summary>
    /// Gets alerts within a date range
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <returns>List of alerts within the date range</returns>
    Task<IEnumerable<AlertResponse>> GetAlertsByDateRangeAsync(DateTime startDate, DateTime endDate);
}
