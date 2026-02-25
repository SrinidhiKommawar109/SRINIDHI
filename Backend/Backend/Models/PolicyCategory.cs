public class PolicyCategory
{
    public int Id { get; set; }
    public string Name { get; set; }

    public ICollection<PolicySubCategory> SubCategories { get; set; }
}