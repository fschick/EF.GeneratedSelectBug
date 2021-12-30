using EF.GeneratedSelectBug.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;

namespace EF.GeneratedSelectBug.DbContexts;

public class TimeTrackingDbContext : DbContext
{
    private readonly ILoggerFactory _loggerFactory;

    public TimeTrackingDbContext(ILoggerFactory loggerFactory)
        => _loggerFactory = loggerFactory;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseLoggerFactory(_loggerFactory)
            .EnableSensitiveDataLogging();

        optionsBuilder.UseSqlite("Data Source=TimeTracking.sqlite");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureProject(modelBuilder.Entity<Project>());
        ConfigureOrder(modelBuilder.Entity<Order>());
        ConfigureTimeSheet(modelBuilder.Entity<TimeSheet>());
    }

    private static void ConfigureProject(EntityTypeBuilder<Project> projectBuilder)
    {
        projectBuilder
            .HasOne(project => project.Customer)
            .WithMany(customer => customer.Projects)
            .HasForeignKey(project => project.CustomerId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }

    private void ConfigureOrder(EntityTypeBuilder<Order> orderBuilder)
    {
        orderBuilder
            .HasOne(project => project.Customer)
            .WithMany(customer => customer.Orders)
            .HasForeignKey(project => project.CustomerId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }

    private void ConfigureTimeSheet(EntityTypeBuilder<TimeSheet> timeSheetBuilder)
    {
        timeSheetBuilder
            .HasOne(x => x.Project)
            .WithMany()
            .HasForeignKey(timeSheet => timeSheet.ProjectId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        timeSheetBuilder
            .HasOne(x => x.Order)
            .WithMany()
            .HasForeignKey(timeSheet => timeSheet.OrderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}