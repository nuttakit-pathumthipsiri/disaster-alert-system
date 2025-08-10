using Core.DTOs;
using Core.Models;
using Core.Services;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

/// <summary>
/// Implementation of alert setting service for managing disaster alert configurations
/// </summary>
public class AlertSettingService : IAlertSettingService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AlertSettingService> _logger;

    public AlertSettingService(ApplicationDbContext context, ILogger<AlertSettingService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<AlertSettingResponse> CreateAlertSettingAsync(CreateAlertSettingRequest request)
    {
        try
        {
            _logger.LogInformation("Creating alert setting for region {RegionId} and disaster type {DisasterTypeId}",
                request.RegionId, request.DisasterTypeId);

            // Check if region exists
            var region = await _context.Regions.FindAsync(request.RegionId);
            if (region == null)
                throw new InvalidOperationException($"Region with ID {request.RegionId} not found");

            // Check if disaster type exists
            var disasterType = await _context.DisasterTypes.FindAsync(request.DisasterTypeId);
            if (disasterType == null)
                throw new InvalidOperationException($"Disaster type with ID {request.DisasterTypeId} not found");

            // Check if alert setting already exists for this region and disaster type
            var existingSetting = await _context.AlertSettings
                .FirstOrDefaultAsync(a => a.RegionId == request.RegionId && a.DisasterTypeId == request.DisasterTypeId);

            if (existingSetting != null)
                throw new InvalidOperationException($"Alert setting already exists for region {request.RegionId} and disaster type {request.DisasterTypeId}");

            var alertSetting = new AlertSetting
            {
                RegionId = request.RegionId,
                DisasterTypeId = request.DisasterTypeId,
                ThresholdRiskScore = request.ThresholdRiskScore,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _context.AlertSettings.Add(alertSetting);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully created alert setting with ID {AlertSettingId}", alertSetting.Id);

            return await MapToResponseAsync(alertSetting);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create alert setting for region {RegionId}", request.RegionId);
            throw;
        }
    }

    public async Task<IEnumerable<AlertSettingResponse>> GetAllAlertSettingsAsync()
    {
        try
        {
            var alertSettings = await _context.AlertSettings
                .Include(a => a.Region)
                .Include(a => a.DisasterType)
                .Where(a => a.IsActive)
                .OrderBy(a => a.RegionId)
                .ThenBy(a => a.DisasterTypeId)
                .ToListAsync();

            var responses = new List<AlertSettingResponse>();
            foreach (var alertSetting in alertSettings)
            {
                responses.Add(await MapToResponseAsync(alertSetting));
            }

            return responses;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve all alert settings");
            throw;
        }
    }

    public async Task<IEnumerable<AlertSettingResponse>> GetAlertSettingsByRegionAsync(int regionId)
    {
        try
        {
            var alertSettings = await _context.AlertSettings
                .Include(a => a.Region)
                .Include(a => a.DisasterType)
                .Where(a => a.RegionId == regionId && a.IsActive)
                .OrderBy(a => a.DisasterTypeId)
                .ToListAsync();

            var responses = new List<AlertSettingResponse>();
            foreach (var alertSetting in alertSettings)
            {
                responses.Add(await MapToResponseAsync(alertSetting));
            }

            return responses;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve alert settings for region {RegionId}", regionId);
            throw;
        }
    }

    public async Task<IEnumerable<AlertSettingResponse>> GetAlertSettingsByDisasterTypeAsync(int disasterTypeId)
    {
        try
        {
            var alertSettings = await _context.AlertSettings
                .Include(a => a.Region)
                .Include(a => a.DisasterType)
                .Where(a => a.DisasterTypeId == disasterTypeId && a.IsActive)
                .OrderBy(a => a.RegionId)
                .ToListAsync();

            var responses = new List<AlertSettingResponse>();
            foreach (var alertSetting in alertSettings)
            {
                responses.Add(await MapToResponseAsync(alertSetting));
            }

            return responses;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve alert settings for disaster type {DisasterTypeId}", disasterTypeId);
            throw;
        }
    }

    public async Task<AlertSettingResponse> UpdateAlertSettingAsync(int id, CreateAlertSettingRequest request)
    {
        try
        {
            _logger.LogInformation("Updating alert setting with ID {AlertSettingId}", id);

            var alertSetting = await _context.AlertSettings.FindAsync(id);
            if (alertSetting == null)
                throw new InvalidOperationException($"Alert setting with ID {id} not found");

            // Check if region exists
            var region = await _context.Regions.FindAsync(request.RegionId);
            if (region == null)
                throw new InvalidOperationException($"Region with ID {request.RegionId} not found");

            // Check if another alert setting already exists for this region and disaster type
            var existingSetting = await _context.AlertSettings
                .FirstOrDefaultAsync(a => a.RegionId == request.RegionId &&
                                        a.DisasterTypeId == request.DisasterTypeId &&
                                        a.Id != id);

            if (existingSetting != null)
                throw new InvalidOperationException($"Alert setting already exists for region {request.RegionId} and disaster type {request.DisasterTypeId}");

            alertSetting.RegionId = request.RegionId;
            alertSetting.DisasterTypeId = request.DisasterTypeId;
            alertSetting.ThresholdRiskScore = request.ThresholdRiskScore;
            alertSetting.IsActive = request.IsActive;
            alertSetting.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully updated alert setting with ID {AlertSettingId}", id);

            return await MapToResponseAsync(alertSetting);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update alert setting with ID {AlertSettingId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteAlertSettingAsync(int id)
    {
        try
        {
            _logger.LogInformation("Deleting alert setting with ID {AlertSettingId}", id);

            var alertSetting = await _context.AlertSettings.FindAsync(id);
            if (alertSetting == null)
                return false;

            _context.AlertSettings.Remove(alertSetting);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully deleted alert setting with ID {AlertSettingId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete alert setting with ID {AlertSettingId}", id);
            throw;
        }
    }

    public async Task<double?> GetThresholdAsync(int regionId, DisasterType disasterType)
    {
        try
        {
            var alertSetting = await _context.AlertSettings
                .Where(a => a.RegionId == regionId && a.DisasterTypeId == disasterType.Id && a.IsActive)
                .Select(a => a.ThresholdRiskScore)
                .FirstOrDefaultAsync();

            return alertSetting;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get threshold for region {RegionId} and disaster type {DisasterTypeId}", regionId, disasterType.Id);
            return null;
        }
    }

    private async Task<AlertSettingResponse> MapToResponseAsync(AlertSetting alertSetting)
    {
        var region = await _context.Regions.FindAsync(alertSetting.RegionId);
        var disasterType = await _context.DisasterTypes.FindAsync(alertSetting.DisasterTypeId);

        return new AlertSettingResponse
        {
            Id = alertSetting.Id,
            RegionId = alertSetting.RegionId,
            RegionName = region?.Name ?? $"Region {alertSetting.RegionId}",
            DisasterTypeId = alertSetting.DisasterTypeId,
            DisasterTypeName = disasterType?.Name ?? $"DisasterType {alertSetting.DisasterTypeId}",
            ThresholdRiskScore = alertSetting.ThresholdRiskScore,
            IsActive = alertSetting.IsActive,
            CreatedAt = alertSetting.CreatedAt,
            UpdatedAt = alertSetting.UpdatedAt
        };
    }
}
