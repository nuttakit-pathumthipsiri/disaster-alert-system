namespace Core.Models;

/// <summary>
/// Weather data model for external weather service
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
/// Earthquake data model for external weather service
/// </summary>
public class EarthquakeData
{
    public double Magnitude { get; set; }
    public double Depth { get; set; }
    public double Distance { get; set; }
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// Wildfire risk data model for external weather service
/// </summary>
public class WildfireRiskData
{
    public double Temperature { get; set; }
    public double Humidity { get; set; }
    public double WindSpeed { get; set; }
    public double RiskScore { get; set; }
    public DateTime Timestamp { get; set; }
}
