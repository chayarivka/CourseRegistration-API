
using CourseRegistration.Core.Entities;

namespace CourseRegistration.Core.Interfaces
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllAsync(
            CancellationToken cancellationToken);
    }
}
