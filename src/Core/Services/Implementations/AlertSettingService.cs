using Core.DTOs;
using Core.Models;
using Core.Interfaces;
using Core.Services;
using Microsoft.Extensions.Logging;

namespace Core.Services.Implementations;

public class AlertSettingService : IAlertSettingService
{
    private readonly ILogger<AlertSettingService> _logger;
    private readonly IDisasterTypeRepository _disasterTypeRepository;
    private readonly IRegionRepository _regionRepository;
    private readonly IAlertSettingRepository _alertSettingRepository;

    public AlertSettingService(
        ILogger<AlertSettingService> logger,
        IDisasterTypeRepository disasterTypeRepository,
        IRegionRepository regionRepository,
        IAlertSettingRepository alertSettingRepository)
    {
        _logger = logger;
        _disasterTypeRepository = disasterTypeRepository;
        _regionRepository = regionRepository;
        _alertSettingRepository = alertSettingRepository;
    }

    public async Task<AlertSettingResponse> CreateAlertSettingAsync(CreateAlertSettingRequest request)
    {
        try
        {
            _logger.LogInformation("Creating alert setting for region {RegionId} and disaster type {DisasterTypeId}",
                request.RegionId, request.DisasterTypeId);

            // Check if region exists
            var region = await _regionRepository.GetByIdAsync(request.RegionId);
            if (region == null)
                throw new InvalidOperationException($"Region with ID {request.RegionId} not found");

            // Check if disaster type exists
            var disasterType = await _disasterTypeRepository.GetByIdAsync(request.DisasterTypeId);
            if (disasterType == null)
                throw new InvalidOperationException($"Disaster type with ID {request.DisasterTypeId} not found or is inactive");

            // Check if alert setting already exists for this region and disaster type
            var existingSetting = await _alertSettingRepository.GetByRegionAndDisasterTypeAsync(request.RegionId, request.DisasterTypeId);

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

            var createdSetting = await _alertSettingRepository.CreateAsync(alertSetting);

            _logger.LogInformation("Successfully created alert setting with ID {AlertSettingId}", createdSetting.Id);

            return await MapToResponseAsync(createdSetting);
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
            var alertSettings = await _alertSettingRepository.GetAllAsync();

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
            var alertSettings = await _alertSettingRepository.GetByRegionAsync(regionId);

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
            var alertSettings = await _alertSettingRepository.GetByDisasterTypeAsync(disasterTypeId);

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
            var existingSetting = await _alertSettingRepository.GetByIdAsync(id);
            if (existingSetting == null)
                throw new InvalidOperationException($"Alert setting with ID {id} not found");

            // Check if region exists
            var region = await _regionRepository.GetByIdAsync(request.RegionId);
            if (region == null)
                throw new InvalidOperationException($"Region with ID {request.RegionId} not found");

            // Check if disaster type exists
            var disasterType = await _disasterTypeRepository.GetByIdAsync(request.DisasterTypeId);
            if (disasterType == null)
                throw new InvalidOperationException($"Disaster type with ID {request.DisasterTypeId} not found or is inactive");

            // Update the alert setting
            existingSetting.RegionId = request.RegionId;
            existingSetting.DisasterTypeId = request.DisasterTypeId;
            existingSetting.ThresholdRiskScore = request.ThresholdRiskScore;
            existingSetting.IsActive = request.IsActive;
            existingSetting.UpdatedAt = DateTime.UtcNow;

            var updatedSetting = await _alertSettingRepository.UpdateAsync(existingSetting);

            _logger.LogInformation("Successfully updated alert setting with ID {AlertSettingId}", updatedSetting.Id);

            return await MapToResponseAsync(updatedSetting);
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
            var result = await _alertSettingRepository.DeleteAsync(id);
            if (result)
            {
                _logger.LogInformation("Successfully deleted alert setting with ID {AlertSettingId}", id);
            }
            return result;
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
            var alertSetting = await _alertSettingRepository.GetByRegionAndDisasterTypeAsync(regionId, disasterType.Id);
            return alertSetting?.ThresholdRiskScore;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get threshold for region {RegionId} and disaster type {DisasterTypeId}", regionId, disasterType.Id);
            throw;
        }
    }

    private async Task<AlertSettingResponse> MapToResponseAsync(AlertSetting alertSetting)
    {
        var region = await _regionRepository.GetByIdAsync(alertSetting.RegionId);
        var disasterType = await _disasterTypeRepository.GetByIdAsync(alertSetting.DisasterTypeId);

        return new AlertSettingResponse
        {
            Id = alertSetting.Id,
            RegionId = alertSetting.RegionId,
            RegionName = region?.Name ?? "Unknown",
            DisasterTypeId = alertSetting.DisasterTypeId,
            DisasterTypeName = disasterType?.Name ?? "Unknown",
            ThresholdRiskScore = alertSetting.ThresholdRiskScore,
            IsActive = alertSetting.IsActive,
            CreatedAt = alertSetting.CreatedAt,
            UpdatedAt = alertSetting.UpdatedAt
        };
    }
}
