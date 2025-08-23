using Microsoft.EntityFrameworkCore;
using ReviewService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ReviewService.Infrastructure
{
    public class ReviewDbContext : DbContext
    {
        public DbSet<Review> Reviews { get; set; }

        public ReviewDbContext(DbContextOptions<ReviewDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(r => r.Id);

                entity.Property(r => r.ReviewerId)
                    .IsRequired();

                entity.Property(r => r.PropertyId)
                    .IsRequired(false);

                entity.Property(r => r.OwnerId)
                    .IsRequired(false);

                entity.Property(r => r.RoomId)
                    .IsRequired(false);

                entity.Property(r => r.Rating)
                    .IsRequired();

                entity.Property(r => r.Comment)
                    .HasMaxLength(1000);

                entity.Property(r => r.CreatedAt)
                    .IsRequired();

                entity.HasIndex(r => r.PropertyId);
                entity.HasIndex(r => r.RoomId);
                entity.HasIndex(r => r.ReviewerId);

            });
        }
    }

}
