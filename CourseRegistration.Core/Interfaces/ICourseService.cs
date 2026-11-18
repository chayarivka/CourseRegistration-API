
using CourseRegistration.Core.DTOs;

namespace CourseRegistration.Core.Interfaces
{
    public interface ICourseService
    {
        Task<CourseResponseDto?> GetByIdAsync(
            int id, CancellationToken cancellationToken = default);

        Task<List<CourseResponseDto>> GetAllAsync(
            CancellationToken cancellationToken = default);

        Task<CourseResponseDto> CreateAsync(
            CreateCourseDto courseDto,
            CancellationToken cancellationToken = default);

        Task<bool> UpdateAsync(
            int id, UpdateCourseDto courseDto,
            CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(
            int id, CancellationToken cancellationToken = default);

        Task<List<CourseResponseDto>> SearchAsync(
            CourseQueryDto query,
            CancellationToken cancellationToken = default);

    }
}
