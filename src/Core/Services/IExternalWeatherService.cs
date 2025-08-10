namespace Core.Services;

/// <summary>
/// Service for retrieving weather data from external APIs
/// </summary>
public interface IExternalWeatherService
{
    /// <summary>
    /// Gets current weather data for a specific location
    /// </summary>
    /// <param name="latitude">Latitude coordinate</param>
    /// <param name="longitude">Longitude coordinate</param>
    /// <returns>Weather data response</returns>
    Task<WeatherData> GetWeatherDataAsync(double latitude, double longitude);

    /// <summary>
    /// Gets earthquake data for a specific location
    /// </summary>
    /// <param name="latitude">Latitude coordinate</param>
    /// <param name="longitude">Longitude coordinate</param>
    /// <param name="radiusKm">Search radius in kilometers</param>
    /// <returns>Earthquake data response</returns>
    Task<EarthquakeData> GetEarthquakeDataAsync(double latitude, double longitude, double radiusKm = 100);

    /// <summary>
    /// Gets wildfire risk data for a specific location
    /// </summary>
    /// <param name="latitude">Latitude coordinate</param>
    /// <param name="longitude">Longitude coordinate</param>
    /// <returns>Wildfire risk data response</returns>
    Task<WildfireRiskData> GetWildfireRiskDataAsync(double latitude, double longitude);
}

/// <summary>
/// Weather data from external API
/// </summary>
public class WeatherData
{
    public double Temperature { get; set; }
    public double Humidity { get; set; }
    public double Precipitation { get; set; }
    public double WindSpeed { get; set; }
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// Earthquake data from external API
/// </summary>
public class EarthquakeData
{
    public double Magnitude { get; set; }
    public double Depth { get; set; }
    public double Distance { get; set; }
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// Wildfire risk data from external API
/// </summary>
public class WildfireRiskData
{
    public double Temperature { get; set; }
    public double Humidity { get; set; }
    public double WindSpeed { get; set; }
    public double DroughtIndex { get; set; }
    public double RiskScore { get; set; }
    public DateTime Timestamp { get; set; }
}
