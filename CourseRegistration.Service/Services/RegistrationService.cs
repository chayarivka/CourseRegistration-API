
using CourseRegistration.Core.DTOs;
using CourseRegistration.Core.Entities;
using CourseRegistration.Core.Enums;
using CourseRegistration.Core.Interfaces;


namespace CourseRegistration.Service.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IRegistrationRepository _registrationRepository;
        private readonly ICourseRepository _courseRepository;

        public RegistrationService(
            IRegistrationRepository registrationRepository,
            ICourseRepository courseRepository)
        {
            _registrationRepository = registrationRepository;
            _courseRepository = courseRepository;
        }

        public async Task RegisterToCourseAsync(
            int userId,
            int courseId,
            CancellationToken cancellationToken = default)
        {
            await _registrationRepository.ExecuteInSerializableTransactionAsync(
                async ct =>
                {
                    var course = await _courseRepository.GetByIdAsync(
                        courseId, ct);

                    if (course == null)
                        throw new InvalidOperationException(
                            "Course not found.");

                    if (course.StartDate.Date + course.StartTime <= DateTime.Now)
                        throw new InvalidOperationException(
                            "Registration is closed for this course.");

                    var existingRegistration =
                        await _registrationRepository.GetByUserAndCourseAsync(
                            userId, courseId, ct);

                    if (existingRegistration != null &&
                        existingRegistration.Status == RegistrationStatus.Active)
                        throw new InvalidOperationException(
                            "User is already registered to this course.");

                    int activeCount =
                        await _registrationRepository.GetActiveRegistrationsCountAsync(
                            courseId, ct);

                    if (activeCount >= course.Capacity)
                        throw new InvalidOperationException(
                            "Course is full.");
                   course.OccupiedSeats = activeCount + 1;

                    if (existingRegistration != null)
                    {
                        existingRegistration.Status = RegistrationStatus.Active;
                        existingRegistration.RegistrationDate = DateTime.Now;
                    }
                    else
                    {
                        var registration = new Registration
                        {
                            UserId = userId,
                            CourseId = courseId,
                            RegistrationDate = DateTime.Now,
                            Status = RegistrationStatus.Active
                        };

                        await _registrationRepository.AddAsync(
                            registration, ct);
                    }

                    await _registrationRepository.SaveChangesAsync(ct);
                },
                cancellationToken);
        }

        public async Task CancelRegistrationAsync(
            int userId,
            int courseId,
            CancellationToken cancellationToken = default)
        {
            await _registrationRepository.ExecuteInSerializableTransactionAsync(
                async ct =>
                {
                    var registration =
                        await _registrationRepository.GetByUserAndCourseAsync(
                            userId, courseId, ct);

                    if (registration == null ||
                        registration.Status != RegistrationStatus.Active)
                        throw new InvalidOperationException(
                            "Active registration not found.");

                    var course = await _courseRepository.GetByIdAsync(
                        courseId, ct);

                    if (course == null)
                        throw new InvalidOperationException(
                            "Course not found.");

                    if (course.StartDate.Date + course.StartTime <= DateTime.Now)
                        throw new InvalidOperationException(
                            "Cannot cancel registration after the course has started.");

                    registration.Status = RegistrationStatus.Cancelled;

                    course.OccupiedSeats--;

                    await _registrationRepository.SaveChangesAsync(ct);
                },
                cancellationToken);
        }
        public async Task<List<MyCourseDto>> GetMyCoursesAsync(
            int userId,
            CancellationToken cancellationToken = default)
        {
            var registrations =
                await _registrationRepository.GetByUserIdAsync(
                    userId, cancellationToken);

            return registrations.Select(r => new MyCourseDto
            {
                RegistrationId = r.Id,
                CourseId = r.CourseId,
                CourseName = r.Course.Name,
                LecturerName = r.Course.LecturerName,
                StartDate = r.Course.StartDate,
                StartTime = r.Course.StartTime,
                RegistrationDate = r.RegistrationDate,
                Status = r.Status.ToString()
            }).ToList();
        }

    }
}
