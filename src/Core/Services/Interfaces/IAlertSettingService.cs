using Core.DTOs;
using Core.Models;

namespace Core.Services;

public interface IAlertSettingService
{
    Task<AlertSettingResponse> CreateAlertSettingAsync(CreateAlertSettingRequest request);

    Task<IEnumerable<AlertSettingResponse>> GetAllAlertSettingsAsync();

    Task<IEnumerable<AlertSettingResponse>> GetAlertSettingsByRegionAsync(int regionId);

    Task<IEnumerable<AlertSettingResponse>> GetAlertSettingsByDisasterTypeAsync(int disasterTypeId);

    Task<AlertSettingResponse> UpdateAlertSettingAsync(int id, CreateAlertSettingRequest request);

    Task<bool> DeleteAlertSettingAsync(int id);

    Task<double?> GetThresholdAsync(int regionId, DisasterType disasterType);
}
