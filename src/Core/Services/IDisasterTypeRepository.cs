using Core.Models;

namespace Core.Services;

/// <summary>
/// Repository interface for managing disaster types
/// </summary>
public interface IDisasterTypeRepository
{
    /// <summary>
    /// Gets all active disaster types
    /// </summary>
    /// <returns>Collection of active disaster types</returns>
    Task<IEnumerable<DisasterType>> GetActiveDisasterTypesAsync();

    /// <summary>
    /// Gets a disaster type by ID
    /// </summary>
    /// <param name="id">The disaster type ID</param>
    /// <returns>The disaster type or null if not found</returns>
    Task<DisasterType?> GetByIdAsync(int id);

    /// <summary>
    /// Gets a disaster type by name
    /// </summary>
    /// <param name="name">The disaster type name</param>
    /// <returns>The disaster type or null if not found</returns>
    Task<DisasterType?> GetByNameAsync(string name);
}
