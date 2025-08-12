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
}
