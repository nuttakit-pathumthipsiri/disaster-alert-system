using Core.DTOs;
using Core.Models;

namespace Core.Services;

/// <summary>
/// Service for managing alert settings
/// </summary>
public interface IAlertSettingService
{
    /// <summary>
    /// Creates a new alert setting
    /// </summary>
    /// <param name="request">The alert setting request</param>
    /// <returns>The created alert setting</returns>
    Task<AlertSettingResponse> CreateAlertSettingAsync(CreateAlertSettingRequest request);

    /// <summary>
    /// Gets all alert settings
    /// </summary>
    /// <returns>List of all alert settings</returns>
    Task<IEnumerable<AlertSettingResponse>> GetAllAlertSettingsAsync();

    /// <summary>
    /// Gets alert settings for a specific region
    /// </summary>
    /// <param name="regionId">The ID of the region</param>
    /// <returns>List of alert settings for the region</returns>
    Task<IEnumerable<AlertSettingResponse>> GetAlertSettingsByRegionAsync(int regionId);

    /// <summary>
    /// Gets alert settings by disaster type
    /// </summary>
    /// <param name="disasterTypeId">The ID of the disaster type</param>
    /// <returns>List of alert settings for the disaster type</returns>
    Task<IEnumerable<AlertSettingResponse>> GetAlertSettingsByDisasterTypeAsync(int disasterTypeId);

    /// <summary>
    /// Updates an existing alert setting
    /// </summary>
    /// <param name="id">The ID of the alert setting to update</param>
    /// <param name="request">The updated alert setting data</param>
    /// <returns>The updated alert setting</returns>
    Task<AlertSettingResponse> UpdateAlertSettingAsync(int id, CreateAlertSettingRequest request);

    /// <summary>
    /// Deletes an alert setting
    /// </summary>
    /// <param name="id">The ID of the alert setting to delete</param>
    /// <returns>True if deleted successfully</returns>
    Task<bool> DeleteAlertSettingAsync(int id);

    /// <summary>
    /// Gets the threshold value for a specific region and disaster type
    /// </summary>
    /// <param name="regionId">The ID of the region</param>
    /// <param name="disasterType">The type of disaster</param>
    /// <returns>The threshold value or null if not found</returns>
    Task<double?> GetThresholdAsync(int regionId, DisasterType disasterType);
}
