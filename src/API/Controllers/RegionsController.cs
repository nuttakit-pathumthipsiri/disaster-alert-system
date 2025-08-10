using Core.DTOs;
using Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers;

/// <summary>
/// Controller for managing disaster alert regions
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class RegionsController : ControllerBase
{
    private readonly IRegionService _regionService;

    public RegionsController(IRegionService regionService)
    {
        _regionService = regionService;
    }

    /// <summary>
    /// Creates a new disaster alert region
    /// </summary>
    /// <param name="request">The region creation request</param>
    /// <returns>The created region information</returns>
    /// <response code="201">Region created successfully</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="500">Internal server error</response>
    [HttpPost]
    [ProducesResponseType(typeof(RegionResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RegionResponse>> CreateRegion(CreateRegionRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid request data", errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            }

            var region = await _regionService.CreateRegionAsync(request);
            return CreatedAtAction(nameof(GetRegion), new { id = region.Id }, region);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { message = "Validation error", error = ex.Message });
        }
        catch (Exception ex)
        {
            // TODO: Add proper logging
            return StatusCode(500, new { message = "Failed to create region", error = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves a specific region by ID
    /// </summary>
    /// <param name="id">The unique identifier of the region</param>
    /// <returns>The region information</returns>
    /// <response code="200">Region found successfully</response>
    /// <response code="404">Region not found</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(RegionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RegionResponse>> GetRegion(int id)
    {
        try
        {
            var region = await _regionService.GetRegionAsync(id);
            if (region == null)
                return NotFound(new { message = "Region not found" });
            return Ok(region);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { message = "Validation error", error = ex.Message });
        }
        catch (Exception ex)
        {
            // TODO: Add proper logging
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves all available regions
    /// </summary>
    /// <returns>List of all regions</returns>
    /// <response code="200">Regions retrieved successfully</response>
    /// <response code="500">Internal server error</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<RegionResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<RegionResponse>>> GetAllRegions()
    {
        try
        {
            var regions = await _regionService.GetAllRegionsAsync();
            return Ok(regions);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { message = "Validation error", error = ex.Message });
        }
        catch (Exception ex)
        {
            // TODO: Add proper logging
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }

    /// <summary>
    /// Updates an existing region
    /// </summary>
    /// <param name="id">The unique identifier of the region to update</param>
    /// <param name="request">The region update request</param>
    /// <returns>The updated region information</returns>
    /// <response code="200">Region updated successfully</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="404">Region not found</response>
    /// <response code="500">Internal server error</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(RegionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RegionResponse>> UpdateRegion(int id, UpdateRegionRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid request data", errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            }

            var region = await _regionService.UpdateRegionAsync(id, request);
            if (region == null)
                return NotFound(new { message = "Region not found" });
            return Ok(region);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { message = "Validation error", error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }

    /// <summary>
    /// Deletes a region
    /// </summary>
    /// <param name="id">The unique identifier of the region to delete</param>
    /// <returns>No content on successful deletion</returns>
    /// <response code="204">Region deleted successfully</response>
    /// <response code="404">Region not found</response>
    /// <response code="500">Internal server error</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteRegion(int id)
    {
        try
        {
            var deleted = await _regionService.DeleteRegionAsync(id);
            if (!deleted)
                return NotFound(new { message = "Region not found" });
            return NoContent();
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { message = "Validation error", error = ex.Message });
        }
        catch (Exception ex)
        {
            // TODO: Add proper logging
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }
}
