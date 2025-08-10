using Core.DTOs;
using Core.Models;

namespace Core.Services;

/// <summary>
/// Service for calculating disaster risks and managing risk assessments
/// </summary>
public interface IDisasterRiskService
{
    /// <summary>
    /// Calculates disaster risks for all active regions
    /// </summary>
    /// <returns>List of disaster risk assessments</returns>
    Task<IEnumerable<DisasterRiskResponse>> CalculateAllDisasterRisksAsync();

    /// <summary>
    /// Calculates disaster risk for a specific region and disaster type
    /// </summary>
    /// <param name="regionId">The ID of the region</param>
    /// <param name="disasterType">The type of disaster to assess</param>
    /// <returns>Disaster risk assessment</returns>
    Task<DisasterRiskResponse> CalculateDisasterRiskAsync(int regionId, DisasterType disasterType);

    /// <summary>
    /// Gets cached disaster risk data for a region
    /// </summary>
    /// <param name="regionId">The ID of the region</param>
    /// <param name="disasterType">The type of disaster</param>
    /// <returns>Cached disaster risk data or null if expired</returns>
    Task<DisasterRisk?> GetCachedDisasterRiskAsync(int regionId, DisasterType disasterType);

    /// <summary>
    /// Stores disaster risk data in cache
    /// </summary>
    /// <param name="disasterRisk">The disaster risk data to cache</param>
    /// <returns>Task completion</returns>
    Task CacheDisasterRiskAsync(DisasterRisk disasterRisk);
}
