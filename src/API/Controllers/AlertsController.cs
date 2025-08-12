using Core.DTOs;
using Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace API.Controllers;


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

    /// <param name="regionId">Optional: The ID of the region to filter by</param>
    /// <param name="disasterTypeId">Optional: The ID of the disaster type to filter by</param>
    /// <param name="pendingOnly">Optional: If true, returns only pending alerts that haven't had emails sent yet</param>
    /// <returns>List of alerts based on the filters</returns>
    /// <response code="200">Alerts retrieved successfully</response>
    /// <response code="500">Internal server error</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AlertResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AlertResponse>>> GetAlerts(
        [FromQuery] int? regionId = null,
        [FromQuery] int? disasterTypeId = null,
        [FromQuery] bool pendingOnly = false)
    {
        try
        {
            var alerts = await _alertService.GetAlertsAsync(regionId, disasterTypeId, pendingOnly);
            return Ok(alerts);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }

    /// <param name="request">The send alert request (RegionId is required, DisasterTypeId is optional)</param>
    /// <returns>The alert information</returns>
    /// <response code="200">Alert sent successfully</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="500">Internal server error</response>
    [HttpPost("send")]
    [ProducesResponseType(typeof(AlertResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AlertResponse>> SendAlert(SendDisasterAlertRequest request)
    {
        try
        {
            var alert = await _alertService.SendAlertAsync(request);
            return Ok(alert);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = "Failed to send alert", error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }

    /// <param name="regionId">Optional: Filter by region ID</param>
    /// <param name="disasterTypeId">Optional: Filter by disaster type ID</param>
    /// <param name="startDate">Optional: Filter by start date</param>
    /// <param name="endDate">Optional: Filter by end date</param>
    /// <returns>List of sent alerts (alert history)</returns>
    /// <response code="200">Alert history retrieved successfully</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("history")]
    [ProducesResponseType(typeof(IEnumerable<AlertResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AlertResponse>>> GetAlertHistory(
        [FromQuery] int? regionId = null,
        [FromQuery] int? disasterTypeId = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        try
        {
            var alerts = await _alertService.GetAlertHistoryAsync(regionId, disasterTypeId, startDate, endDate);
            return Ok(alerts);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }
}
