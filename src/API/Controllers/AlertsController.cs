using Core.DTOs;
using Core.Services;
using Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace API.Controllers;

/// <summary>
/// Controller for managing disaster alerts
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class AlertsController : ControllerBase
{
    private readonly IAlertService _alertService;

    public AlertsController(IAlertService alertService)
    {
        _alertService = alertService;
    }

    /// <summary>
    /// Sends a disaster alert to a region and stores it in the database
    /// </summary>
    /// <param name="request">The alert request</param>
    /// <returns>The created alert information</returns>
    /// <response code="201">Alert sent successfully</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="500">Internal server error</response>
    [HttpPost("send")]
    [ProducesResponseType(typeof(AlertResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AlertResponse>> SendAlert(SendAlertRequest request)
    {
        try
        {
            var alert = await _alertService.SendAlertAsync(request);
            return CreatedAtAction(nameof(GetAlert), new { id = alert.Id }, alert);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Failed to send alert", error = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves a specific alert by ID
    /// </summary>
    /// <param name="id">The unique identifier of the alert</param>
    /// <returns>The alert information</returns>
    /// <response code="200">Alert found successfully</response>
    /// <response code="404">Alert not found</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(AlertResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AlertResponse>> GetAlert(int id)
    {
        try
        {
            var alerts = await _alertService.GetAllAlertsAsync();
            var alert = alerts.FirstOrDefault(a => a.Id == id);

            if (alert == null)
                return NotFound(new { message = "Alert not found" });

            return Ok(alert);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves all alerts from the database
    /// </summary>
    /// <returns>List of all alerts</returns>
    /// <response code="200">Alerts retrieved successfully</response>
    /// <response code="500">Internal server error</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AlertResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AlertResponse>>> GetAllAlerts()
    {
        try
        {
            var alerts = await _alertService.GetAllAlertsAsync();
            return Ok(alerts);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves alerts for a specific region
    /// </summary>
    /// <param name="regionId">The ID of the region</param>
    /// <returns>List of alerts for the region</returns>
    /// <response code="200">Alerts retrieved successfully</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("region/{regionId}")]
    [ProducesResponseType(typeof(IEnumerable<AlertResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AlertResponse>>> GetAlertsByRegion(int regionId)
    {
        try
        {
            var alerts = await _alertService.GetAlertsByRegionAsync(regionId);
            return Ok(alerts);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves alerts by disaster type
    /// </summary>
    /// <param name="disasterType">The type of disaster</param>
    /// <returns>List of alerts for the disaster type</returns>
    /// <response code="200">Alerts retrieved successfully</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("disaster-type/{disasterType}")]
    [ProducesResponseType(typeof(IEnumerable<AlertResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AlertResponse>>> GetAlertsByDisasterType(int disasterType)
    {
        try
        {
            if (!Enum.IsDefined(typeof(DisasterTypeEnum), disasterType))
                return BadRequest(new { message = "Invalid disaster type" });

            var alerts = await _alertService.GetAlertsByDisasterTypeAsync(disasterType);
            return Ok(alerts);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves alerts within a date range
    /// </summary>
    /// <param name="startDate">Start date (ISO 8601 format)</param>
    /// <param name="endDate">End date (ISO 8601 format)</param>
    /// <returns>List of alerts within the date range</returns>
    /// <response code="200">Alerts retrieved successfully</response>
    /// <response code="400">Invalid date format</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("date-range")]
    [ProducesResponseType(typeof(IEnumerable<AlertResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<AlertResponse>>> GetAlertsByDateRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        try
        {
            if (startDate >= endDate)
                return BadRequest(new { message = "Start date must be before end date" });

            var alerts = await _alertService.GetAlertsByDateRangeAsync(startDate, endDate);
            return Ok(alerts);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }
}
