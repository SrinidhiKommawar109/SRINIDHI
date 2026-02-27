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
    public DbSet<PropertyPlans> PropertyPlans { get; set; }

    public DbSet<PolicyRequest> PolicyRequests { get; set; }

    public DbSet<Claim> Claims { get; set; }

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

        // ---- USERS ----
        modelBuilder.Entity<ApplicationUser>().HasData(
            new ApplicationUser
            {
                Id = 1,
                FullName = "Admin",
                Email = "admin@gmail.com",
                PasswordHash = "$2a$11$kkF9EKe7KJxAijZ374He4edBGTSujLGRA48MkMwN9g6PK77IM2H..", // Admin@123
                Role = UserRole.Admin,
                IsActive = true
            },
            new ApplicationUser
            {
                Id = 2,
                FullName = "Claims Officer",
                Email = "claims@gmail.com",
                PasswordHash = "$2a$11$kkF9EKe7KJxAijZ374He4edBGTSujLGRA48MkMwN9g6PK77IM2H..", // Claims@123
                Role = UserRole.ClaimsOfficer,
                IsActive = true
            },
            new ApplicationUser
            {
                Id = 3,
                FullName = "Customer",
                Email = "customer@gmail.com",
                PasswordHash = "$2a$11$kkF9EKe7KJxAijZ374He4edBGTSujLGRA48MkMwN9g6PK77IM2H..", // Customer@123
                Role = UserRole.Customer,
                IsActive = true
            }
        );

        // ---- PROPERTY CATEGORY ----
        modelBuilder.Entity<PropertyCategory>().HasData(
            new PropertyCategory { Id = 1, Name = "Property Insurance" }
        );

        // ---- SUBCATEGORIES ----
        modelBuilder.Entity<PropertySubCategory>().HasData(
            new PropertySubCategory { Id = 1, Code = "SUB_RES_01", Name = "Residential Property", CategoryId = 1 },
            new PropertySubCategory { Id = 2, Code = "SUB_COM_02", Name = "Commercial Property", CategoryId = 1 },
            new PropertySubCategory { Id = 3, Code = "SUB_IND_03", Name = "Industrial & Special Use", CategoryId = 1 },
            new PropertySubCategory { Id = 4, Code = "SUB_CON_04", Name = "Property Contents", CategoryId = 1 }
        );

        // ---- PLANS ----
        modelBuilder.Entity<PropertyPlans>().HasData(
            new PropertyPlans { Id = 1, PlanName = "Basic Residential Plan", BaseCoverageAmount = 1000000, CoverageRate = 0.02m, BasePremium = 5000, AgentCommission = 500, Frequency = PremiumFrequency.Yearly, SubCategoryId = 1 },
            new PropertyPlans { Id = 2, PlanName = "Basic Commercial Plan", BaseCoverageAmount = 5000000, CoverageRate = 0.03m, BasePremium = 15000, AgentCommission = 1200, Frequency = PremiumFrequency.Quarterly, SubCategoryId = 2 },
            new PropertyPlans { Id = 3, PlanName = "Basic Industrial Plan", BaseCoverageAmount = 10000000, CoverageRate = 0.04m, BasePremium = 25000, AgentCommission = 2000, Frequency = PremiumFrequency.HalfYearly, SubCategoryId = 3 },
            new PropertyPlans { Id = 4, PlanName = "Basic Contents Plan", BaseCoverageAmount = 300000, CoverageRate = 0.015m, BasePremium = 2000, AgentCommission = 200, Frequency = PremiumFrequency.Yearly, SubCategoryId = 4 }
        );

        // ---- POLICY REQUESTS (Parent for Claims) ----
        modelBuilder.Entity<PolicyRequest>().HasData(
            new PolicyRequest
            {
                Id = 1,
                PlanId = 1,
                CustomerId = 3, // Customer
                Status = PolicyRequestStatus.PolicyApproved,
                AgentId = null
            },
            new PolicyRequest
            {
                Id = 2,
                PlanId = 2,
                CustomerId = 3,
                Status = PolicyRequestStatus.PolicyApproved,
                AgentId = null
            }
        );

        // ---- CLAIMS (Children, after PolicyRequests) ----
        modelBuilder.Entity<Claim>().HasData(
            new Claim
            {
                Id = 1,
                PolicyRequestId = 1,
                PropertyAddress = "123 Main Street",
                PropertyValue = 1000000,
                PropertyAge = 10,
                ClaimAmount = 5000, // could match BasePremium
                Status = ClaimStatus.Pending,
                Remarks = ""
            },
            new Claim
            {
                Id = 2,
                PolicyRequestId = 2,
                PropertyAddress = "456 Commerce Road",
                PropertyValue = 5000000,
                PropertyAge = 5,
                ClaimAmount = 15000,
                Status = ClaimStatus.Pending,
                Remarks = ""
            }
        );
    }
}