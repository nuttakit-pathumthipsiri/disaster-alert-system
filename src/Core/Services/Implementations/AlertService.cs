using Core.DTOs;
using Core.Models;
using Core.Interfaces;
using Core.Services;
using Core.Utilities;
using Microsoft.Extensions.Logging;

namespace Core.Services.Implementations;

public class AlertService : IAlertService
{
    private readonly IAlertRepository _alertRepository;
    private readonly IEmailService _emailService;
    private readonly IUserRepository _userRepository;
    private readonly IRegionService _regionService;
    private readonly IDisasterTypeRepository _disasterTypeRepository;
    private readonly ILogger<AlertService> _logger;

    public AlertService(
        IAlertRepository alertRepository,
        IEmailService emailService,
        IUserRepository userRepository,
        IRegionService regionService,
        IDisasterTypeRepository disasterTypeRepository,
        ILogger<AlertService> logger)
    {
        _alertRepository = alertRepository;
        _emailService = emailService;
        _userRepository = userRepository;
        _regionService = regionService;
        _disasterTypeRepository = disasterTypeRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<AlertResponse>> GetAlertsAsync(int? regionId = null, int? disasterTypeId = null, bool pendingOnly = false)
    {
        try
        {
            var alerts = await _alertRepository.GetByFiltersAsync(regionId, disasterTypeId, pendingOnly);

            var responses = new List<AlertResponse>();
            foreach (var alert in alerts)
            {
                responses.Add(await MapToResponseAsync(alert));
            }

            return responses;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting alerts with filters: RegionId={RegionId}, DisasterTypeId={DisasterTypeId}, PendingOnly={PendingOnly}",
                regionId, disasterTypeId, pendingOnly);
            throw;
        }
    }

    public async Task<IEnumerable<AlertResponse>> GetAllAlertsAsync()
    {
        try
        {
            var alerts = await _alertRepository.GetAllAsync();

            var responses = new List<AlertResponse>();
            foreach (var alert in alerts)
            {
                responses.Add(await MapToResponseAsync(alert));
            }

            return responses;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all alerts");
            throw;
        }
    }

    public async Task<IEnumerable<AlertResponse>> GetAlertsByRegionAsync(int regionId)
    {
        try
        {
            var alerts = await _alertRepository.GetByRegionAsync(regionId);

            var responses = new List<AlertResponse>();
            foreach (var alert in alerts)
            {
                responses.Add(await MapToResponseAsync(alert));
            }

            return responses;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting alerts for region {RegionId}", regionId);
            throw;
        }
    }

    public async Task<IEnumerable<AlertResponse>> GetAlertsByDisasterTypeAsync(int disasterTypeId)
    {
        try
        {
            var alerts = await _alertRepository.GetByDisasterTypeAsync(disasterTypeId);

            var responses = new List<AlertResponse>();
            foreach (var alert in alerts)
            {
                responses.Add(await MapToResponseAsync(alert));
            }

            return responses;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting alerts for disaster type {DisasterTypeId}", disasterTypeId);
            throw;
        }
    }

    public async Task<IEnumerable<AlertResponse>> GetPendingAlertsAsync()
    {
        try
        {
            var alerts = await _alertRepository.GetPendingAlertsAsync();

            var responses = new List<AlertResponse>();
            foreach (var alert in alerts)
            {
                responses.Add(await MapToResponseAsync(alert));
            }

            return responses;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pending alerts");
            throw;
        }
    }

    public async Task<AlertResponse> SendAlertAsync(SendDisasterAlertRequest request)
    {
        try
        {
            _logger.LogInformation("Sending alert for region {RegionId} and disaster type {DisasterTypeId}",
                request.RegionId, request.DisasterTypeId);

            // Find existing alert for the region and disaster type
            var existingAlerts = await _alertRepository.GetByFiltersAsync(request.RegionId, request.DisasterTypeId ?? 1, false);
            var existingAlert = existingAlerts.FirstOrDefault();

            if (existingAlert == null)
            {
                throw new InvalidOperationException($"No alert found for region {request.RegionId} and disaster type {request.DisasterTypeId ?? 1}");
            }

            // Send email notification
            try
            {
                await SendEmailNotificationAsync(existingAlert);
                existingAlert.EmailSent = true;
                existingAlert.EmailSentAt = DateTime.UtcNow;
                await _alertRepository.UpdateAsync(existingAlert);
            }
            catch (Exception emailEx)
            {
                _logger.LogWarning(emailEx, "Failed to send email notification for alert {AlertId}", existingAlert.Id);
                // Don't fail the entire operation if email fails
            }

            _logger.LogInformation("Successfully sent alert with ID {AlertId}", existingAlert.Id);

            return await MapToResponseAsync(existingAlert);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send alert for region {RegionId}", request.RegionId);
            throw;
        }
    }

