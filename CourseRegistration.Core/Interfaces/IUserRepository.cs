
using CourseRegistration.Core.Entities;

namespace CourseRegistration.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(
            string email,
            CancellationToken cancellationToken = default);

        Task<User?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<bool> EmailExistsAsync(
            string email,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            User user,
            CancellationToken cancellationToken = default);

        Task SaveChangesAsync(
            CancellationToken cancellationToken = default);
    }
}
