using Core.Models;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class RegionRepository : IRegionRepository
{
    private readonly ApplicationDbContext _context;

    public RegionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Region> CreateAsync(Region region)
    {
        _context.Regions.Add(region);
        await _context.SaveChangesAsync();
        return region;
    }

    public async Task<Region?> GetByIdAsync(int id)
    {
        return await _context.Regions
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<Region>> GetAllAsync()
    {
        return await _context.Regions
            .ToListAsync();
    }

    public async Task<Region> UpdateAsync(Region region)
    {
        region.UpdatedAt = DateTime.UtcNow;
        _context.Regions.Update(region);
        await _context.SaveChangesAsync();
        return region;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var region = await _context.Regions.FindAsync(id);
        if (region != null)
        {
            _context.Regions.Remove(region);
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }
}
