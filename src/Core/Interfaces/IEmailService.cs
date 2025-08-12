using Core.Models;
using Core.DTOs;

namespace Core.Interfaces;

public interface IEmailService
{
    Task SendDisasterAlertEmailAsync(User user, DisasterRiskReportResponse report, string regionName);
}
