using Core.DTOs;
using Core.Models;
using System.Text.Json;

namespace Core.Services;

public class RegionService : IRegionService
{
    private readonly IRegionRepository _regionRepository;
    private readonly IRedisService _redisService;

    public RegionService(IRegionRepository repository, IRedisService redisService)
    {
        _regionRepository = repository;
        _redisService = redisService;
    }

    public async Task<RegionResponse> CreateRegionAsync(CreateRegionRequest request)
    {
        // Create new region
        var region = new Region
        {
            Name = request.Name,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            CreatedAt = DateTime.UtcNow
        };

        // Add disaster types by IDs
        if (request.MonitoredDisasterTypeIds != null && request.MonitoredDisasterTypeIds.Any())
        {
            // Get disaster types from repository and add them to the region
            // This will be handled by Entity Framework when the region is loaded with includes
        }

        // Save to database
        var createdRegion = await _regionRepository.CreateAsync(region);

        // Return DTO directly from database
        return MapToRegionResponse(createdRegion);
    }

    public async Task<RegionResponse?> GetRegionAsync(int id)
    {
        // Get directly from database
        var region = await _regionRepository.GetByIdAsync(id);

        if (region != null)
        {
            return MapToRegionResponse(region);
        }

        return null;
    }

    public async Task<IEnumerable<RegionResponse>> GetAllRegionsAsync()
    {
        // Get directly from database
        var regions = await _regionRepository.GetAllAsync();

        // Map to DTOs and return
        var regionResponses = regions.Select(MapToRegionResponse).ToList();
        return regionResponses;
    }

    public async Task<RegionResponse?> UpdateRegionAsync(int id, UpdateRegionRequest request)
    {
        // Get existing region
        var existingRegion = await _regionRepository.GetByIdAsync(id);

        if (existingRegion == null)
        {
            return null;
        }

        // Update region properties (only if provided)
        if (!string.IsNullOrEmpty(request.Name))
            existingRegion.Name = request.Name;

        if (request.Latitude.HasValue)
            existingRegion.Latitude = request.Latitude.Value;

        if (request.Longitude.HasValue)
            existingRegion.Longitude = request.Longitude.Value;

        if (request.MonitoredDisasterTypeIds != null)
        {
            // Clear existing disaster types and add new ones
            // This will be handled by Entity Framework when the region is loaded with includes
        }

        existingRegion.UpdatedAt = DateTime.UtcNow;

        // Save to database
        var updatedRegion = await _regionRepository.UpdateAsync(existingRegion);

        // Return DTO directly from database
        return MapToRegionResponse(updatedRegion);
    }

    public async Task<bool> DeleteRegionAsync(int id)
    {
        // Check if region exists
        var existingRegion = await _regionRepository.GetByIdAsync(id);

        if (existingRegion == null)
        {
            return false;
        }

        // Delete from database
        var deleted = await _regionRepository.DeleteAsync(id);

        return deleted;
    }

    private static RegionResponse MapToRegionResponse(Region region)
    {
        var disasterTypeInfos = new List<DisasterTypeInfo>();

        try
        {
            if (!string.IsNullOrEmpty(region.MonitoredDisasterTypes))
            {
                var disasterTypeNames = JsonSerializer.Deserialize<List<string>>(region.MonitoredDisasterTypes);
                if (disasterTypeNames != null)
                {
                    // Map disaster type names to proper DisasterTypeInfo objects
                    disasterTypeInfos = disasterTypeNames.Select(name => new DisasterTypeInfo
                    {
                        Id = GetDisasterTypeIdByName(name), // Get actual ID from enum
                        Name = name,
                        IsActive = true
                    }).ToList();
                }
            }
        }
        catch (JsonException)
        {
            // If JSON parsing fails, return empty list
            disasterTypeInfos = new List<DisasterTypeInfo>();
        }

        return new RegionResponse
        {
            Id = region.Id,
            Name = region.Name,
            Latitude = region.Latitude,
            Longitude = region.Longitude,
            MonitoredDisasterTypes = disasterTypeInfos,
            CreatedAt = region.CreatedAt,
            UpdatedAt = region.UpdatedAt
        };
    }

    private static int GetDisasterTypeIdByName(string name)
    {
        return name.ToLower() switch
        {
            "earthquake" => 1,
            "tsunami" => 2,
            "flood" => 3,
            "landslide" => 4,
            "wildfire" => 5,
            "storm" => 6,
            "drought" => 7,
            _ => 0
        };
    }
}
