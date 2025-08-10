namespace Core.Models;

public class AlertSetting
{
    public int Id { get; set; }
    public int RegionId { get; set; }
    public int DisasterTypeId { get; set; }
    
    // Navigation properties
    public virtual Region Region { get; set; } = null!;
    public virtual DisasterType DisasterType { get; set; } = null!;
    
    public double ThresholdRiskScore { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
