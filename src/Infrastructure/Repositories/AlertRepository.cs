using Core.Models;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class AlertRepository : IAlertRepository
{
    private readonly ApplicationDbContext _context;

    public AlertRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Alert> CreateAsync(Alert alert)
    {
        _context.Alerts.Add(alert);
        await _context.SaveChangesAsync();
        return alert;
    }

    public async Task<Alert?> GetByIdAsync(int id)
    {
        return await _context.Alerts
            .Include(a => a.Region)
            .Include(a => a.DisasterType)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<Alert>> GetAllAsync()
    {
        return await _context.Alerts
            .Include(a => a.Region)
            .Include(a => a.DisasterType)
            .OrderByDescending(a => a.DetectedAt)
            .ToListAsync();
    }

    public async Task<Alert> UpdateAsync(Alert alert)
    {
        _context.Alerts.Update(alert);
        await _context.SaveChangesAsync();
        return alert;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var alert = await _context.Alerts.FindAsync(id);
        if (alert == null)
            return false;

        _context.Alerts.Remove(alert);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Alert>> GetByRegionAsync(int regionId)
    {
        return await _context.Alerts
            .Include(a => a.Region)
            .Include(a => a.DisasterType)
            .Where(a => a.RegionId == regionId)
            .OrderByDescending(a => a.DetectedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Alert>> GetByDisasterTypeAsync(int disasterTypeId)
    {
        return await _context.Alerts
            .Include(a => a.Region)
            .Include(a => a.DisasterType)
            .Where(a => a.DisasterTypeId == disasterTypeId)
            .OrderByDescending(a => a.DetectedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Alert>> GetPendingAlertsAsync()
    {
        return await _context.Alerts
            .Include(a => a.Region)
            .Include(a => a.DisasterType)
            .Where(a => !a.EmailSent && a.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(a => a.RiskScore)
            .ThenBy(a => a.DetectedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Alert>> GetByFiltersAsync(int? regionId = null, int? disasterTypeId = null, bool pendingOnly = false)
    {
        var query = _context.Alerts
            .Include(a => a.Region)
            .Include(a => a.DisasterType)
            .AsQueryable();

        // Apply filters
        if (pendingOnly)
        {
            query = query.Where(a => !a.EmailSent && a.ExpiresAt > DateTime.UtcNow);
        }

        if (regionId.HasValue)
        {
            query = query.Where(a => a.RegionId == regionId.Value);
        }

        if (disasterTypeId.HasValue)
        {
            query = query.Where(a => a.DisasterTypeId == disasterTypeId.Value);
        }

        // Apply ordering
        if (pendingOnly)
        {
            query = query.OrderByDescending(a => a.RiskScore).ThenBy(a => a.DetectedAt);
        }
        else
        {
            query = query.OrderByDescending(a => a.DetectedAt);
        }

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Alert>> GetAlertHistoryAsync(int? regionId = null, int? disasterTypeId = null, DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.Alerts
            .Include(a => a.Region)
            .Include(a => a.DisasterType)
            .Where(a => a.EmailSent) // Only alerts that have been sent
            .AsQueryable();

        // Apply filters
        if (regionId.HasValue)
        {
            query = query.Where(a => a.RegionId == regionId.Value);
        }

        if (disasterTypeId.HasValue)
        {
            query = query.Where(a => a.DisasterTypeId == disasterTypeId.Value);
        }

        if (startDate.HasValue)
        {
            query = query.Where(a => a.DetectedAt >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(a => a.DetectedAt <= endDate.Value);
        }

        return await query
            .OrderByDescending(a => a.DetectedAt)
            .ToListAsync();
    }
}
