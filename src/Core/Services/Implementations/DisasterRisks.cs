using Core.DTOs;
using Core.Models;
using Core.Interfaces;
using Core.Services;
using Core.Utilities;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Core.Services.Implementations;

public class DisasterRisksService : IDisasterRisksService
{
    private readonly IRegionService _regionService;
    private readonly IAlertSettingService _alertSettingService;
    private readonly IRedisService _redisService;
    private readonly IDisasterTypeRepository _disasterTypeRepository;
    private readonly IAlertService _alertService;
    private readonly ILogger<DisasterRisksService> _logger;

    private readonly IExternalWeatherService _externalWeatherService;

    private const int CACHE_DURATION_MINUTES = 15;
    private const string CACHE_KEY_PREFIX = "disaster_risk_report";

    public DisasterRisksService(
        IRegionService regionService,
        IAlertSettingService alertSettingService,
        IRedisService redisService,
        IDisasterTypeRepository disasterTypeRepository,
        IAlertService alertService,
        ILogger<DisasterRisksService> logger,
        IExternalWeatherService externalWeatherService)
    {
        _regionService = regionService;
        _alertSettingService = alertSettingService;
        _redisService = redisService;
        _disasterTypeRepository = disasterTypeRepository;
        _alertService = alertService;
        _logger = logger;
        _externalWeatherService = externalWeatherService;
    }

    public async Task<IEnumerable<DisasterRiskReportResponse>> GetDisasterRiskReportsAsync()
    {
        try
        {
            var regions = await _regionService.GetAllRegionsAsync();
            var reports = new List<DisasterRiskReportResponse>();

            foreach (var region in regions)
            {
                // Get disaster types for this region (already filtered for active ones by RegionService)
                var disasterTypeIds = region.MonitoredDisasterTypes.Select(dt => dt.Id).ToList();

                foreach (var disasterTypeId in disasterTypeIds)
                {
                    var report = await GetDisasterRiskReportAsync(region.Id, disasterTypeId);
                    if (report != null)
                    {
                        reports.Add(report);
                    }
                }
            }

            return reports;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting disaster risk reports");
            throw;
        }
    }

    public async Task<DisasterRiskReportResponse?> GetDisasterRiskReportAsync(int regionId, int disasterTypeId)
    {
        try
        {
            // Check cache first
            var cacheKey = $"{CACHE_KEY_PREFIX}:{regionId}:{disasterTypeId}";
            var cachedReport = await _redisService.GetAsync<DisasterRiskReportResponse>(cacheKey);

            if (cachedReport != null)
            {
                _logger.LogInformation("Retrieved disaster risk report from cache for region {RegionId} and disaster type {DisasterTypeId}", regionId, disasterTypeId);
                return cachedReport;
            }

            // Get region and disaster type information
            var region = await _regionService.GetRegionAsync(regionId);
            if (region == null)
            {
                _logger.LogWarning("Region {RegionId} not found", regionId);
                return null;
            }

            var disasterType = await _disasterTypeRepository.GetByIdAsync(disasterTypeId);
            if (disasterType == null || !disasterType.IsActive)
            {
                _logger.LogWarning("Disaster type {DisasterTypeId} not found or inactive", disasterTypeId);
                return null;
            }

            // Fetch external API data
            var externalData = await FetchExternalApiDataAsync(region.Latitude, region.Longitude, disasterTypeId);

            // Extract risk score from external data
            int riskScore = 0;
            var riskScoreProperty = externalData?.GetType().GetProperty("RiskScore");
            if (riskScoreProperty?.GetValue(externalData) is double externalRiskScore)
            {
                // Convert percentage to 0-1 scale if needed
                riskScore = (int)(externalRiskScore > 1.0 ? externalRiskScore / 100.0 : externalRiskScore);
            }

            // Get threshold from alert settings
            var threshold = await _alertSettingService.GetThresholdAsync(regionId, disasterType);

            // Determine risk level
            // var riskLevel = RiskAssessmentUtility.DetermineRiskLevelString(riskScore);

            // Check if alert should be triggered
            var alertTriggered = threshold.HasValue && riskScore >= threshold.Value;

            // Create and save alert if threshold is exceeded
            if (alertTriggered)
            {
                try
                {
                    var alert = await _alertService.CreateAlertAsync(
                        regionId,
                        disasterTypeId,
                        riskScore,
                        threshold.Value,
                        JsonSerializer.Serialize(externalData)
                    );

                    _logger.LogInformation("Alert created successfully with ID {AlertId} for region {RegionId} and disaster type {DisasterTypeId}",
                        alert.Id, regionId, disasterTypeId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to create alert for region {RegionId} and disaster type {DisasterTypeId}",
                        regionId, disasterTypeId);
                    // Continue with report generation even if alert creation fails
                }
            }

            // Create response
            var report = new DisasterRiskReportResponse
            {
                RegionId = region.Id,
                RegionName = region.Name,
                DisasterTypeId = disasterType.Id,
                DisasterTypeName = disasterType.Name,
                RiskScore = riskScore,
                // RiskLevel = riskLevel,
                AlertTriggered = alertTriggered,
                ThresholdValue = threshold ?? 0.0,
                CalculatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(CACHE_DURATION_MINUTES),
                ExternalApiData = JsonSerializer.Serialize(externalData)
            };

            // Cache the report
            await _redisService.SetAsync(cacheKey, report, TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));

            _logger.LogInformation("Generated disaster risk report for region {RegionId} and disaster type {DisasterTypeId} with risk score {RiskScore}",
                regionId, disasterTypeId, riskScore);

            return report;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting disaster risk report for region {RegionId} and disaster type {DisasterTypeId}", regionId, disasterTypeId);
            throw;
        }
    }

    private async Task<object?> FetchExternalApiDataAsync(double latitude, double longitude, int disasterTypeId)
    {
        try
        {
            switch (disasterTypeId)
            {
                case 1:
                    var earthquakeData = await _externalWeatherService.GetEarthquakeDataAsync(latitude, longitude);
                    return earthquakeData;
                case 2:
                    var floodData = await _externalWeatherService.GetWeatherDataAsync(latitude, longitude);
                    return floodData;
                case 3:
                    var wildfireData = await _externalWeatherService.GetWildfireRiskDataAsync(latitude, longitude);
                    return wildfireData;
                default:
                    return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching external API data for coordinates ({Latitude}, {Longitude}) and disaster type {DisasterTypeId}",
                latitude, longitude, disasterTypeId);
            throw;
        }
    }
}
