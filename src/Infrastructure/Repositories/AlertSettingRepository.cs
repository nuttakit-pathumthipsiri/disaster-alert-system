using Core.Models;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class AlertSettingRepository : IAlertSettingRepository
{
    private readonly ApplicationDbContext _context;

    public AlertSettingRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AlertSetting> CreateAsync(AlertSetting alertSetting)
    {
        _context.AlertSettings.Add(alertSetting);
        await _context.SaveChangesAsync();
        return alertSetting;
    }

    public async Task<AlertSetting?> GetByIdAsync(int id)
    {
        return await _context.AlertSettings
            .Include(a => a.Region)
            .Include(a => a.DisasterType)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<AlertSetting>> GetAllAsync()
    {
        return await _context.AlertSettings
            .Include(a => a.Region)
            .Include(a => a.DisasterType)
            .Where(a => a.IsActive)
            .OrderBy(a => a.RegionId)
            .ThenBy(a => a.DisasterTypeId)
            .ToListAsync();
    }

    public async Task<AlertSetting> UpdateAsync(AlertSetting alertSetting)
    {
        _context.AlertSettings.Update(alertSetting);
        await _context.SaveChangesAsync();
        return alertSetting;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var alertSetting = await _context.AlertSettings.FindAsync(id);
        if (alertSetting == null)
            return false;

        _context.AlertSettings.Remove(alertSetting);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<AlertSetting?> GetByRegionAndDisasterTypeAsync(int regionId, int disasterTypeId)
    {
        return await _context.AlertSettings
            .Include(a => a.Region)
            .Include(a => a.DisasterType)
            .FirstOrDefaultAsync(a => a.RegionId == regionId && a.DisasterTypeId == disasterTypeId && a.IsActive);
    }

    public async Task<IEnumerable<AlertSetting>> GetByRegionAsync(int regionId)
    {
        return await _context.AlertSettings
            .Include(a => a.Region)
            .Include(a => a.DisasterType)
            .Where(a => a.RegionId == regionId && a.IsActive)
            .OrderBy(a => a.DisasterTypeId)
            .ToListAsync();
    }

    public async Task<IEnumerable<AlertSetting>> GetByDisasterTypeAsync(int disasterTypeId)
    {
        return await _context.AlertSettings
            .Include(a => a.Region)
            .Include(a => a.DisasterType)
            .Where(a => a.DisasterTypeId == disasterTypeId && a.IsActive)
            .OrderBy(a => a.RegionId)
            .ToListAsync();
    }
}
