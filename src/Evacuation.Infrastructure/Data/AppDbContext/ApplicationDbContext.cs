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
        public DbSet<User> Users { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Zone> Zones { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<Status> Statuses { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure entity properties and relationships here

            // Key configurations
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);
            modelBuilder.Entity<Vehicle>()
                .HasKey(v => v.Id);
            modelBuilder.Entity<Zone>()
                .HasKey(z => z.Id);
            modelBuilder.Entity<Plan>()
                .HasKey(p => p.Id);
            modelBuilder.Entity<Status>()
                .HasKey(s => s.Id);

            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<Vehicle>()
                .Property(v => v.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<Zone>()
                .Property(z => z.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<Plan>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<Status>()
                .Property(s => s.Id)
                .ValueGeneratedOnAdd();

            // Configure properties if necessary
            modelBuilder.Entity<Vehicle>()
                .OwnsOne(v => v.LocationCoordinates, lc => 
                {
                    lc.Property(l => l.Latitude).HasColumnName("Latitude");
                    lc.Property(l => l.Longitude).HasColumnName("Longitude");
                });
            modelBuilder.Entity<Zone>()
                .OwnsOne(z => z.LocationCoordinates, lc =>
                {
                    lc.Property(l => l.Latitude).HasColumnName("Latitude");
                    lc.Property(l => l.Longitude).HasColumnName("Longitude");
                });

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
