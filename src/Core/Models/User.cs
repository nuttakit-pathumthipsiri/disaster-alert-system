namespace Core.Models;

/// <summary>
/// User entity for disaster alert notifications
/// </summary>
public class User
{
    /// <summary>
    /// Unique identifier for the user
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Region ID where the user is located
    /// </summary>
    public int RegionId { get; set; }

    /// <summary>
    /// User's email address for notifications
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User's phone number for SMS notifications
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// Whether the user is active and can receive notifications
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// When the user record was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the user record was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    /// <summary>
    /// The region where the user is located
    /// </summary>
    public virtual Region? Region { get; set; }
}
