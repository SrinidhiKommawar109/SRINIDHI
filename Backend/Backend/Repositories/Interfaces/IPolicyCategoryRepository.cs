public interface IPolicyCategoryRepository
{
    Task<IEnumerable<PolicyCategory>> GetAllAsync();
    Task<PolicyCategory> GetByIdAsync(int id);
    Task AddAsync(PolicyCategory category);
}