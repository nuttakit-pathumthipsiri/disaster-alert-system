using Core.DTOs;
using Core.Models;
using Core.Interfaces;
using Core.Services;
using System.Text.Json;

namespace Core.Services.Implementations;

public class RegionService : IRegionService
{
    private readonly IRegionRepository _regionRepository;
    private readonly IRedisService _redisService;
    private readonly IDisasterTypeRepository _disasterTypeRepository;

    public RegionService(
        IRegionRepository repository,
        IRedisService redisService,
        IDisasterTypeRepository disasterTypeRepository)
    {
        _regionRepository = repository;
        _redisService = redisService;
        _disasterTypeRepository = disasterTypeRepository;
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
        if (request.MonitoredDisasterTypes != null && request.MonitoredDisasterTypes.Any())
        {
            // Validate that all disaster type IDs exist and are active
            foreach (var disasterTypeId in request.MonitoredDisasterTypes)
            {
                var disasterType = await _disasterTypeRepository.GetByIdAsync(disasterTypeId);
                if (disasterType == null)
                {
                    throw new NotFoundException($"Disaster type with ID {disasterTypeId} not found or inactive");
                }
            }

            // Store as JSON array of IDs
            region.MonitoredDisasterTypes = JsonSerializer.Serialize(request.MonitoredDisasterTypes);
        }

        // Save to database
        var createdRegion = await _regionRepository.CreateAsync(region);

        // Return DTO directly from database
        return await MapToRegionResponse(createdRegion);
    }

    public async Task<RegionResponse?> GetRegionAsync(int id)
    {
        // Get directly from database
        var region = await _regionRepository.GetByIdAsync(id);

        if (region != null)
        {
            return await MapToRegionResponse(region);
        }

        return null;
    }

    public async Task<IEnumerable<RegionResponse>> GetAllRegionsAsync()
    {
        // Get all regions and disaster types separately
        var regions = await _regionRepository.GetAllAsync();
        var disasterTypes = await _disasterTypeRepository.GetActiveDisasterTypesAsync();

        var regionResponses = new List<RegionResponse>();

        foreach (var region in regions)
        {
            var disasterTypeInfos = new List<DisasterTypeInfo>();

            try
            {
                if (!string.IsNullOrEmpty(region.MonitoredDisasterTypes))
                {
                    var disasterTypeIds = JsonSerializer.Deserialize<List<int>>(region.MonitoredDisasterTypes);
                    if (disasterTypeIds != null)
                    {
                        // Only include active disaster types
                        foreach (var id in disasterTypeIds)
                        {
                            var disasterType = disasterTypes.FirstOrDefault(dt => dt.Id == id);
                            if (disasterType != null)
                            {
                                disasterTypeInfos.Add(new DisasterTypeInfo
                                {
                                    Id = disasterType.Id,
                                    Name = disasterType.Name
                                });
                            }
                        }
                    }
                }
            }
            catch (JsonException)
            {
                // If JSON parsing fails, use empty list
                disasterTypeInfos = new List<DisasterTypeInfo>();
            }

            regionResponses.Add(new RegionResponse
            {
                Id = region.Id,
                Name = region.Name,
                Latitude = region.Latitude,
                Longitude = region.Longitude,
                MonitoredDisasterTypes = disasterTypeInfos,
                CreatedAt = region.CreatedAt,
                UpdatedAt = region.UpdatedAt
            });
        }

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

        if (request.MonitoredDisasterTypes != null)
        {
            // Validate that all disaster type IDs exist and are active
            foreach (var disasterTypeId in request.MonitoredDisasterTypes)
            {
                var disasterType = await _disasterTypeRepository.GetByIdAsync(disasterTypeId);
                if (disasterType == null)
                {
                    throw new NotFoundException($"Disaster type with ID {disasterTypeId} not found or inactive");
                }
            }

            // Update monitored disaster types
            existingRegion.MonitoredDisasterTypes = JsonSerializer.Serialize(request.MonitoredDisasterTypes);
        }

        existingRegion.UpdatedAt = DateTime.UtcNow;

        // Save to database
        var updatedRegion = await _regionRepository.UpdateAsync(existingRegion);

        // Return DTO directly from database
        return await MapToRegionResponse(updatedRegion);
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

    private async Task<RegionResponse> MapToRegionResponse(Region region)
    {
        var disasterTypeInfos = new List<DisasterTypeInfo>();

        try
        {
            if (!string.IsNullOrEmpty(region.MonitoredDisasterTypes))
            {
                var disasterTypeIds = JsonSerializer.Deserialize<List<int>>(region.MonitoredDisasterTypes);
                if (disasterTypeIds != null)
                {
                    // Fetch actual disaster type data from database using IDs
                    foreach (var id in disasterTypeIds)
                    {
                        var disasterType = await _disasterTypeRepository.GetByIdAsync(id);
                        if (disasterType != null)
                        {
                            disasterTypeInfos.Add(new DisasterTypeInfo
                            {
                                Id = disasterType.Id,
                                Name = disasterType.Name
                            });
                        }
                    }
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
}
