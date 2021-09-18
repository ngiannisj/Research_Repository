using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Research_Repository_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Research_Repository.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Theme> Themes { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ItemTag> ItemTags { get; set; }
        public DbSet<ThemeTag> ThemeTags { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ItemTag>()
                .HasKey(i => new { i.ItemId, i.TagId });

            modelBuilder.Entity<ThemeTag>()
                .HasKey(i => new { i.ThemeId, i.TagId });

            modelBuilder.Entity<Project>()
                .HasOne(b => b.Team)
                .WithMany(a => a.Projects)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Item>()
                .HasOne(b => b.Theme)
                .WithMany(a => a.Items)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Item>()
                .HasOne(b => b.Project)
                .WithMany(a => a.Items)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<ApplicationUser>()
    .HasOne(b => b.Team)
    .WithMany(a => a.Users)
    .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
