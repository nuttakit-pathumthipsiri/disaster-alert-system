using Core.Models;

namespace Core.Interfaces;

public interface IExternalWeatherService
{
    Task<WeatherData> GetWeatherDataAsync(double latitude, double longitude);
    Task<EarthquakeData> GetEarthquakeDataAsync(double latitude, double longitude, double radiusKm = 100);
    Task<WildfireRiskData> GetWildfireRiskDataAsync(double latitude, double longitude);
    Task<double> GetRiskScoreAsync(double latitude, double longitude, string disasterType);
}
