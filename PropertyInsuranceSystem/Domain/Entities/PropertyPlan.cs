using Domain.Common;

namespace Domain.Entities;

public class PropertyPlan : BaseEntity
{
    public string PlanName { get; set; }

    public decimal BaseCoverageAmount { get; set; }

    public decimal CoverageRate { get; set; }

    // Foreign Key
    public int SubCategoryId { get; set; }

    // Navigation Property (THIS WAS MISSING)
    public PropertySubCategory SubCategory { get; set; }
}