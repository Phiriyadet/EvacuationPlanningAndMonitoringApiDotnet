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
        public DbSet<Zone> Zones { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<Status> Statuses { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure entity properties and relationships here

            // Key configurations
            modelBuilder.Entity<Vehicle>()
                .HasKey(v => v.VehicleId);
            modelBuilder.Entity<Zone>()
                .HasKey(z => z.ZoneId);
            modelBuilder.Entity<Plan>()
                .HasKey(p => p.PlanId);
            modelBuilder.Entity<Status>()
                .HasKey(s => s.ZoneId);

            modelBuilder.Entity<Plan>()
                .Property(p => p.PlanId)
                .ValueGeneratedOnAdd();

            // Configure properties if necessary
            modelBuilder.Entity<Vehicle>()
                .OwnsOne(v => v.LocationCoordinates);
            modelBuilder.Entity<Zone>()
                .OwnsOne(z => z.LocationCoordinates);

            // Configure relationships if necessary
            modelBuilder.Entity<Plan>()
                .HasOne<Vehicle>()
                .WithOne()
                .HasForeignKey<Plan>(p => p.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Plan>()
                .HasOne<Zone>()
                .WithMany()
                .HasForeignKey(p => p.ZoneId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Status>()
                .HasOne<Zone>()
                .WithOne()
                .HasForeignKey<Status>(s => s.ZoneId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes for faster queries
            modelBuilder.Entity<Plan>().HasIndex(p => p.VehicleId).IsUnique();
            modelBuilder.Entity<Plan>().HasIndex(p => p.ZoneId);


        }


    }
}
