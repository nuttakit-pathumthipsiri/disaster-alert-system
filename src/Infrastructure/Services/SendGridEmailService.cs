using Core.Models;
using Core.DTOs;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Infrastructure.Services;

public class SendGridEmailService : IEmailService
{
    private readonly ISendGridClient _sendGridClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SendGridEmailService> _logger;
    private readonly string _fromEmail;
    private readonly string _fromName;

    public SendGridEmailService(
        ISendGridClient sendGridClient,
        IConfiguration configuration,
        ILogger<SendGridEmailService> logger)
    {
        _sendGridClient = sendGridClient;
        _configuration = configuration;
        _logger = logger;
        _fromEmail = _configuration["ExternalApis:SendGrid:FromEmail"] ?? "alerts@disaster-alert-system.com";
        _fromName = _configuration["ExternalApis:SendGrid:FromName"] ?? "Disaster Alert System";
    }

    public async Task SendDisasterAlertEmailAsync(User user, DisasterRiskReportResponse report, string regionName)
    {
        try
        {
            _logger.LogInformation("Sending disaster alert email to {UserEmail} for region {RegionName}", user.Email, regionName);
            var subject = $"🚨 DISASTER ALERT: {report.DisasterTypeName} Risk in {regionName}";

            var htmlContent = GenerateAlertEmailHtml(user, report, regionName);
            var plainTextContent = GenerateAlertEmailPlainText(user, report, regionName);

            var message = MailHelper.CreateSingleEmail(
                new EmailAddress(_fromEmail, _fromName),
                new EmailAddress(user.Email, user.Email),
                subject,
                plainTextContent,
                htmlContent
            );

            var response = await _sendGridClient.SendEmailAsync(message);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Disaster alert email sent successfully to {UserEmail} for region {RegionName}",
                    user.Email, regionName);
            }
            else
            {
                _logger.LogWarning("Failed to send disaster alert email to {UserEmail}. Status: {StatusCode}",
                    user.Email, response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending disaster alert email to {UserEmail}", user.Email);
        }
    }

    public Task SendDisasterAlertEmailsToRegionAsync(int regionId, DisasterRiskReportResponse report)
    {
        try
        {
            // This method will be called from DisasterRiskAssessmentService
            // The actual user retrieval should be handled by the calling service
            _logger.LogInformation("Disaster alert emails will be sent to users in region {RegionId} for {DisasterType}",
                regionId, report.DisasterTypeName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error preparing disaster alert emails for region {RegionId}", regionId);
        }

        return Task.CompletedTask;
    }

    private string GenerateAlertEmailHtml(User user, DisasterRiskReportResponse report, string regionName)
    {
        return $@"
            <!DOCTYPE html>
            <html>
            <head>
                <title>Test Alert</title>
            </head>
            <body>
                <h1>Test Disaster Alert</h1>
                <p>Hello {user.Email},</p>
                <p>This is a test alert for {report.DisasterTypeName} in {regionName}.</p>
                <p>Risk Level: {report.RiskLevel}</p>
                <p>Test completed at {DateTime.UtcNow:HH:mm:ss}</p>
            </body>
            </html>";
    }

    private string GenerateAlertEmailPlainText(User user, DisasterRiskReportResponse report, string regionName)
    {
        return $@"
Test Disaster Alert

Hello {user.Email},

This is a test alert for {report.DisasterTypeName} in {regionName}.

Risk Level: {report.RiskLevel}

Test completed at {DateTime.UtcNow:HH:mm:ss}";
    }

    public async Task<bool> SendAlertEmailAsync(string toEmail, string subject, string message, int alertId)
    {
        try
        {
            _logger.LogInformation("Sending alert email to {ToEmail} for alert {AlertId}", toEmail, alertId);

            var htmlContent = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <title>{subject}</title>
                </head>
                <body>
                    <h1>{subject}</h1>
                    <div>{message}</div>
                    <p>Alert ID: {alertId}</p>
                    <p>Sent at: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC</p>
                </body>
                </html>";

            var emailMessage = MailHelper.CreateSingleEmail(
                new EmailAddress(_fromEmail, _fromName),
                new EmailAddress(toEmail),
                subject,
                message,
                htmlContent
            );

            var response = await _sendGridClient.SendEmailAsync(emailMessage);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Alert email sent successfully to {ToEmail} for alert {AlertId}", toEmail, alertId);
                return true;
            }
            else
            {
                _logger.LogWarning("Failed to send alert email to {ToEmail} for alert {AlertId}. Status: {StatusCode}",
                    toEmail, alertId, response.StatusCode);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending alert email to {ToEmail} for alert {AlertId}", toEmail, alertId);
            return false;
        }
    }

    public async Task<bool> SendBulkAlertEmailAsync(IEnumerable<string> toEmails, string subject, string message, int alertId)
    {
        try
        {
            _logger.LogInformation("Sending bulk alert emails to {EmailCount} recipients for alert {AlertId}", toEmails.Count(), alertId);

            var htmlContent = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <title>{subject}</title>
                </head>
                <body>
                    <h1>{subject}</h1>
                    <div>{message}</div>
                    <p>Alert ID: {alertId}</p>
                    <p>Sent at: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC</p>
                </body>
                </html>";

            var emailMessages = new List<SendGridMessage>();
            foreach (var toEmail in toEmails)
            {
                var emailMessage = MailHelper.CreateSingleEmail(
                    new EmailAddress(_fromEmail, _fromName),
                    new EmailAddress(toEmail),
                    subject,
                    message,
                    htmlContent
                );
                emailMessages.Add(emailMessage);
            }

            var tasks = emailMessages.Select(msg => _sendGridClient.SendEmailAsync(msg));
            var responses = await Task.WhenAll(tasks);

            var successCount = responses.Count(r => r.IsSuccessStatusCode);
            var failureCount = responses.Length - successCount;

            _logger.LogInformation("Bulk alert emails completed for alert {AlertId}. Success: {SuccessCount}, Failed: {FailureCount}",
                alertId, successCount, failureCount);

            return failureCount == 0; // Return true only if all emails were sent successfully
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending bulk alert emails for alert {AlertId}", alertId);
            return false;
        }
    }
}
