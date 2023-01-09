using Gospodarstwo.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

namespace Gospodarstwo.Data
{
    public class ApplicationDbContext : IdentityDbContext <AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category>? Categories { get; set; }
        public DbSet<Item>? Items { get; set; }
        public DbSet<Note>? Notes { get; set; }
        public DbSet<AppUser>? AppUsers { get; set; }
        public DbSet<Unit>? Units { get; set; }

        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            base.OnModelCreating(modelbuilder);

            modelbuilder.Entity<Category>()
            .HasMany(c => c.Items)
            .WithOne(t => t.Category);
            modelbuilder.Entity<Item>()
            .HasOne(t => t.Category)
            .WithMany(c => c.Items);
            modelbuilder.Entity<Item>()
            .HasMany(t => t.Notes)
            .WithOne(o => o.Item);
            modelbuilder.Entity<Note>()
            .HasOne(o => o.Item)
            .WithMany(t => t.Notes)
            .OnDelete(DeleteBehavior.Restrict);
            modelbuilder.Entity<AppUser>()
            .HasMany(u => u.Items)
            .WithOne(t => t.User);
            modelbuilder.Entity<Item>()
            .HasOne(u => u.User)
            .WithMany(u => u.Items);
            modelbuilder.Entity<AppUser>()
            .HasMany(u => u.Notes)
            .WithOne(o => o.User);
            modelbuilder.Entity<Note>()
            .HasOne(o => o.User)
            .WithMany(u => u.Notes);
            modelbuilder.Entity<Unit>()
            .HasMany(c => c.Items)
            .WithOne(t => t.Unit);
            modelbuilder.Entity<Item>()
            .HasOne(t => t.Unit)
            .WithMany(c => c.Items);
        }

    }
}