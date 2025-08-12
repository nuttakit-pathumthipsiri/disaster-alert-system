using Core.Models;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

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
            .Where(dt => dt.Id == id && dt.IsActive)
            .FirstOrDefaultAsync();
    }

    public async Task<DisasterType?> GetByNameAsync(string name)
    {
        return await _context.DisasterTypes
            .Where(dt => dt.Name == name && dt.IsActive)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<DisasterType>> GetAllDisasterTypesAsync()
    {
        return await _context.DisasterTypes
            .OrderBy(dt => dt.Name)
            .ToListAsync();
    }

    public async Task<DisasterType?> GetByIdIncludeInactiveAsync(int id)
    {
        return await _context.DisasterTypes
            .FirstOrDefaultAsync(dt => dt.Id == id);
    }

    public async Task<DisasterType?> GetByNameIncludeInactiveAsync(string name)
    {
        return await _context.DisasterTypes
            .FirstOrDefaultAsync(dt => dt.Name == name);
    }

    public async Task<DisasterType> CreateAsync(DisasterType disasterType)
    {
        _context.DisasterTypes.Add(disasterType);
        await _context.SaveChangesAsync();
        return disasterType;
    }

    public async Task<IEnumerable<DisasterType>> GetAllAsync()
    {
        return await _context.DisasterTypes
            .OrderBy(dt => dt.Name)
            .ToListAsync();
    }

    public async Task<DisasterType> UpdateAsync(DisasterType disasterType)
    {
        _context.DisasterTypes.Update(disasterType);
        await _context.SaveChangesAsync();
        return disasterType;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var disasterType = await _context.DisasterTypes.FindAsync(id);
        if (disasterType == null)
            return false;

        _context.DisasterTypes.Remove(disasterType);
        await _context.SaveChangesAsync();
        return true;
    }
}
