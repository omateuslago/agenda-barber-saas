using BarberSaaS.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BarberSaaS.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<BarberShop> BarberShops => Set<BarberShop>();
        public DbSet<Barber> Barbers => Set<Barber>();
        public DbSet<Service> Services => Set<Service>();
        public DbSet<Appointment> Appointments => Set<Appointment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BarberShop>(entity =>
            {
                entity.ToTable("barber_shops");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(x => x.Phone)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(x => x.CreatedAt)
                    .IsRequired();

                entity.HasMany(x => x.Barbers)
                    .WithOne(x => x.BarberShop)
                    .HasForeignKey(x => x.BarberShopId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(x => x.Services)
                    .WithOne(x => x.BarberShop)
                    .HasForeignKey(x => x.BarberShopId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Barber>(entity =>
            {
                entity.ToTable("barbers");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(120);
            });

            modelBuilder.Entity<Service>(entity =>
            {
                entity.ToTable("services");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(120);

                entity.Property(x => x.Price)
                    .HasColumnType("numeric(10,2)")
                    .IsRequired();

                entity.Property(x => x.DurationMinutes)
                    .IsRequired();
            });

            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.ToTable("appointments");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.CustomerName)
                    .IsRequired()
                    .HasMaxLength(120);

                entity.Property(x => x.CustomerPhone)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(x => x.StartsAt)
                    .IsRequired();

                entity.Property(x => x.Status)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.HasOne(x => x.Barber)
                    .WithMany(x => x.Appointments)
                    .HasForeignKey(x => x.BarberId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Service)
                    .WithMany(x => x.Appointments)
                    .HasForeignKey(x => x.ServiceId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}