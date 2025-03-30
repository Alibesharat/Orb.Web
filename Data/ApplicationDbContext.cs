using Microsoft.EntityFrameworkCore;
using Orb.Web.Models;
using System;
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

            // Configure Tags as a converted column since EF Core can't store lists directly
            modelBuilder.Entity<BlogPost>()
                .Property(b => b.Tags)
                .HasConversion(
                    v => string.Join(',', v ?? new List<string>()),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                );

            // Configure many-to-many relationship for User Following
            modelBuilder.Entity<User>()
                .HasMany(u => u.Following)
                .WithMany(u => u.Followers)
                .UsingEntity(j => j.ToTable("UserFollowers"));
        }
    }
}