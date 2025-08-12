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

    private const int CACHE_DURATION_MINUTES = 15;
    private const string CACHE_KEY_PREFIX = "disaster_risk_report";

    public DisasterRisksService(
        IRegionService regionService,
        IAlertSettingService alertSettingService,
        IRedisService redisService,
        IDisasterTypeRepository disasterTypeRepository,
        IAlertService alertService,
        ILogger<DisasterRisksService> logger)
    {
        _regionService = regionService;
        _alertSettingService = alertSettingService;
        _redisService = redisService;
        _disasterTypeRepository = disasterTypeRepository;
        _alertService = alertService;
        _logger = logger;
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
            var externalData = FetchExternalApiDataAsync(region.Latitude, region.Longitude, disasterTypeId);

            // Calculate risk score
            var riskScore = CalculateRiskScoreAsync(disasterTypeId, externalData);

            // Get threshold from alert settings
            var threshold = await _alertSettingService.GetThresholdAsync(regionId, disasterType);

            // Determine risk level
            var riskLevel = RiskAssessmentUtility.DetermineRiskLevelString(riskScore);

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
                RiskLevel = riskLevel,
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

    private object FetchExternalApiDataAsync(double latitude, double longitude, int disasterTypeId)
    {
        try
        {
            // This would integrate with actual external APIs
            // For now, we'll return mock data
            var mockData = new
            {
                Latitude = latitude,
                Longitude = longitude,
                DisasterTypeId = disasterTypeId,
                Timestamp = DateTime.UtcNow,
                WeatherData = "Mock weather data",
                SeismicData = "Mock seismic data",
                EnvironmentalData = "Mock environmental data"
            };

            _logger.LogDebug("Fetched external API data for coordinates ({Latitude}, {Longitude}) and disaster type {DisasterTypeId}",
                latitude, longitude, disasterTypeId);

            return mockData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching external API data for coordinates ({Latitude}, {Longitude}) and disaster type {DisasterTypeId}",
                latitude, longitude, disasterTypeId);
            throw;
        }
    }

    private double CalculateRiskScoreAsync(int disasterTypeId, object externalData)
    {
        try
        {
            // This would implement actual risk calculation algorithms
            // For now, we'll return a mock score based on disaster type
            var baseScore = disasterTypeId switch
            {
                1 => 0.7, // Earthquake
                2 => 0.5, // Flood
                3 => 0.6, // Hurricane
                4 => 0.4, // Wildfire
                _ => 0.3  // Default
            };

            // Add some randomness to simulate real-world variability
            var random = new Random();
            var variation = (random.NextDouble() - 0.5) * 0.2; // ±10% variation
            var finalScore = Math.Max(0.0, Math.Min(1.0, baseScore + variation));

            _logger.LogDebug("Calculated risk score {RiskScore} for disaster type {DisasterTypeId}", finalScore, disasterTypeId);

            return finalScore;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating risk score for disaster type {DisasterTypeId}", disasterTypeId);
            throw;
        }
    }
}
