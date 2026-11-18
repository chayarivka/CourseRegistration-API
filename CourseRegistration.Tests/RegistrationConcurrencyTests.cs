
using CourseRegistration.Core.Entities;
using CourseRegistration.Core.Enums;
using CourseRegistration.Data.Repositories;
using CourseRegistration.Service.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CourseRegistration.Tests
{
    public class RegistrationConcurrencyTests
    {
        [Fact]
        public async Task TwoUsers_LastSeat_OnlyOneRegistrationSucceeds()
        {
            // Arrange
            int courseId;
            int user1Id;
            int user2Id;

            await using (var setup = TestDbContextFactory.Create())
            {
                int categoryId = await setup.Categories
                    .Select(c => c.Id)
                    .FirstAsync();

                var course = new Course
                {
                    Name = "Parallel Registration Test",
                    Description = "Concurrency test",
                    Price = 100,
                    StartDate = DateTime.Now.AddMonths(2),
                    StartTime = new TimeSpan(10, 0, 0),
                    LecturerName = "Test Lecturer",
                    NumberOfMeetings = 5,
                    Capacity = 1,
                    OccupiedSeats = 0,
                    CategoryId = categoryId
                };

                var uniqueId = Guid.NewGuid().ToString("N");

                var user1 = new User
                {
                    FullName = "Test User 1",
                    Email = $"test1_{uniqueId}@example.com",
                    PasswordHash = "test-hash"
                };

                var user2 = new User
                {
                    FullName = "Test User 2",
                    Email = $"test2_{uniqueId}@example.com",
                    PasswordHash = "test-hash"
                };

                setup.Courses.Add(course);
                setup.Users.AddRange(user1, user2);

                await setup.SaveChangesAsync();

                courseId = course.Id;
                user1Id = user1.Id;
                user2Id = user2.Id;
            }

            // Two independent database contexts
            await using var context1 = TestDbContextFactory.Create();
            await using var context2 = TestDbContextFactory.Create();

            var service1 = new RegistrationService(
                new RegistrationRepository(context1),
                new CourseRepository(context1));

            var service2 = new RegistrationService(
                new RegistrationRepository(context2),
                new CourseRepository(context2));

            // Act: attempt both registrations concurrently
            async Task<Exception?> TryRegister(
                RegistrationService service, int userId)
            {
                try
                {
                    await service.RegisterToCourseAsync(userId, courseId);
                    return null;
                }
                catch (Exception ex)
                {
                    return ex;
                }
            }

            var results = await Task.WhenAll(
                TryRegister(service1, user1Id),
                TryRegister(service2, user2Id));

            // Assert: exactly one registration succeeds
            Assert.Single(results.Where(r => r == null));

            await using var verify = TestDbContextFactory.Create();

            int activeRegistrations = await verify.Registrations
                .CountAsync(r =>
                    r.CourseId == courseId &&
                    r.Status == RegistrationStatus.Active);

            var savedCourse = await verify.Courses
                .FirstAsync(c => c.Id == courseId);

            Assert.Equal(1, activeRegistrations);
            Assert.Equal(1, savedCourse.OccupiedSeats);
        }
    }
}
