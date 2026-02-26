using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // ========================
    // DbSets
    // ========================

    public DbSet<ApplicationUser> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    public DbSet<PropertyCategory> PropertyCategories { get; set; }
    public DbSet<PropertySubCategory> PropertySubCategories { get; set; }
    public DbSet<PropertyPlan> PropertyPlans { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ========================
        // RELATIONSHIPS
        // ========================

        modelBuilder.Entity<RefreshToken>()
            .HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId);

        modelBuilder.Entity<PropertyCategory>()
            .HasMany(c => c.SubCategories)
            .WithOne(s => s.Category)
            .HasForeignKey(s => s.CategoryId);

        modelBuilder.Entity<PropertySubCategory>()
            .HasMany(s => s.Plans)
            .WithOne(p => p.SubCategory)
            .HasForeignKey(p => p.SubCategoryId);

        modelBuilder.Entity<ApplicationUser>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // ========================
        // SEED DATA
        // ========================

        // ---- ADMIN USER ----
        modelBuilder.Entity<ApplicationUser>().HasData(
            new ApplicationUser
            {
                Id = 1,
                FullName = "Admin",
                Email = "admin@gmail.com",
                PasswordHash = "$2a$11$kkF9EKe7KJxAijZ374He4edBGTSujLGRA48MkMwN9g6PK77IM2H..",
                Role = UserRole.Admin,
                IsActive = true,
                CreatedAt = new DateTime(2024, 1, 1)
            }
        );

        // ---- MASTER CATEGORY ----
        modelBuilder.Entity<PropertyCategory>().HasData(
            new PropertyCategory
            {
                Id = 1,
                Name = "Property Insurance",
                CreatedAt = new DateTime(2024, 1, 1)
            }
        );

        // ---- SUBCATEGORIES ----
        modelBuilder.Entity<PropertySubCategory>().HasData(
            new PropertySubCategory
            {
                Id = 1,
                Code = "SUB_RES_01",
                Name = "Residential Property",
                CategoryId = 1,
                CreatedAt = new DateTime(2024, 1, 1)
            },
            new PropertySubCategory
            {
                Id = 2,
                Code = "SUB_COM_02",
                Name = "Commercial Property",
                CategoryId = 1,
                CreatedAt = new DateTime(2024, 1, 1)
            },
            new PropertySubCategory
            {
                Id = 3,
                Code = "SUB_IND_03",
                Name = "Industrial & Special Use",
                CategoryId = 1,
                CreatedAt = new DateTime(2024, 1, 1)
            },
            new PropertySubCategory
            {
                Id = 4,
                Code = "SUB_CON_04",
                Name = "Property Contents",
                CategoryId = 1,
                CreatedAt = new DateTime(2024, 1, 1)
            }
        );

        // ---- ONE PLAN PER SUBCATEGORY (DEMO) ----
        modelBuilder.Entity<PropertyPlan>().HasData(
            new PropertyPlan
            {
                Id = 1,
                PlanName = "Basic Residential Plan",
                BaseCoverageAmount = 1000000,
                CoverageRate = 0.02m,
                SubCategoryId = 1,
                CreatedAt = new DateTime(2024, 1, 1)
            },
            new PropertyPlan
            {
                Id = 2,
                PlanName = "Basic Commercial Plan",
                BaseCoverageAmount = 5000000,
                CoverageRate = 0.03m,
                SubCategoryId = 2,
                CreatedAt = new DateTime(2024, 1, 1)
            },
            new PropertyPlan
            {
                Id = 3,
                PlanName = "Basic Industrial Plan",
                BaseCoverageAmount = 10000000,
                CoverageRate = 0.04m,
                SubCategoryId = 3,
                CreatedAt = new DateTime(2024, 1, 1)
            },
            new PropertyPlan
            {
                Id = 4,
                PlanName = "Basic Contents Plan",
                BaseCoverageAmount = 300000,
                CoverageRate = 0.015m,
                SubCategoryId = 4,
                CreatedAt = new DateTime(2024, 1, 1)
            }
        );
    }
}