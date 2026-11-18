
using CourseRegistration.Core.Entities;

namespace CourseRegistration.Core.Interfaces
{
    public interface IRegistrationRepository
    {
        Task<Registration?> GetByUserAndCourseAsync(
            int userId,
            int courseId,
            CancellationToken cancellationToken = default);

        Task<List<Registration>> GetByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default);


        Task<int> GetActiveRegistrationsCountAsync(
            int courseId,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            Registration registration,
            CancellationToken cancellationToken = default);

        Task SaveChangesAsync(
            CancellationToken cancellationToken = default);
        Task ExecuteInSerializableTransactionAsync(
    Func<CancellationToken, Task> operation,
    CancellationToken cancellationToken = default);
    }
}
