using Core.Models;
using Core.Services;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// Repository implementation for managing disaster types
/// </summary>
public class DisasterTypeRepository : IDisasterTypeRepository
{
    private readonly ApplicationDbContext _context;

    public DisasterTypeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<DisasterType>> GetActiveDisasterTypesAsync()
    {
        return await _context.DisasterTypes
            .Where(dt => dt.IsActive)
            .OrderBy(dt => dt.Name)
            .ToListAsync();
    }

    public async Task<DisasterType?> GetByIdAsync(int id)
    {
        return await _context.DisasterTypes
            .FirstOrDefaultAsync(dt => dt.Id == id);
    }

    public async Task<DisasterType?> GetByNameAsync(string name)
    {
        return await _context.DisasterTypes
            .FirstOrDefaultAsync(dt => dt.Name == name);
    }
}
