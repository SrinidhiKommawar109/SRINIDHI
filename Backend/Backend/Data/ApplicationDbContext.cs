using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<PolicyCategory> PolicyCategories { get; set; }
        public DbSet<PolicySubCategory> PolicySubCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Store enum as string in database 
            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<string>();

            // Unique Username
            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserName)
                .IsUnique();

            // Unique Email
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}