using Microsoft.EntityFrameworkCore;
using PropertyService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace PropertyService.Infrastructure
{
    public class PropertyDbContext : DbContext
    {
        public DbSet<Property> Properties { get; set; }
        public DbSet<PropertyImage> PropertyImages { get; set; }
        public DbSet<Room> Rooms { get; set; }

        public PropertyDbContext(DbContextOptions<PropertyDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ---------- Property ----------
            modelBuilder.Entity<Property>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<Property>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(255);

            modelBuilder.Entity<Property>()
                .Property(p => p.PricePerMonth)
                .HasPrecision(18, 2); // ⬅️ aktualizacja: już nie PricePerNight

            modelBuilder.Entity<Property>()
                .Property(p => p.AreaInSquareMeters)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Property>()
                .HasMany(p => p.Images)
                .WithOne(i => i.Property)
                .HasForeignKey(i => i.PropertyId);

            modelBuilder.Entity<Property>()
                .HasMany(p => p.Rooms)
                .WithOne(r => r.Property)
                .HasForeignKey(r => r.PropertyId);

            modelBuilder.Entity<Property>()
                .HasIndex(p => p.OwnerId);

            // ---------- Room ----------
            modelBuilder.Entity<Room>()
                .HasKey(r => r.Id);

            modelBuilder.Entity<Room>()
                .Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(255);

            modelBuilder.Entity<Room>()
                .Property(r => r.PricePerMonth)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Room>()
                .Property(r => r.AreaInSquareMeters)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Room>()
                .Property(r => r.IsAvailable)
                .HasDefaultValue(true);

            // Opcjonalnie: indeksy
            modelBuilder.Entity<Room>()
                .HasIndex(r => r.PropertyId);
        }
    }

}