    public async Task<AlertResponse> CreateAlertAsync(
        int regionId,
        int disasterTypeId,
        double riskScore,
        double thresholdValue,
        string? externalApiData = null)
    {
        try
        {
            _logger.LogInformation("Creating alert for region {RegionId} and disaster type {DisasterTypeId}",
                regionId, disasterTypeId);

            var riskLevel = RiskAssessmentUtility.DetermineRiskLevel(riskScore);
            var alert = new Alert
            {
                RegionId = regionId,
                DisasterTypeId = disasterTypeId,
                RiskScore = riskScore,
                RiskLevel = riskLevel,
                ThresholdValue = thresholdValue,
                AlertMessage = RiskAssessmentUtility.GenerateAlertMessage(riskLevel, riskScore, thresholdValue),
                DetectedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(24),
                Metadata = externalApiData
            };

            var createdAlert = await _alertRepository.CreateAsync(alert);

            _logger.LogInformation("Successfully created alert with ID {AlertId}", createdAlert.Id);

            return await MapToResponseAsync(createdAlert);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create alert for region {RegionId}", regionId);
            throw;
        }
    }

    // Risk level determination and alert message generation moved to RiskAssessmentUtility

    private async Task<AlertResponse> MapToResponseAsync(Alert alert)
    {
        try
        {
            // Get region and disaster type data from repositories
            var region = await _regionService.GetRegionAsync(alert.RegionId);
            var disasterType = await _disasterTypeRepository.GetByIdAsync(alert.DisasterTypeId);

            return new AlertResponse
            {
                Id = alert.Id,
                RegionId = alert.RegionId,
                RegionName = region?.Name ?? $"Region {alert.RegionId}",
                DisasterTypeId = alert.DisasterTypeId,
                DisasterTypeName = disasterType?.Name ?? $"Disaster Type {alert.DisasterTypeId}",
                RiskScore = alert.RiskScore,
                RiskLevel = alert.RiskLevel.ToString(),
                ThresholdValue = alert.ThresholdValue,
                EmailSent = alert.EmailSent,
                EmailSentAt = alert.EmailSentAt,
                AlertMessage = alert.AlertMessage,
                DetectedAt = alert.DetectedAt,
                ExpiresAt = alert.ExpiresAt,
                Metadata = alert.Metadata
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get region or disaster type data for alert {AlertId}, using placeholder names", alert.Id);

            // Fallback to placeholder names if data retrieval fails
            return new AlertResponse
            {
                Id = alert.Id,
                RegionId = alert.RegionId,
                RegionName = $"Region {alert.RegionId}",
                DisasterTypeId = alert.DisasterTypeId,
                DisasterTypeName = $"Disaster Type {alert.DisasterTypeId}",
                RiskScore = alert.RiskScore,
                RiskLevel = alert.RiskLevel.ToString(),
                ThresholdValue = alert.ThresholdValue,
                EmailSent = alert.EmailSent,
                EmailSentAt = alert.EmailSentAt,
                AlertMessage = alert.AlertMessage,
                DetectedAt = alert.DetectedAt,
                ExpiresAt = alert.ExpiresAt,
                Metadata = alert.Metadata
            };
        }
    }

    public async Task<IEnumerable<AlertResponse>> GetAlertHistoryAsync(
        int? regionId = null,
        int? disasterTypeId = null,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        try
        {
            // This would need to be implemented in the repository
            // For now, we'll use the existing method
            var alerts = await _alertRepository.GetByFiltersAsync(regionId, disasterTypeId, false);

            var responses = new List<AlertResponse>();
            foreach (var alert in alerts)
            {
                responses.Add(await MapToResponseAsync(alert));
            }

            return responses;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting alert history");
            throw;
        }
    }

    private async Task SendEmailNotificationAsync(Alert alert)
    {
        try
        {
            _logger.LogInformation("Sending email notification for alert {AlertId} to relevant users", alert.Id);

            // Get users in the region to send alerts to
            var usersInRegion = await _userRepository.GetByRegionAsync(alert.RegionId);

            if (!usersInRegion.Any())
            {
                _logger.LogWarning("No users found in region {RegionId} for alert {AlertId}", alert.RegionId, alert.Id);
                return;
            }

            // Get region and disaster type information for the email
            var region = await _regionService.GetRegionAsync(alert.RegionId);
            var disasterType = await _disasterTypeRepository.GetByIdAsync(alert.DisasterTypeId);

            if (region == null || disasterType == null)
            {
                _logger.LogWarning("Region or disaster type not found for alert {AlertId}", alert.Id);
                return;
            }

            // Create a DisasterRiskReportResponse for email content
            var report = new DisasterRiskReportResponse
            {
                RegionId = alert.RegionId,
                RegionName = region.Name,
                DisasterTypeId = alert.DisasterTypeId,
                DisasterTypeName = disasterType.Name,
                RiskScore = alert.RiskScore,
                RiskLevel = alert.RiskLevel.ToString(),
                ThresholdValue = alert.ThresholdValue,
                AlertTriggered = true,
                CalculatedAt = alert.DetectedAt,
                ExpiresAt = alert.ExpiresAt,
                ExternalApiData = alert.ExternalApiData
            };

            // Send emails to all users in the region
            foreach (var user in usersInRegion)
            {
                try
                {
                    await _emailService.SendDisasterAlertEmailAsync(user, report, region.Name);
                    _logger.LogInformation("Disaster alert email sent successfully to user {UserId} for alert {AlertId}",
                        user.Id, alert.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send disaster alert email to user {UserId} for alert {AlertId}",
                        user.Id, alert.Id);
                    // Continue sending to other users even if one fails
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email notifications for alert {AlertId}", alert.Id);
            // Don't fail the entire operation if email sending fails
        }
    }
}
