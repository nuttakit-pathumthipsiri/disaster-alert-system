using Core.DTOs;
using Core.Models;

namespace Core.Services;

public interface IRegionService
{
    Task<RegionResponse> CreateRegionAsync(CreateRegionRequest request);
    Task<RegionResponse?> GetRegionAsync(int id);
    Task<IEnumerable<RegionResponse>> GetAllRegionsAsync();
    Task<RegionResponse?> UpdateRegionAsync(int id, UpdateRegionRequest request);
    Task<bool> DeleteRegionAsync(int id);
}
