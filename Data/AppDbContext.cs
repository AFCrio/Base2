using Base2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Reflection.Emit;

namespace Base2.Data;

/// <summary>
/// Контекст бази даних для системи "Добовий наряд"
/// </summary>
public class AppDbContext : DbContext
{

    // Довідники
    public DbSet<Rank> Ranks { get; set; } = null!;
    public DbSet<Position> Positions { get; set; } = null!;
    public DbSet<Person> People { get; set; } = null!;
    public DbSet<Weapon> Weapons { get; set; } = null!;
    public DbSet<Vehicle> Vehicles { get; set; } = null!;
    public DbSet<Location> Locations { get; set; } = null!;

    // Робочі сутності
    public DbSet<DutyOrder> DutyOrders { get; set; } = null!;
    public DbSet<DutyTimeRange> DutyTimeRanges { get; set; } = null!;
    public DbSet<DutySectionNode> DutySectionNodes { get; set; } = null!;
    public DbSet<DutyAssignment> DutyAssignments { get; set; } = null!;

    // Шаблони
    public DbSet<DutyTemplate> DutyTemplates { get; set; } = null!;
    public DbSet<TemplateChangeLog> TemplateChangeLogs { get; set; } = null!;

    public static void EnsureDatabaseUpToDate()
    {
        using var db = new AppDbContext();
        db.Database.Migrate();
    }

    /// <summary>
    /// Configures the SQLite database connection.
    /// </summary>
    /// <param name="optionsBuilder">Options builder.</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string dbPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "DutyOrder.db"
        );

        optionsBuilder.UseSqlite($"Data Source={dbPath}");

#if DEBUG
        optionsBuilder.LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name }, LogLevel.Information, DbContextLoggerOptions.DefaultWithLocalTime | DbContextLoggerOptions.SingleLine); // ✅ Все SQL-запросы
       optionsBuilder.EnableSensitiveDataLogging(); // ✅ Показывать параметры запросов
       optionsBuilder.EnableDetailedErrors();       // ✅ Подробные ошибки
