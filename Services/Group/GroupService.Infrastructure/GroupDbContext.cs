using GroupService.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace GroupService.Infrastructure
{
    public class GroupDbContext : DbContext
    {

        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }
        public DbSet<GroupListing> GroupListings { get; set; }
        public DbSet<RoomApplication> RoomApplications { get; set; }

        public GroupDbContext(DbContextOptions<GroupDbContext> options) : base(options) { }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // GROUP
            modelBuilder.Entity<Group>(entity =>
            {
                entity.HasKey(g => g.Id);

                entity.Property(g => g.Name)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(g => g.Description)
                      .HasMaxLength(1000);

                entity.HasMany(g => g.Members)
                      .WithOne(m => m.Group)
                      .HasForeignKey(m => m.GroupId);
            });

            // GROUP MEMBER
            modelBuilder.Entity<GroupMember>(entity =>
            {
                entity.HasKey(m => m.Id);

                entity.HasIndex(m => new { m.UserId, m.GroupId }).IsUnique();

                entity.Property(m => m.Role)
                      .HasConversion<string>()
                      .IsRequired();
            });

            // GROUP LISTING
            modelBuilder.Entity<GroupListing>(entity =>
            {
                entity.HasKey(l => l.Id);

                entity.Property(l => l.Title)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(l => l.Description)
                      .HasMaxLength(1000);

                entity.Property(l => l.Status)
                      .HasConversion<string>()
                      .IsRequired();

                entity.HasOne(l => l.Group)
                      .WithMany()
                      .HasForeignKey(l => l.GroupId);

                entity.HasMany(l => l.Applications)
                      .WithOne(a => a.Listing)
                      .HasForeignKey(a => a.ListingId);
            });

            // ROOM APPLICATION
            modelBuilder.Entity<RoomApplication>(entity =>
            {
                entity.HasKey(a => a.Id);

                entity.Property(a => a.Status)
                      .HasConversion<string>()
                      .IsRequired();

                entity.Property(a => a.Message)
                      .HasMaxLength(1000);
            });
        }
    }
}
