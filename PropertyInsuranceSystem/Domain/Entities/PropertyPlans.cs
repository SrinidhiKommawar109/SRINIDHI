using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class PropertyPlans : BaseEntity
{
    
    public string PlanName { get; set; }

    public decimal BaseCoverageAmount { get; set; }

    public decimal CoverageRate { get; set; }

    public decimal BasePremium { get; set; }

    public decimal AgentCommission { get; set; }

    public PremiumFrequency Frequency { get; set; }


    // Foreign Key
    public int SubCategoryId { get; set; }

    // Navigation Property (THIS WAS MISSING)
    public PropertySubCategory SubCategory { get; set; }
}