using Microsoft.AspNetCore.Mvc;
using Core.Services;
using Core.DTOs;
using Core.Models;

namespace API.Controllers;

/// <summary>
/// Controller for managing disaster risk assessments
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class DisasterRisksController : ControllerBase
{
    private readonly IDisasterRiskService _disasterRiskService;
    private readonly IDisasterTypeRepository _disasterTypeRepository;

    public DisasterRisksController(
        IDisasterRiskService disasterRiskService,
        IDisasterTypeRepository disasterTypeRepository)
    {
        _disasterRiskService = disasterRiskService;
        _disasterTypeRepository = disasterTypeRepository;
    }

    /// <summary>
    /// Calculates disaster risks for all active regions and returns real-time risk assessments
    /// </summary>
    /// <returns>List of disaster risk assessments for all regions</returns>
    /// <response code="200">Risk assessments calculated successfully</response>
    /// <response code="500">Internal server error</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<DisasterRiskResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<DisasterRiskResponse>>> GetDisasterRisks()
    {
        try
        {
            var disasterRisks = await _disasterRiskService.CalculateAllDisasterRisksAsync();
            return Ok(disasterRisks);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Failed to calculate disaster risks", error = ex.Message });
        }
    }

    /// <summary>
    /// Calculates disaster risk for a specific region and disaster type
    /// </summary>
    /// <param name="regionId">The ID of the region</param>
    /// <param name="disasterType">The type of disaster to assess</param>
    /// <returns>Disaster risk assessment for the specific region and disaster type</returns>
    /// <response code="200">Risk assessment calculated successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("{regionId}/{disasterType}")]
    [ProducesResponseType(typeof(DisasterRiskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DisasterRiskResponse>> GetDisasterRisk(int regionId, int disasterType)
    {
        try
        {
            if (regionId <= 0)
                return BadRequest(new { message = "Invalid region ID" });

            if (!Enum.IsDefined(typeof(DisasterTypeEnum), disasterType))
                return BadRequest(new { message = "Invalid disaster type" });

            // Get the DisasterType entity from repository
            var disasterTypeEntity = await _disasterTypeRepository.GetByIdAsync(disasterType);
            if (disasterTypeEntity == null)
                return BadRequest(new { message = "Disaster type not found" });

            var disasterRisk = await _disasterRiskService.CalculateDisasterRiskAsync(regionId, disasterTypeEntity);

            return Ok(disasterRisk);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Failed to calculate disaster risk", error = ex.Message });
        }
    }

    /// <summary>
    /// Gets cached disaster risk data for a specific region and disaster type
    /// </summary>
    /// <param name="regionId">The ID of the region</param>
    /// <param name="disasterType">The type of disaster</param>
    /// <returns>Cached disaster risk data if available and not expired</returns>
    /// <response code="200">Cached risk data retrieved successfully</response>
    /// <response code="404">No cached data found or data expired</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("cached/{regionId}/{disasterType}")]
    [ProducesResponseType(typeof(Core.Models.DisasterRisk), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Core.Models.DisasterRisk>> GetCachedDisasterRisk(int regionId, int disasterType)
    {
        try
        {
            if (regionId <= 0)
                return BadRequest(new { message = "Invalid region ID" });

            if (!Enum.IsDefined(typeof(DisasterTypeEnum), disasterType))
                return BadRequest(new { message = "Invalid disaster type" });

            // Get the DisasterType entity from repository
            var disasterTypeEntity = await _disasterTypeRepository.GetByIdAsync(disasterType);
            if (disasterTypeEntity == null)
                return BadRequest(new { message = "Disaster type not found" });

            var cachedRisk = await _disasterRiskService.GetCachedDisasterRiskAsync(regionId, disasterTypeEntity);

            if (cachedRisk == null)
                return NotFound(new { message = "No cached risk data found or data expired" });

            return Ok(cachedRisk);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Failed to retrieve cached disaster risk", error = ex.Message });
        }
    }
}
