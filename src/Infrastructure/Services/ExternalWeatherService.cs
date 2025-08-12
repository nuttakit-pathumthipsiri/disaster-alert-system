using Core.Interfaces;
using Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Infrastructure.Services;


public class ExternalWeatherService : IExternalWeatherService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ExternalWeatherService> _logger;
    private readonly IRedisService _redisService;
    private readonly string _openWeatherApiKey;
    private readonly string _usgsApiBaseUrl;

    public ExternalWeatherService(
        HttpClient httpClient,
        ILogger<ExternalWeatherService> logger,
        IRedisService redisService,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        _redisService = redisService;
        _openWeatherApiKey = configuration["ExternalApis:OpenWeather:ApiKey"] ?? string.Empty;
        _usgsApiBaseUrl = configuration["ExternalApis:USGS:BaseUrl"] ?? "https://earthquake.usgs.gov/fdsnws/event/1/";
    }

    public async Task<WeatherData> GetWeatherDataAsync(double latitude, double longitude)
    {
        try
        {
            // Check cache first
            var cacheKey = $"weather:{latitude:F4}:{longitude:F4}";
            var cachedWeather = await _redisService.GetAsync<WeatherData>(cacheKey);
            if (cachedWeather != null)
            {
                _logger.LogDebug("Returning cached weather data for coordinates {Latitude}, {Longitude}", latitude, longitude);
                return cachedWeather;
            }

            if (string.IsNullOrEmpty(_openWeatherApiKey))
            {
                _logger.LogWarning("OpenWeather API key not configured, returning mock data");
                var mockData = GetMockWeatherData(latitude, longitude);
                // Cache mock data for 15 minutes
                await _redisService.SetAsync(cacheKey, mockData, TimeSpan.FromMinutes(15));
                return mockData;
            }

            var url = $"https://api.openweathermap.org/data/2.5/weather?lat={latitude}&lon={longitude}&appid={_openWeatherApiKey}&units=metric";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var weatherResponse = JsonSerializer.Deserialize<OpenWeatherResponse>(content);

            if (weatherResponse?.Main == null)
                throw new InvalidOperationException("Invalid response from OpenWeather API");

            var weatherData = new WeatherData
            {
                Temperature = weatherResponse.Main.Temp,
                Humidity = weatherResponse.Main.Humidity,
                Precipitation = weatherResponse.Rain?.OneHour ?? 0,
                WindSpeed = weatherResponse.Wind?.Speed ?? 0,
                Timestamp = DateTime.UtcNow
            };

            // Cache external API data for 15 minutes
            await _redisService.SetAsync(cacheKey, weatherData, TimeSpan.FromMinutes(15));
            return weatherData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get weather data for coordinates {Latitude}, {Longitude}", latitude, longitude);
            var mockData = GetMockWeatherData(latitude, longitude);
            // Cache mock data for 15 minutes
            var cacheKey = $"weather:{latitude:F4}:{longitude:F4}";
            await _redisService.SetAsync(cacheKey, mockData, TimeSpan.FromMinutes(15));
            return mockData;
        }
    }

    public async Task<EarthquakeData> GetEarthquakeDataAsync(double latitude, double longitude, double radiusKm = 100)
    {
        try
        {
            // Check cache first
            var cacheKey = $"earthquake:{latitude:F4}:{longitude:F4}:{radiusKm}";
            var cachedEarthquake = await _redisService.GetAsync<EarthquakeData>(cacheKey);
            if (cachedEarthquake != null)
            {
                _logger.LogDebug("Returning cached earthquake data for coordinates {Latitude}, {Longitude}", latitude, longitude);
                return cachedEarthquake;
            }

            var url = $"{_usgsApiBaseUrl}query?format=geojson&starttime={DateTime.UtcNow.AddDays(-1):yyyy-MM-dd}&endtime={DateTime.UtcNow:yyyy-MM-dd}&latitude={latitude}&longitude={longitude}&maxradiuskm={radiusKm}&minmagnitude=2.0&orderby=magnitude&limit=1";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var earthquakeResponse = JsonSerializer.Deserialize<USGSEarthquakeResponse>(content);

            EarthquakeData earthquakeData;
            if (earthquakeResponse?.Features?.Any() == true)
            {
                var latest = earthquakeResponse.Features.First();
                earthquakeData = new EarthquakeData
                {
                    Magnitude = latest.Properties.Mag,
                    Depth = latest.Geometry.Coordinates[2],
                    Distance = CalculateDistance(latitude, longitude, latest.Geometry.Coordinates[1], latest.Geometry.Coordinates[0]),
                    Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(latest.Properties.Time).DateTime
                };
            }
            else
            {
                earthquakeData = new EarthquakeData
                {
                    Magnitude = 0,
                    Depth = 0,
                    Distance = 0,
                    Timestamp = DateTime.UtcNow
                };
            }

            // Cache external API data for 15 minutes
            await _redisService.SetAsync(cacheKey, earthquakeData, TimeSpan.FromMinutes(15));
            return earthquakeData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get earthquake data for coordinates {Latitude}, {Longitude}", latitude, longitude);
            var mockData = GetMockEarthquakeData();
            // Cache mock data for 15 minutes
            var cacheKey = $"earthquake:{latitude:F4}:{longitude:F4}:{radiusKm}";
            await _redisService.SetAsync(cacheKey, mockData, TimeSpan.FromMinutes(15));
            return mockData;
        }
    }

    public async Task<WildfireRiskData> GetWildfireRiskDataAsync(double latitude, double longitude)
    {
        try
        {
            // Check cache first
            var cacheKey = $"wildfire:{latitude:F4}:{longitude:F4}";
            var cachedWildfire = await _redisService.GetAsync<WildfireRiskData>(cacheKey);
            if (cachedWildfire != null)
            {
                _logger.LogDebug("Returning cached wildfire data for coordinates {Latitude}, {Longitude}", latitude, longitude);
                return cachedWildfire;
            }

            // Get weather data for wildfire risk calculation
            var weatherData = await GetWeatherDataAsync(latitude, longitude);

            // Calculate wildfire risk based on weather conditions
            var temperatureRisk = Math.Min(100, Math.Max(0, (weatherData.Temperature - 20) * 2)); // Higher temp = higher risk
            var humidityRisk = Math.Max(0, 100 - weatherData.Humidity); // Lower humidity = higher risk
            var windRisk = Math.Min(100, weatherData.WindSpeed * 5); // Higher wind = higher risk

            var wildfireData = new WildfireRiskData
            {
                Temperature = weatherData.Temperature,
                Humidity = weatherData.Humidity,
                WindSpeed = weatherData.WindSpeed,
                RiskScore = Math.Min(100, (temperatureRisk + humidityRisk + windRisk) / 3),
                Timestamp = DateTime.UtcNow
            };

            // Cache calculated data for 15 minutes
            await _redisService.SetAsync(cacheKey, wildfireData, TimeSpan.FromMinutes(15));
            return wildfireData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get wildfire risk data for coordinates {Latitude}, {Longitude}", latitude, longitude);
            var mockData = GetMockWildfireRiskData();
            // Cache mock data for 15 minutes
            var cacheKey = $"wildfire:{latitude:F4}:{longitude:F4}";
            await _redisService.SetAsync(cacheKey, mockData, TimeSpan.FromMinutes(15));
            return mockData;
        }
    }

    private WeatherData GetMockWeatherData(double latitude, double longitude)
    {
        var random = new Random((int)(latitude * 1000 + longitude * 1000));
        return new WeatherData
        {
            Temperature = 20 + random.Next(-10, 15),
            Humidity = 40 + random.Next(0, 40),
            Precipitation = random.Next(0, 100),
            WindSpeed = random.Next(0, 30),
            Timestamp = DateTime.UtcNow
        };
    }

    private EarthquakeData GetMockEarthquakeData()
    {
        var random = new Random();
        return new EarthquakeData
        {
            Magnitude = random.Next(0, 10),
            Depth = random.Next(0, 100),
            Distance = random.Next(0, 50),
            Timestamp = DateTime.UtcNow
        };
    }

    private WildfireRiskData GetMockWildfireRiskData()
    {
        var random = new Random();
        return new WildfireRiskData
        {
            Temperature = 25 + random.Next(-5, 15),
            Humidity = 30 + random.Next(0, 40),
            WindSpeed = random.Next(0, 25),
            Timestamp = DateTime.UtcNow
        };
    }

    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double earthRadius = 6371; // Earth's radius in kilometers
        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return earthRadius * c;
    }

    private double ToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }

    public async Task<double> GetRiskScoreAsync(double latitude, double longitude, string disasterType)
    {
        try
        {
            switch (disasterType.ToLower())
            {
                case "earthquake":
                    var earthquakeData = await GetEarthquakeDataAsync(latitude, longitude);
                    // Calculate earthquake risk based on magnitude and distance
                    var earthquakeRisk = Math.Min(100, earthquakeData.Magnitude * 10 + (100 - earthquakeData.Distance));
                    return earthquakeRisk;

                case "wildfire":
                    var wildfireData = await GetWildfireRiskDataAsync(latitude, longitude);
                    return wildfireData.RiskScore;

                case "flood":
                    var weatherData = await GetWeatherDataAsync(latitude, longitude);
                    // Calculate flood risk based on precipitation and humidity
                    var floodRisk = Math.Min(100, weatherData.Precipitation * 0.5 + (100 - weatherData.Humidity) * 0.3);
                    return floodRisk;

                default:
                    // Default risk calculation based on weather conditions
                    var defaultWeatherData = await GetWeatherDataAsync(latitude, longitude);
                    var defaultRisk = Math.Min(100,
                        (defaultWeatherData.Temperature > 30 ? 30 : 0) +
                        (defaultWeatherData.WindSpeed > 20 ? 40 : 0) +
                        (defaultWeatherData.Precipitation > 50 ? 30 : 0));
                    return defaultRisk;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to calculate risk score for disaster type {DisasterType} at coordinates {Latitude}, {Longitude}",
                disasterType, latitude, longitude);
            // Return a default risk score in case of error
            return 25.0;
        }
    }
}

// Response models for external APIs
public class OpenWeatherResponse
{
    public MainInfo Main { get; set; } = new();
    public WindInfo Wind { get; set; } = new();
    public RainInfo? Rain { get; set; }
}

public class MainInfo
{
    public double Temp { get; set; }
    public double Humidity { get; set; }
}

public class WindInfo
{
    public double Speed { get; set; }
}

public class RainInfo
{
    public double OneHour { get; set; }
}

public class USGSEarthquakeResponse
{
    public List<EarthquakeFeature> Features { get; set; } = new();
}

public class EarthquakeFeature
{
    public EarthquakeProperties Properties { get; set; } = new();
    public EarthquakeGeometry Geometry { get; set; } = new();
}

public class EarthquakeProperties
{
    public double Mag { get; set; }
    public long Time { get; set; }
}

public class EarthquakeGeometry
{
    public List<double> Coordinates { get; set; } = new();
}
