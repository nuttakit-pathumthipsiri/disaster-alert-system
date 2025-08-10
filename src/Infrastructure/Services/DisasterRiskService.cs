using Core.DTOs;
using Core.Models;
using Core.Services;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

/// <summary>
/// Implementation of disaster risk service for calculating and managing risk assessments
/// </summary>
public class DisasterRiskService : IDisasterRiskService
{
    private readonly IExternalWeatherService _weatherService;
    private readonly IAlertSettingService _alertSettingService;
    private readonly IRedisService _redisService;
    private readonly IDisasterTypeRepository _disasterTypeRepository;
    private readonly ILogger<DisasterRiskService> _logger;

    public DisasterRiskService(
        IExternalWeatherService weatherService,
        IAlertSettingService alertSettingService,
        IRedisService redisService,
        IDisasterTypeRepository disasterTypeRepository,
        ILogger<DisasterRiskService> logger)
    {
        _weatherService = weatherService;
        _alertSettingService = alertSettingService;
        _redisService = redisService;
        _disasterTypeRepository = disasterTypeRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<DisasterRiskResponse>> CalculateAllDisasterRisksAsync()
    {
        try
        {
            _logger.LogInformation("Starting calculation of disaster risks for all regions");

            // Get all active disaster types
            var activeDisasterTypes = await _disasterTypeRepository.GetActiveDisasterTypesAsync();
            if (!activeDisasterTypes.Any())
            {
                _logger.LogWarning("No active disaster types found");
                return Enumerable.Empty<DisasterRiskResponse>();
            }

            var disasterRisks = new List<DisasterRiskResponse>();

            // This would typically get all regions from a repository
            // For now, we'll return an empty list as the actual implementation would depend on the region repository
            // TODO: Implement region repository integration

            _logger.LogInformation("Completed calculation of disaster risks for all regions. Found {ActiveDisasterTypes} active disaster types", activeDisasterTypes.Count());
            return disasterRisks;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to calculate disaster risks for all regions");
            throw;
        }
    }

    public async Task<DisasterRiskResponse> CalculateDisasterRiskAsync(int regionId, DisasterType disasterType)
    {
        try
        {
            _logger.LogInformation("Calculating disaster risk for region {RegionId} and disaster type {DisasterTypeId}", regionId, disasterType.Id);

            // Check if disaster type is active
            if (!disasterType.IsActive)
            {
                _logger.LogWarning("Disaster type {DisasterTypeId} is not active, skipping calculation", disasterType.Id);
                throw new InvalidOperationException($"Disaster type {disasterType.Id} is not active");
            }

            // Check cache first
            var cachedRisk = await GetCachedDisasterRiskAsync(regionId, disasterType);
            if (cachedRisk != null)
            {
                _logger.LogInformation("Returning cached disaster risk for region {RegionId}", regionId);
                return MapToResponse(cachedRisk);
            }

            // Get threshold for this region and disaster type
            var threshold = await _alertSettingService.GetThresholdAsync(regionId, disasterType);
            var thresholdValue = threshold ?? GetDefaultThreshold(disasterType.Id);

            // Calculate risk based on disaster type
            var riskScore = await CalculateRiskScoreAsync(regionId, disasterType.Id);
            var riskLevel = DetermineRiskLevel(riskScore);
            var shouldTriggerAlert = riskScore >= thresholdValue;

            // Create disaster risk object
            var disasterRisk = new DisasterRisk
            {
                RegionId = regionId,
                DisasterTypeId = disasterType.Id,
                RiskScore = riskScore,
                RiskLevel = riskLevel,
                ShouldTriggerAlert = shouldTriggerAlert,
                ThresholdValue = thresholdValue,
                CalculatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15) // Cache for 15 minutes
            };

            // Cache the result
            await CacheDisasterRiskAsync(disasterRisk);

            _logger.LogInformation("Calculated disaster risk for region {RegionId}: Score={RiskScore}, Level={RiskLevel}, Alert={ShouldTriggerAlert}",
                regionId, riskScore, riskLevel, shouldTriggerAlert);

            return MapToResponse(disasterRisk);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to calculate disaster risk for region {RegionId} and disaster type {DisasterTypeId}", regionId, disasterType.Id);
            throw;
        }
    }

