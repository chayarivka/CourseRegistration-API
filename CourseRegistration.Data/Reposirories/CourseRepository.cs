
using CourseRegistration.Core.Entities;
using CourseRegistration.Core.Interfaces;
using CourseRegistration.Data.Context;
using Microsoft.EntityFrameworkCore;
using CourseRegistration.Core.Enums;
using CourseRegistration.Core.DTOs;



namespace CourseRegistration.Data.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly AppDbContext _context;

        public CourseRepository(AppDbContext context)
        {
            _context = context;
        }


        public async Task<Course?> GetByIdAsync(
            int id, CancellationToken cancellationToken = default)
        {
            return await _context.Courses
                .Include(c => c.Category)
                .Include(c => c.Registrations)
                .FirstOrDefaultAsync(
                    c => c.Id == id, cancellationToken);
        }



        public async Task<List<Course>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.Courses
                .AsNoTracking()
                .Include(c => c.Category)
                .Include(c => c.Registrations)
                .ToListAsync(cancellationToken);
        }


        public async Task AddAsync(
            Course course, CancellationToken cancellationToken = default)
        {
            await _context.Courses.AddAsync(
                course, cancellationToken);
        }

        public void Update(Course course)
        {
            _context.Courses.Update(course);
        }

        public void Delete(Course course)
        {
            _context.Courses.Remove(course);
        }

        public async Task SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> CategoryExistsAsync(
            int categoryId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Categories
                .AnyAsync(c => c.Id == categoryId, cancellationToken);
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

        public void SetOriginalRowVersion(
            Course course, byte[] rowVersion)
        {
            _context.Entry(course)
                .Property(c => c.RowVersion)
                .OriginalValue = rowVersion;
        }

        public async Task<List<Course>> SearchAsync(
            CourseQueryDto query,
            CancellationToken cancellationToken = default)
        {
            var courses = _context.Courses
                .AsNoTracking()
                .Include(c => c.Category)
                .Include(c => c.Registrations)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                string search = query.Search.Trim();

                courses = courses.Where(c =>
                    c.Name.Contains(search));
            }

            if (query.CategoryId.HasValue)
            {
                courses = courses.Where(c =>
                    c.CategoryId == query.CategoryId.Value);
            }

            courses = query.SortBy.ToLowerInvariant() switch
            {
                "price" => query.Descending
                    ? courses.OrderByDescending(c => c.Price).ThenBy(c => c.Id)
                    : courses.OrderBy(c => c.Price).ThenBy(c => c.Id),

                _ => query.Descending
                    ? courses.OrderByDescending(c => c.StartDate)
                        .ThenByDescending(c => c.StartTime).ThenBy(c => c.Id)
                    : courses.OrderBy(c => c.StartDate)
                        .ThenBy(c => c.StartTime).ThenBy(c => c.Id)
            };

            int skip = (query.Page - 1) * query.PageSize;

            return await courses
                .Skip(skip)
                .Take(query.PageSize)
                .ToListAsync(cancellationToken);
        }



    }
}

