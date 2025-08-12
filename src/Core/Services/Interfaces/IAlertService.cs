using Core.DTOs;

namespace Core.Services;

public interface IAlertService
{
    Task<IEnumerable<AlertResponse>> GetAlertsAsync(int? regionId = null, int? disasterTypeId = null, bool pendingOnly = false);

    Task<IEnumerable<AlertResponse>> GetAllAlertsAsync();

    Task<IEnumerable<AlertResponse>> GetAlertsByRegionAsync(int regionId);

    Task<IEnumerable<AlertResponse>> GetAlertsByDisasterTypeAsync(int disasterTypeId);

    Task<IEnumerable<AlertResponse>> GetPendingAlertsAsync();

    Task<AlertResponse> SendAlertAsync(SendDisasterAlertRequest request);

    Task<AlertResponse> CreateAlertAsync(
        int regionId,
        int disasterTypeId,
        double riskScore,
        double thresholdValue,
        string? externalApiData = null);

    Task<IEnumerable<AlertResponse>> GetAlertHistoryAsync(
        int? regionId = null,
        int? disasterTypeId = null,
        DateTime? startDate = null,
        DateTime? endDate = null);
}
