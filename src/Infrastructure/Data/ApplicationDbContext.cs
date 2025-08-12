using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Region> Regions { get; set; }
    public DbSet<DisasterType> DisasterTypes { get; set; }
    public DbSet<AlertSetting> AlertSettings { get; set; }
    public DbSet<Alert> Alerts { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure DisasterType entity
        modelBuilder.Entity<DisasterType>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.IsActive).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
        });

        // Configure Region entity
        modelBuilder.Entity<Region>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Latitude).IsRequired();
            entity.Property(e => e.Longitude).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.MonitoredDisasterTypes).IsRequired().HasDefaultValue("[]");
        });

        // Configure AlertSetting entity
        modelBuilder.Entity<AlertSetting>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ThresholdRiskScore).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasOne(e => e.Region)
                  .WithMany(r => r.AlertSettings)
                  .HasForeignKey(e => e.RegionId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.DisasterType)
                  .WithMany(dt => dt.AlertSettings)
                  .HasForeignKey(e => e.DisasterTypeId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Alerts entity
        modelBuilder.Entity<Alert>(entity =>
        {
            entity.ToTable("Alerts");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RiskScore).IsRequired();
            entity.Property(e => e.ThresholdValue).IsRequired();
            entity.Property(e => e.EmailSent).IsRequired();
            entity.Property(e => e.AlertMessage).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.DetectedAt).IsRequired();
            entity.Property(e => e.ExpiresAt).IsRequired();
            entity.Property(e => e.ExternalApiData).HasMaxLength(4000);

            entity.HasOne(e => e.Region)
                  .WithMany()
                  .HasForeignKey(e => e.RegionId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.DisasterType)
                  .WithMany()
                  .HasForeignKey(e => e.DisasterTypeId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Add index for performance
            entity.HasIndex(e => new { e.RegionId, e.DisasterTypeId });
            entity.HasIndex(e => e.EmailSent);
            entity.HasIndex(e => e.DetectedAt);
        });

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Phone).IsRequired().HasMaxLength(20);
            entity.Property(e => e.IsActive).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasOne(e => e.Region)
                  .WithMany()
                  .HasForeignKey(e => e.RegionId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Make Email unique
            entity.HasIndex(e => e.Email).IsUnique();
        });
    }
}
