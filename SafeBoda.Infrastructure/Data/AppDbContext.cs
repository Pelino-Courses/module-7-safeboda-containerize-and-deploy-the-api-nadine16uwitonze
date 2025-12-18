using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SafeBoda.Core.Entities;
using SafeBoda.Core.Identity;

namespace SafeBoda.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Trip> Trips => Set<Trip>();
        public DbSet<Driver> Drivers => Set<Driver>();
        public DbSet<Rider> Riders => Set<Rider>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Trip>(entity =>
            {
                entity.HasKey(t => t.Id);
                // Map Amount CLR property to existing database column named "Price"
                entity.Property(t => t.Amount)
                      .HasColumnName("Price")
                      .HasColumnType("decimal(18,2)");

                entity.Property(t => t.Date).HasColumnType("datetime2");
            });

            modelBuilder.Entity<Driver>(entity =>
            {
                entity.HasKey(d => d.Id);
                entity.Property(d => d.Name).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<Rider>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Name).IsRequired().HasMaxLength(100);
            });
        }
    }
}