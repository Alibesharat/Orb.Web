using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Orb.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Orb.Web.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Create a value converter for the Tags property
            var tagsConverter = new ValueConverter<List<string>, string>(
                v => string.Join(',', v ?? new List<string>()),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
            );

            // Create a value comparer for the Tags property
            var tagsComparer = new ValueComparer<List<string>>(
                (c1, c2) => c1.SequenceEqual(c2),                  // Compare lists for equality
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())), // Get hash code
                c => c.ToList()                                   // Create snapshot for change tracking
            );

            // Apply both the converter and comparer to the Tags property
            modelBuilder.Entity<BlogPost>()
                .Property(b => b.Tags)
                .HasConversion(tagsConverter)
                .Metadata.SetValueComparer(tagsComparer);

            // Configure many-to-many relationship for User Following
            modelBuilder.Entity<User>()
                .HasMany(u => u.Following)
                .WithMany(u => u.Followers)
                .UsingEntity(j => j.ToTable("UserFollowers"));
        }
    }
}