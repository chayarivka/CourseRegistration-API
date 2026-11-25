
using CourseRegistration.Core.Entities;
using CourseRegistration.Core.Enums;
using CourseRegistration.Core.Exceptions;
using CourseRegistration.Core.Interfaces;
using CourseRegistration.Service.Services;
using Moq;
using Xunit;

namespace CourseRegistration.Tests
{
    public class RegistrationServiceTests
    {
        [Fact]
        public async Task RegisterToCourse_WhenCourseDoesNotExist_ThrowsException()
        {
            // Arrange
            var registrationRepo = new Mock<IRegistrationRepository>();
            var courseRepo = new Mock<ICourseRepository>();

            registrationRepo
                .Setup(r => r.ExecuteInSerializableTransactionAsync(
                    It.IsAny<Func<CancellationToken, Task>>(),
                    It.IsAny<CancellationToken>()))
                .Returns((Func<CancellationToken, Task> operation,
                          CancellationToken ct) => operation(ct));

            var service = new RegistrationService(
                registrationRepo.Object,
                courseRepo.Object);

            // Act
            var exception = await Assert.ThrowsAsync<ResourceNotFoundException>(
                () => service.RegisterToCourseAsync(1, 99999));

            // Assert
            Assert.Equal("Course not found.", exception.Message);
        }

        [Fact]
        public async Task RegisterToCourse_WhenAlreadyRegistered_ThrowsException()
        {
            // Arrange
            var registrationRepo = new Mock<IRegistrationRepository>();
            var courseRepo = new Mock<ICourseRepository>();

            var course = new Course
            {
                Id = 2,
                Name = "Test Course",
                Description = "Test Description",
                LecturerName = "Test Lecturer",
                StartDate = DateTime.Now.AddDays(30),
                StartTime = new TimeSpan(10, 0, 0),
                Capacity = 20,
                CategoryId = 1
            };

            courseRepo
                .Setup(r => r.GetByIdAsync(
                    2, It.IsAny<CancellationToken>()))
                .ReturnsAsync(course);

            registrationRepo
                .Setup(r => r.GetByUserAndCourseAsync(
                    1, 2, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Registration
                {
                    UserId = 1,
                    CourseId = 2,
                    Status = RegistrationStatus.Active
                });

            registrationRepo
                .Setup(r => r.ExecuteInSerializableTransactionAsync(
                    It.IsAny<Func<CancellationToken, Task>>(),
                    It.IsAny<CancellationToken>()))
                .Returns((Func<CancellationToken, Task> operation,
                          CancellationToken ct) => operation(ct));

            var service = new RegistrationService(
                registrationRepo.Object,
                courseRepo.Object);

            // Act
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.RegisterToCourseAsync(1, 2));

            // Assert
            Assert.Equal(
                "User is already registered to this course.",
                exception.Message);
        }

        [Fact]
        public async Task RegisterToCourse_WhenCourseIsFull_ThrowsException()
        {
            // Arrange
            var registrationRepo = new Mock<IRegistrationRepository>();
            var courseRepo = new Mock<ICourseRepository>();

            var course = new Course
            {
                Id = 2,
                Name = "Full Course",
                Description = "Test Description",
                LecturerName = "Test Lecturer",
                StartDate = DateTime.Now.AddDays(30),
                StartTime = new TimeSpan(10, 0, 0),
                Capacity = 2,
                CategoryId = 1
            };

            courseRepo
                .Setup(r => r.GetByIdAsync(
                    2, It.IsAny<CancellationToken>()))
                .ReturnsAsync(course);

            registrationRepo
                .Setup(r => r.GetActiveRegistrationsCountAsync(
                    2, It.IsAny<CancellationToken>()))
                .ReturnsAsync(2);

            registrationRepo
                .Setup(r => r.ExecuteInSerializableTransactionAsync(
                    It.IsAny<Func<CancellationToken, Task>>(),
                    It.IsAny<CancellationToken>()))
                .Returns((Func<CancellationToken, Task> operation,
                          CancellationToken ct) => operation(ct));

            var service = new RegistrationService(
                registrationRepo.Object,
                courseRepo.Object);

            // Act
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.RegisterToCourseAsync(1, 2));

            // Assert
            Assert.Equal("Course is full.", exception.Message);

