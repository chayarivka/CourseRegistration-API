
using CourseRegistration.Core.Entities;

namespace CourseRegistration.Core.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllAsync(
            CancellationToken cancellationToken);
    }
}
