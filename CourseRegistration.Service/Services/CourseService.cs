
using AutoMapper;
using CourseRegistration.Core.DTOs;
using CourseRegistration.Core.Entities;
using CourseRegistration.Core.Interfaces;

namespace CourseRegistration.Service.Services
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IMapper _mapper;

        public CourseService(
            ICourseRepository courseRepository,
            IMapper mapper)
        {
            _courseRepository = courseRepository;
            _mapper = mapper;
        }

        public async Task<CourseResponseDto?> GetByIdAsync(
            int id, CancellationToken cancellationToken = default)
        {
            var course = await _courseRepository.GetByIdAsync(
                id, cancellationToken);

            return _mapper.Map<CourseResponseDto?>(course);
        }

        public async Task<List<CourseResponseDto>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            var courses = await _courseRepository.GetAllAsync(
                cancellationToken);

            return _mapper.Map<List<CourseResponseDto>>(courses);
        }


        public async Task<CourseResponseDto> CreateAsync(
            CreateCourseDto courseDto,
            CancellationToken cancellationToken = default)
        {
            bool categoryExists =
                await _courseRepository.CategoryExistsAsync(
                    courseDto.CategoryId, cancellationToken);

            if (!categoryExists)
                throw new ArgumentException("Category does not exist.");

            DateTime courseStart =
                courseDto.StartDate.Date.Add(courseDto.StartTime);

            if (courseStart <= DateTime.Now)
                throw new ArgumentException(
                    "Course start date and time must be in the future.");

            var course = _mapper.Map<Course>(courseDto);
            await _courseRepository.AddAsync(course, cancellationToken);
            await _courseRepository.SaveChangesAsync(cancellationToken);

            var createdCourse = await _courseRepository.GetByIdAsync(
                course.Id, cancellationToken);

            return _mapper.Map<CourseResponseDto>(createdCourse);
        }



        public async Task<bool> UpdateAsync(
            int id, UpdateCourseDto courseDto,
            CancellationToken cancellationToken = default)
        {
            var existingCourse = await _courseRepository.GetByIdAsync(
                id, cancellationToken);

            if (existingCourse == null)
                return false;

            bool categoryExists =
                await _courseRepository.CategoryExistsAsync(
                    courseDto.CategoryId, cancellationToken);

            if (!categoryExists)
                throw new ArgumentException("Category does not exist.");

            int activeCount =
                await _courseRepository.GetActiveRegistrationsCountAsync(
                    id, cancellationToken);

            if (courseDto.Capacity < activeCount)
                throw new ArgumentException(
                    "Capacity cannot be lower than active registrations.");

            if (activeCount > 0 &&
                (existingCourse.StartDate.Date != courseDto.StartDate.Date ||
                 existingCourse.StartTime != courseDto.StartTime))
            {
                throw new InvalidOperationException(
                    "Cannot change start date or time when active registrations exist.");
            }

            DateTime courseStart =
                courseDto.StartDate.Date.Add(courseDto.StartTime);

            if (courseStart <= DateTime.Now)
                throw new ArgumentException(
                    "Course start date and time must be in the future.");

            _mapper.Map(courseDto, existingCourse);

            _courseRepository.SetOriginalRowVersion(
                existingCourse, courseDto.RowVersion);


            await _courseRepository.SaveChangesAsync(cancellationToken);

            return true;
        }



        public async Task<bool> DeleteAsync(
            int id, CancellationToken cancellationToken = default)
        {
            var course = await _courseRepository.GetByIdAsync(
                id, cancellationToken);

            if (course == null)
                return false;

            int activeCount =
                await _courseRepository.GetActiveRegistrationsCountAsync(
                    id, cancellationToken);

            if (activeCount > 0)
                throw new InvalidOperationException(
                    "Cannot delete a course with active registrations.");

            _courseRepository.Delete(course);

            await _courseRepository.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<List<CourseResponseDto>> SearchAsync(
            CourseQueryDto query,
            CancellationToken cancellationToken = default)
        {
            var courses = await _courseRepository.SearchAsync(
                query, cancellationToken);

            return _mapper.Map<List<CourseResponseDto>>(courses);
        }


    }
}
