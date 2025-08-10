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
    public DbSet<DisasterRisk> DisasterRisks { get; set; }
    public DbSet<Alert> Alerts { get; set; }

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

        // Remove the many-to-many relationship configuration since we're using JSON storage
        // modelBuilder.Entity<Region>()
        //     .HasMany(r => r.MonitoredDisasterTypes)
        //     .WithMany(dt => dt.Regions)
        //     .UsingEntity(j => j.ToTable("RegionDisasterTypes"));

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

        // Configure DisasterRisk entity
        modelBuilder.Entity<DisasterRisk>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RiskScore).IsRequired();
            entity.Property(e => e.CalculatedAt).IsRequired();
            entity.Property(e => e.ExpiresAt).IsRequired();

            entity.HasOne(e => e.Region)
                  .WithMany(r => r.DisasterRisks)
                  .HasForeignKey(e => e.RegionId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.DisasterType)
                  .WithMany(dt => dt.DisasterRisks)
                  .HasForeignKey(e => e.DisasterTypeId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Alert entity
        modelBuilder.Entity<Alert>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RiskScore).IsRequired();
            entity.Property(e => e.AlertMessage).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.SentAt).IsRequired();

            entity.HasOne(e => e.Region)
                  .WithMany()
                  .HasForeignKey(e => e.RegionId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.DisasterType)
                  .WithMany()
                  .HasForeignKey(e => e.DisasterTypeId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
