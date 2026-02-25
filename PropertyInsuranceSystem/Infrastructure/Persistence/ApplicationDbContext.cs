using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<ApplicationUser> Users { get; set; }

    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var adminId = Guid.NewGuid();
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationUser>().HasData(
     new ApplicationUser
     {
         Id = adminId,
         FullName = "Admin",
         Email = "admin@gmail.com",
         PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
         Role = UserRole.Admin,
         IsActive = true,
         CreatedAt = DateTime.UtcNow
     }
 );

        modelBuilder.Entity<RefreshToken>()
            .HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId);
    }
}
