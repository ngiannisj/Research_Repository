using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Research_Repository_Models;

namespace Research_Repository.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        //Create database tables
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
            //Configure key properties in ItemTags table in database
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ItemTag>()
                .HasKey(i => new { i.ItemId, i.TagId });

            //Configure key properties in ThemeTags table in database
            modelBuilder.Entity<ThemeTag>()
                .HasKey(i => new { i.ThemeId, i.TagId });

            //Enable cascade delete of projects in the database under a team which has been deleted
            modelBuilder.Entity<Project>()
                .HasOne(b => b.Team)
                .WithMany(a => a.Projects)
                .OnDelete(DeleteBehavior.Cascade);

            //Disable cascade delete of items in the database under a theme which has been deleted, the 'themeId' foreign key value is simply set to 'null'
            modelBuilder.Entity<Item>()
                .HasOne(b => b.Theme)
                .WithMany(a => a.Items)
                .OnDelete(DeleteBehavior.SetNull);

            //Disable cascade delete of items in the database under a team which has been deleted, the 'teamId' foreign key value is simply set to 'null'
            modelBuilder.Entity<Item>()
                .HasOne(b => b.Team)
                .WithMany(a => a.Items)
                .OnDelete(DeleteBehavior.SetNull);

            //Disable cascade delete of items in the database under a project which has been deleted, the 'projectId' foreign key value is simply set to 'null'
            modelBuilder.Entity<Item>()
                .HasOne(b => b.Project)
                .WithMany(a => a.Items)
                .OnDelete(DeleteBehavior.SetNull);

            //Disable cascade delete of users in the database under a team which has been deleted, the 'teamId' foreign key value is simply set to 'null'
            modelBuilder.Entity<ApplicationUser>()
    .HasOne(b => b.Team)
    .WithMany(a => a.Users)
    .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
