
using CourseRegistration.Core.Entities;
using CourseRegistration.Core.DTOs;


namespace CourseRegistration.Core.Interfaces
{
    public interface ICourseRepository
    {
        Task<Course?> GetByIdAsync(
            int id, CancellationToken cancellationToken = default);

        Task<CourseResponseDto?> GetResponseByIdAsync(
            int id, CancellationToken cancellationToken = default);

        Task<List<CourseResponseDto>> GetAllAsync(
            CancellationToken cancellationToken = default);

        Task AddAsync(
            Course course, CancellationToken cancellationToken = default);

        void Update(Course course);

        void Delete(Course course);

        Task SaveChangesAsync(
            CancellationToken cancellationToken = default);

        Task<bool> CategoryExistsAsync(
            int categoryId,
            CancellationToken cancellationToken = default);

        Task<int> GetActiveRegistrationsCountAsync(
            int courseId,
            CancellationToken cancellationToken = default);

        void SetOriginalRowVersion(
            Course course, byte[] rowVersion);

        Task<List<CourseResponseDto>> SearchAsync(
            CourseQueryDto query,
            CancellationToken cancellationToken = default);



    }
}
