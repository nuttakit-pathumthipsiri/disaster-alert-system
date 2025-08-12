using Core.Models;

namespace Core.Interfaces;

public interface IAlertSettingRepository
{
    Task<AlertSetting> CreateAsync(AlertSetting alertSetting);
    Task<AlertSetting?> GetByIdAsync(int id);
    Task<IEnumerable<AlertSetting>> GetAllAsync();
    Task<IEnumerable<AlertSetting>> GetByRegionAsync(int regionId);
    Task<IEnumerable<AlertSetting>> GetByDisasterTypeAsync(int disasterTypeId);
    Task<AlertSetting?> GetByRegionAndDisasterTypeAsync(int regionId, int disasterTypeId);
    Task<AlertSetting> UpdateAsync(AlertSetting alertSetting);
    Task<bool> DeleteAsync(int id);
}
