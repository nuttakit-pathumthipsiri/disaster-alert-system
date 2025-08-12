using Core.DTOs;
using Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace API.Controllers;


[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class AlertSettingsController : ControllerBase
{
    private readonly IAlertSettingService _alertSettingService;

    public AlertSettingsController(IAlertSettingService alertSettingService)
    {
        _alertSettingService = alertSettingService;
    }

    /// <param name="request">The alert setting creation request</param>
    /// <returns>The created alert setting information</returns>
    /// <response code="201">Alert setting created successfully</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="500">Internal server error</response>
    [HttpPost]
    [ProducesResponseType(typeof(AlertSettingResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AlertSettingResponse>> CreateAlertSetting(CreateAlertSettingRequest request)
    {
        try
        {
            var alertSetting = await _alertSettingService.CreateAlertSettingAsync(request);
            return CreatedAtAction(nameof(GetAlertSetting), new { id = alertSetting.Id }, alertSetting);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Failed to create alert setting", error = ex.Message });
        }
    }

    /// <param name="id">The unique identifier of the alert setting</param>
    /// <returns>The alert setting information</returns>
    /// <response code="200">Alert setting found successfully</response>
    /// <response code="404">Alert setting not found</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(AlertSettingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AlertSettingResponse>> GetAlertSetting(int id)
    {
        try
        {
            var alertSettings = await _alertSettingService.GetAllAlertSettingsAsync();
            var alertSetting = alertSettings.FirstOrDefault(a => a.Id == id);

            if (alertSetting == null)
                return NotFound(new { message = "Alert setting not found" });

            return Ok(alertSetting);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }

    /// <returns>List of all alert settings</returns>
    /// <response code="200">Alert settings retrieved successfully</response>
    /// <response code="500">Internal server error</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AlertSettingResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AlertSettingResponse>>> GetAllAlertSettings()
    {
        try
        {
            var alertSettings = await _alertSettingService.GetAllAlertSettingsAsync();
            return Ok(alertSettings);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }

    /// <param name="id">The unique identifier of the alert setting to update</param>
    /// <param name="request">The updated alert setting data</param>
    /// <returns>The updated alert setting information</returns>
    /// <response code="200">Alert setting updated successfully</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="404">Alert setting not found</response>
    /// <response code="500">Internal server error</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(AlertSettingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AlertSettingResponse>> UpdateAlertSetting(int id, CreateAlertSettingRequest request)
    {
        try
        {
            var alertSetting = await _alertSettingService.UpdateAlertSettingAsync(id, request);
            return Ok(alertSetting);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Failed to update alert setting", error = ex.Message });
        }
    }

    /// <param name="id">The unique identifier of the alert setting to delete</param>
    /// <returns>No content on successful deletion</returns>
    /// <response code="204">Alert setting deleted successfully</response>
    /// <response code="404">Alert setting not found</response>
    /// <response code="500">Internal server error</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteAlertSetting(int id)
    {
        try
        {
            var deleted = await _alertSettingService.DeleteAlertSettingAsync(id);
            if (!deleted)
                return NotFound(new { message = "Alert setting not found" });

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }
}
