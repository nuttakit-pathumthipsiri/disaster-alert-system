using Core.Models;

namespace Core.Interfaces;

public interface IDisasterTypeRepository
{
    Task<DisasterType> CreateAsync(DisasterType disasterType);
    Task<DisasterType?> GetByIdAsync(int id);
    Task<IEnumerable<DisasterType>> GetAllAsync();
    Task<IEnumerable<DisasterType>> GetActiveDisasterTypesAsync();
    Task<DisasterType> UpdateAsync(DisasterType disasterType);
    Task<bool> DeleteAsync(int id);
}
