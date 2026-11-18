
using CourseRegistration.Core.Entities;
using CourseRegistration.Core.Interfaces;

namespace CourseRegistration.Service.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<List<Category>> GetAllAsync(
            CancellationToken cancellationToken)
        {
            return await _categoryRepository.GetAllAsync(
                cancellationToken);
        }
    }
}
