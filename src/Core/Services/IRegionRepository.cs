using Core.Models;

namespace Core.Services;

public interface IRegionRepository
{
    Task<Region> CreateAsync(Region region);
    Task<Region?> GetByIdAsync(int id);
    Task<IEnumerable<Region>> GetAllAsync();
    Task<Region> UpdateAsync(Region region);
    Task<bool> DeleteAsync(int id);
}