            registrationRepo.Verify(
                r => r.AddAsync(
                    It.IsAny<Registration>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task RegisterToCourse_WhenCourseAlreadyStarted_ThrowsException()
        {
            // Arrange
            var registrationRepo = new Mock<IRegistrationRepository>();
            var courseRepo = new Mock<ICourseRepository>();

            var course = new Course
            {
                Id = 2,
                Name = "Past Course",
                Description = "Test Description",
                LecturerName = "Test Lecturer",
                StartDate = DateTime.Now.AddDays(-1),
                StartTime = new TimeSpan(10, 0, 0),
                Capacity = 20,
                CategoryId = 1
            };

            courseRepo
                .Setup(r => r.GetByIdAsync(
                    2, It.IsAny<CancellationToken>()))
                .ReturnsAsync(course);

            registrationRepo
                .Setup(r => r.ExecuteInSerializableTransactionAsync(
                    It.IsAny<Func<CancellationToken, Task>>(),
                    It.IsAny<CancellationToken>()))
                .Returns((Func<CancellationToken, Task> operation,
                          CancellationToken ct) => operation(ct));

            var service = new RegistrationService(
                registrationRepo.Object,
                courseRepo.Object);

            // Act
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.RegisterToCourseAsync(1, 2));

            // Assert
            Assert.Equal(
                "Registration is closed for this course.",
                exception.Message);

            registrationRepo.Verify(
                r => r.AddAsync(
                    It.IsAny<Registration>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task RegisterToCourse_WhenValid_SavesRegistration()
        {
            // Arrange
            var registrationRepo = new Mock<IRegistrationRepository>();
            var courseRepo = new Mock<ICourseRepository>();

            var course = new Course
            {
                Id = 2,
                Name = "Test Course",
                Description = "Test Description",
                LecturerName = "Test Lecturer",
                StartDate = DateTime.Now.AddDays(30),
                StartTime = new TimeSpan(10, 0, 0),
                Capacity = 20,
                CategoryId = 1
            };

            courseRepo
                .Setup(r => r.GetByIdAsync(
                    2, It.IsAny<CancellationToken>()))
                .ReturnsAsync(course);

            registrationRepo
                .Setup(r => r.GetActiveRegistrationsCountAsync(
                    2, It.IsAny<CancellationToken>()))
                .ReturnsAsync(5);

            registrationRepo
                .Setup(r => r.ExecuteInSerializableTransactionAsync(
                    It.IsAny<Func<CancellationToken, Task>>(),
                    It.IsAny<CancellationToken>()))
                .Returns((Func<CancellationToken, Task> operation,
                          CancellationToken ct) => operation(ct));

            var service = new RegistrationService(
                registrationRepo.Object,
                courseRepo.Object);

            // Act
            await service.RegisterToCourseAsync(1, 2);

            // Assert
            registrationRepo.Verify(
                r => r.AddAsync(
                    It.Is<Registration>(registration =>
                        registration.UserId == 1 &&
                        registration.CourseId == 2 &&
                        registration.Status == RegistrationStatus.Active),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            registrationRepo.Verify(
                r => r.SaveChangesAsync(
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task RegisterToCourse_WhenCancelled_ReactivatesRegistration()
        {
            // Arrange
            var registrationRepo = new Mock<IRegistrationRepository>();
            var courseRepo = new Mock<ICourseRepository>();

            var course = new Course
            {
                Id = 2,
                Name = "Test Course",
                Description = "Test Description",
                LecturerName = "Test Lecturer",
                StartDate = DateTime.Now.AddDays(30),
                StartTime = new TimeSpan(10, 0, 0),
                Capacity = 20,
                CategoryId = 1
            };

            var existingRegistration = new Registration
            {
                Id = 1,
                UserId = 1,
                CourseId = 2,
                Status = RegistrationStatus.Cancelled,
                RegistrationDate = DateTime.Now.AddDays(-5)
            };

            courseRepo
                .Setup(r => r.GetByIdAsync(
                    2, It.IsAny<CancellationToken>()))
                .ReturnsAsync(course);

            registrationRepo
                .Setup(r => r.GetByUserAndCourseAsync(
                    1, 2, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingRegistration);

            registrationRepo
                .Setup(r => r.GetActiveRegistrationsCountAsync(
                    2, It.IsAny<CancellationToken>()))
                .ReturnsAsync(5);

            registrationRepo
                .Setup(r => r.ExecuteInSerializableTransactionAsync(
                    It.IsAny<Func<CancellationToken, Task>>(),
                    It.IsAny<CancellationToken>()))
                .Returns((Func<CancellationToken, Task> operation,
                          CancellationToken ct) => operation(ct));

            var service = new RegistrationService(
                registrationRepo.Object,
                courseRepo.Object);

            // Act
            await service.RegisterToCourseAsync(1, 2);

            // Assert
            Assert.Equal(RegistrationStatus.Active, existingRegistration.Status);

            registrationRepo.Verify(
                r => r.AddAsync(
                    It.IsAny<Registration>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            registrationRepo.Verify(
                r => r.SaveChangesAsync(
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }



    }
}
