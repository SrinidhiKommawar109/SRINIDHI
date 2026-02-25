using Backend.Services.Interfaces;
using Backend.Models;
using Backend.Repositories.Interfaces;
using Backend.Services.Interfaces;

namespace PropertyInsuranceSystem.Services.Implementations
{
    public class PolicyService : IPolicyService
    {
        private readonly IPolicyCategoryRepository _policyCategoryRepository;

        public PolicyService(IPolicyCategoryRepository policyCategoryRepository)
        {
            _policyCategoryRepository = policyCategoryRepository;
        }

        // ================================
        // CATEGORY METHODS
        // ================================

        public async Task CreateCategoryAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Category name cannot be empty");

            var category = new PolicyCategory
            {
                Name = name
            };

            await _policyCategoryRepository.AddAsync(category);
        }

        public async Task<IEnumerable<PolicyCategory>> GetAllCategoriesAsync()
        {
            return await _policyCategoryRepository.GetAllAsync();
        }

        
    }
}