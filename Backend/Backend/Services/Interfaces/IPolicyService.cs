using Backend.Models;
namespace Backend.Services.Interfaces
{
    public interface IPolicyService
    {
        Task CreateCategoryAsync(string name);
        Task<IEnumerable<PolicyCategory>> GetAllCategoriesAsync();
    }
}