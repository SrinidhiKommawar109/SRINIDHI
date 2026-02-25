public class PolicySubCategory
{
    public int Id { get; set; }
    public string Name { get; set; }

    public int PolicyCategoryId { get; set; }
    public PolicyCategory PolicyCategory { get; set; }
}