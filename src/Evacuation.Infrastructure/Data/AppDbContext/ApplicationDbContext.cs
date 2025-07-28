using Evacuation.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Evacuation.Infrastructure.Data.AppDbContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        // DbSet properties for your entities
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<EvacuationZone> EvacuationZones { get; set; }
        public DbSet<EvacuationPlan> EvacuationPlans { get; set; }
        public DbSet<EvacuationStatus> EvacuationStatuses { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure entity properties and relationships here

            // Key configurations
            modelBuilder.Entity<Vehicle>()
                .HasKey(v => v.VehicleId);
            modelBuilder.Entity<EvacuationZone>()
                .HasKey(z => z.ZoneId);
            modelBuilder.Entity<EvacuationPlan>()
                .HasKey(p => p.PlanId);
            modelBuilder.Entity<EvacuationStatus>()
                .HasKey(s => s.ZoneId);

            // Configure properties if necessary
            modelBuilder.Entity<Vehicle>()
                .OwnsOne(v => v.LocationCoordinates);
            modelBuilder.Entity<EvacuationZone>()
                .OwnsOne(z => z.LocationCoordinates);

            // Configure relationships if necessary
            modelBuilder.Entity<EvacuationPlan>()
                .HasOne<Vehicle>()
                .WithMany()
                .HasForeignKey(p => p.VehicleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EvacuationPlan>()
                .HasOne<EvacuationZone>()
                .WithMany()
                .HasForeignKey(p => p.ZoneId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EvacuationStatus>()
                .HasOne<EvacuationZone>()
                .WithOne()
                .HasForeignKey<EvacuationStatus>(s => s.ZoneId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes for faster queries
            modelBuilder.Entity<EvacuationPlan>().HasIndex(p => p.VehicleId);
            modelBuilder.Entity<EvacuationPlan>().HasIndex(p => p.ZoneId);


        }


    }
}