#endif



    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // =====================================================
        // RANK
        // =====================================================
        modelBuilder.Entity<Rank>(entity =>
        {
            entity.HasKey(e => e.RankId);

            entity.Property(e => e.RankName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.RankLevel)
                .IsRequired();

            entity.HasIndex(e => e.RankName)
                .IsUnique();
        });

        // =====================================================
        // POSITION
        // =====================================================
        modelBuilder.Entity<Position>(entity =>
        {
            entity.HasKey(e => e.PositionId);

            entity.Property(e => e.PositionName)
                .IsRequired()
                .HasMaxLength(500);
        });

        // =====================================================
        // PERSON
        // =====================================================
        modelBuilder.Entity<Person>(entity =>
        {
            entity.HasKey(e => e.PersonId);

            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.FirstName)
                .HasMaxLength(50);

            entity.Property(e => e.MiddleName)
                .HasMaxLength(50);

            // FK до Rank
            entity.HasOne(e => e.Rank)
                .WithMany(r => r.People)
                .HasForeignKey(e => e.RankId)
                .OnDelete(DeleteBehavior.Restrict);

            // FK до Position
            entity.HasOne(e => e.Position)
                .WithMany(p => p.People)
                .HasForeignKey(e => e.PositionId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.LastName);
        });

        // =====================================================
        // WEAPON
        // =====================================================
        modelBuilder.Entity<Weapon>(entity =>
        {
            entity.HasKey(e => e.WeaponId);

            entity.Property(e => e.WeaponType)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.WeaponNumber)
                .IsRequired()
                .HasMaxLength(50);

            // FK до Location
            entity.HasOne(e => e.StoredInLocation)
                .WithMany(l => l.StoredWeapons)
                .HasForeignKey(e => e.StoredInLocationId)
                .OnDelete(DeleteBehavior.SetNull);

            // FK до Person (закріплення)
            entity.HasOne(e => e.AssignedToPerson)
                .WithMany(p => p.AssignedWeapons)
                .HasForeignKey(e => e.AssignedToPersonId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.WeaponNumber)
                .IsUnique();
        });

        // =====================================================
        // VEHICLE
        // =====================================================
        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.VehicleId);

            entity.Property(e => e.VehicleName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.VehicleNumber)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.VehicleType)
                .HasMaxLength(50);

            entity.HasIndex(e => e.VehicleNumber)
                .IsUnique();
        });

        // =====================================================
        // LOCATION
        // =====================================================
        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.LocationId);

            entity.Property(e => e.LocationName)
                .IsRequired()
                .HasMaxLength(300);

            entity.Property(e => e.Address)
                .HasMaxLength(500);
        });

        // =====================================================
        // DUTY TEMPLATE
        // =====================================================
        modelBuilder.Entity<DutyTemplate>(entity =>
        {
            entity.HasKey(e => e.DutyTemplateId);

            entity.Property(e => e.TemplateName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Description)
                .HasMaxLength(1000);

            entity.Property(e => e.IsActive)
                .IsRequired();

            entity.Property(e => e.Version)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .IsRequired();

            entity.Property(e => e.UpdatedAt)
                .IsRequired();
        });

        // =====================================================
        // DUTY TIME RANGE
        // =====================================================
        modelBuilder.Entity<DutyTimeRange>(entity =>
        {
            entity.HasKey(e => e.DutyTimeRangeId);

            entity.Property(e => e.Label)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Start)
                .IsRequired();

            entity.Property(e => e.End)
                .IsRequired();

            // FK до DutyOrder
            entity.HasOne(e => e.DutyOrder)
                .WithMany(o => o.TimeRanges)
                .HasForeignKey(e => e.DutyOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Ігноруємо computed properties
            entity.Ignore(e => e.StartTime);
            entity.Ignore(e => e.StartDate);
            entity.Ignore(e => e.EndTime);
            entity.Ignore(e => e.EndDate);
        });

        // =====================================================
        // DUTY ORDER
        // =====================================================
        modelBuilder.Entity<DutyOrder>(entity =>
        {
            entity.HasKey(e => e.DutyOrderId);

            entity.Property(e => e.OrderNumber)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.OrderDate)
                .IsRequired();

            entity.Property(e => e.StartDateTime)
                .IsRequired();

            entity.Property(e => e.EndDateTime)
                .IsRequired();

            entity.Property(e => e.CommanderInfo)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.SourceTemplateVersion)
                .IsRequired();

            // FK до DutyTemplate (шаблон-джерело)
            entity.HasOne(e => e.SourceTemplate)
                .WithMany(t => t.Orders)
                .HasForeignKey(e => e.SourceTemplateId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.OrderNumber)
                .IsUnique();
        });

        // =====================================================
        // DUTY SECTION NODE
        // =====================================================
        modelBuilder.Entity<DutySectionNode>(entity =>
        {
            entity.HasKey(e => e.DutySectionNodeId);

            entity.Property(e => e.NodeType)
                .IsRequired()
                .HasConversion<string>();  // Зберігаємо enum як string

            entity.Property(e => e.OrderIndex)
                .IsRequired();

            entity.Property(e => e.Title)
                .HasMaxLength(50);

            entity.Property(e => e.DutyPositionTitle)
                .HasMaxLength(500);

            entity.Property(e => e.HasWeapon)
                .IsRequired();

            entity.Property(e => e.HasAmmo)
                .IsRequired();

            entity.Property(e => e.HasVehicle)
                .IsRequired();

            entity.Property(e => e.MaxAssignments)
                .IsRequired();

            entity.Property(e => e.TimeRangeLabel)
                .HasMaxLength(200);

            // FK до Parent (self-reference)
            entity.HasOne(e => e.Parent)
                .WithMany(p => p.Children)
                .HasForeignKey(e => e.ParentDutySectionNodeId)
                .OnDelete(DeleteBehavior.Restrict);

            // FK до DutyTemplate (для вузлів шаблону)
            entity.HasOne(e => e.DutyTemplate)
                .WithMany(t => t.Sections)
                .HasForeignKey(e => e.DutyTemplateId)
                .OnDelete(DeleteBehavior.Cascade);

            // FK до DutyOrder (для вузлів наказу)
            entity.HasOne(e => e.DutyOrder)
                .WithMany(o => o.Sections)
                .HasForeignKey(e => e.DutyOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // FK до DutyTimeRange
            entity.HasOne(e => e.DutyTimeRange)
                .WithMany()
                .HasForeignKey(e => e.DutyTimeRangeId)
                .OnDelete(DeleteBehavior.SetNull);

            // FK до Location (опціонально, для фільтрації зброї)
            entity.HasOne(e => e.Location)
                .WithMany(l => l.SectionNodes)
                .HasForeignKey(e => e.LocationId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.DutyTemplateId);
            entity.HasIndex(e => e.DutyOrderId);
            entity.HasIndex(e => e.ParentDutySectionNodeId);
            entity.HasIndex(e => new { e.ParentDutySectionNodeId, e.OrderIndex });
        });

        // =====================================================
        // DUTY ASSIGNMENT
        // =====================================================
        modelBuilder.Entity<DutyAssignment>(entity =>
        {
            entity.HasKey(e => e.DutyAssignmentId);

            entity.Property(e => e.AmmoCount)
                .IsRequired(false);

            entity.Property(e => e.AmmoType)
                .HasMaxLength(50);

            // FK до DutySectionNode
            entity.HasOne(e => e.DutySectionNode)
                .WithMany(n => n.Assignments)
                .HasForeignKey(e => e.DutySectionNodeId)
                .OnDelete(DeleteBehavior.Cascade);

            // FK до Person
            entity.HasOne(e => e.Person)
                .WithMany()
                .HasForeignKey(e => e.PersonId)
                .OnDelete(DeleteBehavior.Restrict);

            // FK до Weapon
            entity.HasOne(e => e.Weapon)
                .WithMany()
                .HasForeignKey(e => e.WeaponId)
                .OnDelete(DeleteBehavior.SetNull);

            // FK до Vehicle
            entity.HasOne(e => e.Vehicle)
                .WithMany()
                .HasForeignKey(e => e.VehicleId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.DutySectionNodeId);
            entity.HasIndex(e => e.PersonId);
        });

        // =====================================================
        // TEMPLATE CHANGE LOG
        // =====================================================
        modelBuilder.Entity<TemplateChangeLog>(entity =>
        {
            entity.HasKey(e => e.TemplateChangeLogId);

            entity.Property(e => e.Version)
                .IsRequired();

            entity.Property(e => e.ChangedAt)
                .IsRequired();

            entity.Property(e => e.ChangedBy)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.ChangeDescription)
                .IsRequired()
                .HasMaxLength(2000);

            // FK до DutyTemplate
            entity.HasOne(e => e.DutyTemplate)
                .WithMany(t => t.ChangeLogs)
                .HasForeignKey(e => e.DutyTemplateId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.DutyTemplateId);
            entity.HasIndex(e => new { e.DutyTemplateId, e.Version });
        });
    }
}
