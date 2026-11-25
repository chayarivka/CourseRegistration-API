
using CourseRegistration.Core.Entities;
using CourseRegistration.Core.Enums;
using CourseRegistration.Core.Interfaces;
using CourseRegistration.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CourseRegistration.Data.Repositories
{
    public class RegistrationRepository : IRegistrationRepository
    {
        private readonly AppDbContext _context;

        public RegistrationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Registration?> GetByUserAndCourseAsync(
            int userId,
            int courseId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Registrations
                .FirstOrDefaultAsync(
                    r => r.UserId == userId &&
                         r.CourseId == courseId,
                    cancellationToken);
        }

        public async Task<int> GetActiveRegistrationsCountAsync(
            int courseId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Registrations
                .CountAsync(
                    r => r.CourseId == courseId &&
                         r.Status == RegistrationStatus.Active,
                    cancellationToken);
        }

        public async Task AddAsync(
            Registration registration,
            CancellationToken cancellationToken = default)
        {
            await _context.Registrations.AddAsync(
                registration, cancellationToken);
        }

        public async Task SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<Registration>> GetByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Registrations
                .AsNoTracking()
                .Include(r => r.Course)
                .Where(r => r.UserId == userId &&
            r.Status == RegistrationStatus.Active)
                .OrderByDescending(r => r.RegistrationDate)
                .ToListAsync(cancellationToken);
        }
        public async Task ExecuteInSerializableTransactionAsync(
         Func<CancellationToken, Task> operation,
         CancellationToken cancellationToken = default)
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async attemptToken =>
            {
                _context.ChangeTracker.Clear();
                await using var transaction =
                    await _context.Database.BeginTransactionAsync(
                        IsolationLevel.Serializable,
                        attemptToken);

                await operation(attemptToken);

                await transaction.CommitAsync(attemptToken);
            }, cancellationToken);
        }

    }
}
