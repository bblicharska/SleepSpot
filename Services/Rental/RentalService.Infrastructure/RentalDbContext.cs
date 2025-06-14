using Microsoft.EntityFrameworkCore;
using RentalService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace RentalService.Infrastructure
{
    public class RentalDbContext : DbContext
    {
        public DbSet<RentalAgreement> RentalAgreements { get; set; }

        public RentalDbContext(DbContextOptions<RentalDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RentalAgreement>(entity =>
            {
                entity.HasKey(ra => ra.Id);

                entity.Property(ra => ra.MonthlyRent)
                      .HasColumnType("decimal(18,2)");

                entity.Property(ra => ra.Status)
                      .HasConversion<string>();

                entity.Property(ra => ra.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.HasIndex(ra => ra.PropertyId);
                entity.HasIndex(ra => ra.UserId);
                entity.HasIndex(ra => ra.GroupId);
                entity.HasIndex(ra => ra.RoomId);
            });
        }
    }
}
