using Core.DTOs;

namespace Core.Services;

public interface IDisasterRisksService
{
    Task<IEnumerable<DisasterRiskReportResponse>> GetDisasterRiskReportsAsync();

    Task<DisasterRiskReportResponse?> GetDisasterRiskReportAsync(int regionId, int disasterTypeId);
}
