using System.ComponentModel.DataAnnotations;
using CourseRegistration.Core.DTOs;
using CourseRegistration.Core.Entities;
using CourseRegistration.Core.Enums;
using CourseRegistration.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CourseRegistration.Tests;

public class CourseReadTests
{
    [Fact]
    public async Task CourseReads_ReturnProjectedValuesWithoutTrackingAndPageInIdOrder()
    {
        await using var setup = TestDbContextFactory.Create();
        await using var transaction = await setup.Database.BeginTransactionAsync();
        var category = await setup.Categories.FirstAsync();
        var prefix = Guid.NewGuid().ToString("N");
        var start = DateTime.Today.AddDays(30);
        var first = new Course
        {
            Name = prefix + " first",
            Description = "First description",
            Price = 100,
            StartDate = start,
            StartTime = TimeSpan.FromHours(10),
            LecturerName = "First lecturer",
            NumberOfMeetings = 5,
            Capacity = 3,
            OccupiedSeats = 0,
            CategoryId = category.Id
        };
        var second = new Course
        {
            Name = prefix + " second",
            Description = "Second description",
            Price = first.Price,
            StartDate = first.StartDate,
            StartTime = first.StartTime,
            LecturerName = "Second lecturer",
            NumberOfMeetings = 2,
            Capacity = 2,
            CategoryId = category.Id
        };
        foreach (var status in new[] { RegistrationStatus.Active, RegistrationStatus.Cancelled })
        {
            first.Registrations.Add(new Registration
            {
                User = new User
                {
                    Email = $"{prefix}-{status}@example.test",
                    FullName = "Course read test",
                    PasswordHash = "test"
                },
                RegistrationDate = DateTime.Now,
                Status = status
            });
        }

        setup.Courses.AddRange(first, second);
        await setup.SaveChangesAsync();
        setup.ChangeTracker.Clear();

        var repository = new CourseRepository(setup);
        var details = await repository.GetResponseByIdAsync(first.Id);
        Assert.NotNull(details);
        Assert.Equal(first.Id, details.Id);
        Assert.Equal(first.Name, details.Name);
        Assert.Equal(first.Description, details.Description);
        Assert.Equal(first.Price, details.Price);
        Assert.Equal(first.StartDate, details.StartDate);
        Assert.Equal(first.StartTime, details.StartTime);
        Assert.Equal(first.LecturerName, details.LecturerName);
        Assert.Equal(first.NumberOfMeetings, details.NumberOfMeetings);
        Assert.Equal(first.Capacity, details.Capacity);
        Assert.Equal(2, details.AvailableSeats);
        Assert.Equal(category.Id, details.CategoryId);
        Assert.Equal(category.Name, details.CategoryName);
        Assert.Equal(first.RowVersion, details.RowVersion);
        Assert.Null(await repository.GetResponseByIdAsync(0));

        var all = await repository.GetAllAsync();
        Assert.Contains(all, course => course.Id == first.Id && course.AvailableSeats == 2);
        var query = new CourseQueryDto
        {
            Search = prefix,
            CategoryId = category.Id,
            SortBy = "price",
            PageSize = 1
        };

        var orderedIds = new[] { first.Id, second.Id }.Order().ToArray();
        Assert.Equal(orderedIds[0], Assert.Single(await repository.SearchAsync(query)).Id);
        query.Page = 2;
        Assert.Equal(orderedIds[1], Assert.Single(await repository.SearchAsync(query)).Id);
        query.Page = 3;
        Assert.Empty(await repository.SearchAsync(query));

        query.Page = 214748365;
        query.PageSize = 10;
        Assert.Empty(await repository.SearchAsync(query));
        Assert.Empty(setup.ChangeTracker.Entries());

        var tracked = await repository.GetByIdAsync(first.Id);
        Assert.NotNull(tracked);
        Assert.Equal(EntityState.Unchanged, setup.Entry(tracked).State);
        Assert.False(setup.Entry(tracked).Collection(course => course.Registrations).IsLoaded);
    }

    [Theory]
    [InlineData(214748365, 10, true)]
    [InlineData(214748366, 10, false)]
    [InlineData(int.MaxValue, 1, true)]
    [InlineData(int.MaxValue, 10, false)]
    public void PaginationValidation_RejectsOffsetsAboveIntMaxValue(
        int page, int pageSize, bool expectedValid)
    {
        var query = new CourseQueryDto { Page = page, PageSize = pageSize };
        var errors = new List<ValidationResult>();

        bool valid = Validator.TryValidateObject(
            query, new ValidationContext(query), errors, validateAllProperties: true);

        Assert.Equal(expectedValid, valid);
        if (!expectedValid)
        {
            Assert.Contains(nameof(CourseQueryDto.Page), Assert.Single(errors).MemberNames);
        }
    }
}
