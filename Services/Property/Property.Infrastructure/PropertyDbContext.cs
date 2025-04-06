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

        public PropertyDbContext(DbContextOptions<PropertyDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define the primary key for Property
            modelBuilder.Entity<Property>()
                .HasKey(p => p.Id); // Explicitly set Id as the primary key

            // Configure the properties for Property
            modelBuilder.Entity<Property>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(255); // Ensure Name is required and has max length

            modelBuilder.Entity<Property>()
                .Property(p => p.PricePerNight)
                .HasPrecision(18, 2); // Set precision for decimal fields like PricePerNight

            // Create a relationship between Property and PropertyImage
            modelBuilder.Entity<Property>()
                .HasMany(p => p.Images)
                .WithOne(i => i.Property)
                .HasForeignKey(i => i.PropertyId);

            modelBuilder.Entity<Property>()
                .HasIndex(p => p.OwnerId); // Optional: Index for OwnerId if you plan to search by it often
        }
    }

}
