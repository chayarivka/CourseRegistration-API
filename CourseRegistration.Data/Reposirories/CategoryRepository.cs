
using CourseRegistration.Core.Entities;
using CourseRegistration.Core.Interfaces;
using CourseRegistration.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace CourseRegistration.Data.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetAllAsync(
            CancellationToken cancellationToken)
        {
            return await _context.Categories
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ToListAsync(cancellationToken);
        }
    }
}