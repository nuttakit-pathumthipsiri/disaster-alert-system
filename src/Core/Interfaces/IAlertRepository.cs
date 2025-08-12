using Core.Models;

namespace Core.Interfaces;

public interface IAlertRepository
{
    Task<Alert> CreateAsync(Alert alert);
    Task<Alert?> GetByIdAsync(int id);
    Task<IEnumerable<Alert>> GetAllAsync();
    Task<IEnumerable<Alert>> GetByRegionAsync(int regionId);
    Task<IEnumerable<Alert>> GetByDisasterTypeAsync(int disasterTypeId);
    Task<IEnumerable<Alert>> GetPendingAlertsAsync();
    Task<IEnumerable<Alert>> GetByFiltersAsync(int? regionId = null, int? disasterTypeId = null, bool pendingOnly = false);
    Task<IEnumerable<Alert>> GetAlertHistoryAsync(int? regionId = null, int? disasterTypeId = null, DateTime? startDate = null, DateTime? endDate = null);
    Task<Alert> UpdateAsync(Alert alert);
    Task<bool> DeleteAsync(int id);
}
