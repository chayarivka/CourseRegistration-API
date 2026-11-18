
using CourseRegistration.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CourseRegistration.Tests
{
    public class CourseConcurrencyTests
    {
        [Fact]
        public async Task TwoContexts_SecondUpdateThrowsConcurrencyException()
        {
            // Arrange
            await using var setupContext = TestDbContextFactory.Create();

            var categoryId = await setupContext.Categories
                .Select(c => c.Id)
                .FirstAsync();

            var course = new Course
            {
                Name = "Concurrency Test Course",
                Description = "Test",
                Price = 100,
                StartDate = DateTime.Now.AddMonths(2),
                StartTime = new TimeSpan(10, 0, 0),
                LecturerName = "Test Lecturer",
                NumberOfMeetings = 5,
                Capacity = 10,
                CategoryId = categoryId
            };

            setupContext.Courses.Add(course);
            await setupContext.SaveChangesAsync();

            int courseId = course.Id;

            await using var context1 = TestDbContextFactory.Create();
            await using var context2 = TestDbContextFactory.Create();

            var course1 = await context1.Courses.FindAsync(courseId);
            var course2 = await context2.Courses.FindAsync(courseId);

            Assert.NotNull(course1);
            Assert.NotNull(course2);

            // Act - first context
            course1.Name = "Updated by Context 1";
            await context1.SaveChangesAsync();

            // Act - second context
            course2.Name = "Updated by Context 2";

            // Assert
            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(
                async () => await context2.SaveChangesAsync());
        }

        [Fact]
        public async Task TwoContexts_LastSeat_SecondUpdateThrowsConcurrencyException()
        {
            // Arrange
            await using var setupContext = TestDbContextFactory.Create();

            var categoryId = await setupContext.Categories
                .Select(c => c.Id)
                .FirstAsync();

            var course = new Course
            {
                Name = "Last Seat Test Course",
                Description = "Test",
                Price = 100,
                StartDate = DateTime.Now.AddMonths(2),
                StartTime = new TimeSpan(10, 0, 0),
                LecturerName = "Test Lecturer",
                NumberOfMeetings = 5,
                Capacity = 1,
                OccupiedSeats = 0,
                CategoryId = categoryId
            };

            setupContext.Courses.Add(course);
            await setupContext.SaveChangesAsync();

            int courseId = course.Id;

            await using var context1 = TestDbContextFactory.Create();
            await using var context2 = TestDbContextFactory.Create();

            var course1 = await context1.Courses.FindAsync(courseId);
            var course2 = await context2.Courses.FindAsync(courseId);

            Assert.NotNull(course1);
            Assert.NotNull(course2);

            // Both contexts see one available seat
            Assert.Equal(0, course1.OccupiedSeats);
            Assert.Equal(0, course2.OccupiedSeats);

            // First context takes the last seat
            course1.OccupiedSeats++;
            await context1.SaveChangesAsync();

            // Second context tries to take the same seat
            course2.OccupiedSeats++;

            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(
                async () => await context2.SaveChangesAsync());

            // Verify that only one seat was occupied
            await using var verifyContext = TestDbContextFactory.Create();

            var savedCourse = await verifyContext.Courses.FindAsync(courseId);

            Assert.NotNull(savedCourse);
            Assert.Equal(1, savedCourse.OccupiedSeats);
        }

    }
}
