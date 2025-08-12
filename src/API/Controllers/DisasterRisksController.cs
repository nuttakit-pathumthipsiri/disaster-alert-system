using Microsoft.AspNetCore.Mvc;
using Core.Services;
using Core.DTOs;

namespace API.Controllers;


[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class DisasterRisksController : ControllerBase
{
    private readonly IDisasterRisksService _disasterRiskService;
    private readonly ILogger<DisasterRisksController> _logger;

    public DisasterRisksController(
        IDisasterRisksService disasterRiskService,
        ILogger<DisasterRisksController> logger)
    {
        _disasterRiskService = disasterRiskService;
        _logger = logger;
    }

    /// <returns>List of disaster risk reports with Region ID, Disaster Type, Risk Score, Risk Level, and Alert Triggered status</returns>
    /// <response code="200">Risk reports retrieved successfully</response>
    /// <response code="500">Internal server error</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<DisasterRiskReportResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<DisasterRiskReportResponse>>> GetDisasterRisks()
    {
        try
        {
            var disasterRiskReports = await _disasterRiskService.GetDisasterRiskReportsAsync();
            return Ok(disasterRiskReports);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve disaster risk reports");
            return StatusCode(500, new { message = "Failed to retrieve disaster risk reports" });
        }
    }
}
