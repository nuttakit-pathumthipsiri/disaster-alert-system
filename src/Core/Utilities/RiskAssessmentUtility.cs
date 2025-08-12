using Core.Models;

namespace Core.Utilities;

public static class RiskAssessmentUtility
{
    public static RiskLevel DetermineRiskLevel(double riskScore)
    {
        if (riskScore >= 0.8) return RiskLevel.High;
        if (riskScore >= 0.6) return RiskLevel.Medium;
        if (riskScore >= 0.4) return RiskLevel.Medium;
        if (riskScore >= 0.2) return RiskLevel.Low;
        return RiskLevel.Low;
    }

    public static string GenerateAlertMessage(RiskLevel riskLevel, double riskScore, double thresholdValue)
    {
        var baseMessage = $"Disaster risk detected with {riskLevel} level (Score: {riskScore:F2}, Threshold: {thresholdValue:F2})";

        return riskLevel switch
        {
            RiskLevel.High => $"⚠️ HIGH: {baseMessage} - Urgent attention needed!",
            RiskLevel.Medium => $"⚡ MEDIUM: {baseMessage} - Monitor closely!",
            RiskLevel.Low => $"📊 LOW: {baseMessage} - Stay alert!",
            _ => $"ℹ️ LOW: {baseMessage} - No immediate action required."
        };
    }

    public static string DetermineRiskLevelString(double riskScore)
    {
        if (riskScore >= 0.8) return "Critical";
        if (riskScore >= 0.6) return "High";
        if (riskScore >= 0.4) return "Medium";
        if (riskScore >= 0.2) return "Low";
        return "Minimal";
    }
}
