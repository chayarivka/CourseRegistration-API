
using CourseRegistration.Core.DTOs;

namespace CourseRegistration.Core.Interfaces
{
    public interface IRegistrationService
    {
        Task RegisterToCourseAsync(
            int userId,
            int courseId,
            CancellationToken cancellationToken = default);

        Task CancelRegistrationAsync(
            int userId,
            int courseId,
            CancellationToken cancellationToken = default);
        Task<List<MyCourseDto>> GetMyCoursesAsync(
        int userId,
        CancellationToken cancellationToken = default);
    }

    

}
