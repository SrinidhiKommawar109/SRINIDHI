using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Backend.Repositories.Interfaces;

namespace Backend.Repositories.Implementations
{
    public class PolicyCategoryRepository : IPolicyCategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public PolicyCategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PolicyCategory>> GetAllAsync()
        {
            return await _context.PolicyCategories
                .Include(c => c.SubCategories)
                .ToListAsync();
        }

        // ✅ FIX ADDED HERE
        public async Task<PolicyCategory> GetByIdAsync(int id)
        {
            return await _context.PolicyCategories
                .Include(c => c.SubCategories)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddAsync(PolicyCategory category)
        {
            await _context.PolicyCategories.AddAsync(category);
            await _context.SaveChangesAsync();
        }
    }
}