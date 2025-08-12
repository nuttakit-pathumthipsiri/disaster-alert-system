using Core.Models;
using Core.DTOs;

namespace Core.Interfaces;

public interface IEmailService
{
    Task<bool> SendAlertEmailAsync(string toEmail, string subject, string message, int alertId);
    Task<bool> SendBulkAlertEmailAsync(IEnumerable<string> toEmails, string subject, string message, int alertId);
    Task SendDisasterAlertEmailAsync(User user, DisasterRiskReportResponse report, string regionName);
}