    public async Task<DisasterRisk?> GetCachedDisasterRiskAsync(int regionId, DisasterType disasterType)
    {
        try
        {
            var cacheKey = $"disaster_risk:{regionId}:{disasterType.Id}";
            var cachedData = await _redisService.GetAsync<DisasterRisk>(cacheKey);

            if (cachedData != null)
            {
                _logger.LogDebug("Retrieved cached disaster risk for region {RegionId}", regionId);
                return cachedData;
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve cached disaster risk for region {RegionId}", regionId);
            return null;
        }
    }

    public async Task CacheDisasterRiskAsync(DisasterRisk disasterRisk)
    {
        try
        {
            var cacheKey = $"disaster_risk:{disasterRisk.RegionId}:{disasterRisk.DisasterTypeId}";

            // Cache disaster risk for 15 minutes
            await _redisService.SetAsync(cacheKey, disasterRisk, TimeSpan.FromMinutes(15));
            _logger.LogDebug("Cached disaster risk for region {RegionId} for 15 minutes",
                disasterRisk.RegionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cache disaster risk for region {RegionId}", disasterRisk.RegionId);
        }
    }

    private async Task<double> CalculateRiskScoreAsync(int regionId, int disasterTypeId)
    {
        try
        {
            // This would typically get region coordinates from a repository
            // For now, we'll use mock coordinates
            var mockLatitude = 13.7563; // Bangkok coordinates
            var mockLongitude = 100.5018;

            switch (disasterTypeId)
            {
                case 1: // Flood
                    return await CalculateFloodRiskAsync(mockLatitude, mockLongitude);

                case 2: // Earthquake
                    return await CalculateEarthquakeRiskAsync(mockLatitude, mockLongitude);

                case 3: // Wildfire
                    return await CalculateWildfireRiskAsync(mockLatitude, mockLongitude);

                case 4: // Storm
                    return await CalculateStormRiskAsync(mockLatitude, mockLongitude);

                case 5: // Drought
                    return await CalculateDroughtRiskAsync(mockLatitude, mockLongitude);

                default:
                    return 0.0;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to calculate risk score for region {RegionId} and disaster type {DisasterTypeId}", regionId, disasterTypeId);
            return 0.0;
        }
    }

    private async Task<double> CalculateFloodRiskAsync(double latitude, double longitude)
    {
        var weatherData = await _weatherService.GetWeatherDataAsync(latitude, longitude);

        // Flood risk calculation based on precipitation
        var precipitationRisk = Math.Min(100, weatherData.Precipitation * 2); // 50mm = 100% risk
        var humidityRisk = Math.Min(50, weatherData.Humidity * 0.5); // High humidity increases risk

        return Math.Min(100, (precipitationRisk + humidityRisk) / 2);
    }

    private async Task<double> CalculateEarthquakeRiskAsync(double latitude, double longitude)
    {
        var earthquakeData = await _weatherService.GetEarthquakeDataAsync(latitude, longitude);

        // Earthquake risk calculation based on magnitude and distance
        var magnitudeRisk = Math.Min(100, earthquakeData.Magnitude * 20); // Magnitude 5.0 = 100% risk
        var distanceRisk = Math.Max(0, 100 - earthquakeData.Distance * 2); // Closer = higher risk

        return Math.Min(100, (magnitudeRisk + distanceRisk) / 2);
    }

    private async Task<double> CalculateWildfireRiskAsync(double latitude, double longitude)
    {
        var wildfireData = await _weatherService.GetWildfireRiskDataAsync(latitude, longitude);

        // Wildfire risk calculation based on temperature, humidity, and drought
        var temperatureRisk = Math.Min(100, Math.Max(0, wildfireData.Temperature - 25) * 5); // Above 25°C
        var humidityRisk = Math.Min(100, Math.Max(0, 50 - wildfireData.Humidity) * 2); // Below 50%
        var droughtRisk = wildfireData.DroughtIndex;

        return Math.Min(100, (temperatureRisk + humidityRisk + droughtRisk) / 3);
    }

    private async Task<double> CalculateStormRiskAsync(double latitude, double longitude)
    {
        var weatherData = await _weatherService.GetWeatherDataAsync(latitude, longitude);

        // Storm risk calculation based on wind speed and precipitation
        var windRisk = Math.Min(100, weatherData.WindSpeed * 3.33); // 30 m/s = 100% risk
        var precipitationRisk = Math.Min(100, weatherData.Precipitation * 2); // 50mm = 100% risk

        return Math.Min(100, (windRisk + precipitationRisk) / 2);
    }

    private async Task<double> CalculateDroughtRiskAsync(double latitude, double longitude)
    {
        var weatherData = await _weatherService.GetWeatherDataAsync(latitude, longitude);

        // Drought risk calculation based on low precipitation and high temperature
        var precipitationRisk = Math.Min(100, Math.Max(0, 50 - weatherData.Precipitation) * 2); // Below 50mm
        var temperatureRisk = Math.Min(100, Math.Max(0, weatherData.Temperature - 20) * 5); // Above 20°C
        var humidityRisk = Math.Min(100, Math.Max(0, 60 - weatherData.Humidity) * 2.5); // Below 60%

        return Math.Min(100, (precipitationRisk + temperatureRisk + humidityRisk) / 3);
    }

    private RiskLevel DetermineRiskLevel(double riskScore)
    {
        return riskScore switch
        {
            < 30 => RiskLevel.Low,
            < 70 => RiskLevel.Medium,
            _ => RiskLevel.High
        };
    }

    private double GetDefaultThreshold(int disasterTypeId)
    {
        return disasterTypeId switch
        {
            1 => 50.0,      // 50% risk triggers flood alert
            2 => 70.0,      // 70% risk triggers earthquake alert
            3 => 60.0,      // 60% risk triggers wildfire alert
            4 => 65.0,      // 65% risk triggers storm alert
            5 => 55.0,      // 55% risk triggers drought alert
            _ => 50.0
        };
    }

    private DisasterRiskResponse MapToResponse(DisasterRisk disasterRisk)
    {
        return new DisasterRiskResponse
        {
            RegionId = disasterRisk.RegionId,
            RegionName = $"Region {disasterRisk.RegionId}", // This would come from region repository
            DisasterTypeId = disasterRisk.DisasterTypeId,
            DisasterTypeName = $"DisasterType {disasterRisk.DisasterTypeId}", // This would come from disaster type repository
            RiskScore = disasterRisk.RiskScore,
            RiskLevel = disasterRisk.RiskLevel,
            AlertTriggered = disasterRisk.ShouldTriggerAlert,
            ThresholdValue = disasterRisk.ThresholdValue,
            CalculatedAt = disasterRisk.CalculatedAt,
            Details = $"Risk assessment for disaster type {disasterRisk.DisasterTypeId} in region {disasterRisk.RegionId}"
        };
    }
}
