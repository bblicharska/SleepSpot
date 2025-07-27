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
            base.OnModelCreating(modelBuilder);

            // Property → Rooms: Cascade
            modelBuilder.Entity<Room>()
                .HasOne(r => r.Property)
                .WithMany(p => p.Rooms)
                .HasForeignKey(r => r.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            // PropertyImage → Room: OPTIONAL, Cascade delete
            modelBuilder.Entity<PropertyImage>()
                .HasOne(pi => pi.Room)
                .WithMany(r => r.Images)
                .HasForeignKey(pi => pi.RoomId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            // PropertyImage → Property: OPTIONAL, Restrict delete
            modelBuilder.Entity<PropertyImage>()
                .HasOne(pi => pi.Property)
                .WithMany(p => p.Images)
                .HasForeignKey(pi => pi.PropertyId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            // 🆕 Property configuration
            modelBuilder.Entity<Property>(entity =>
            {
                entity.Property(p => p.isAvailable)
                      .HasDefaultValue(true); // domyślnie true
            });
        }

    }
}
