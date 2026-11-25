
using CourseRegistration.Core.DTOs;
using CourseRegistration.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CourseRegistration.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CoursesController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CourseResponseDto>> GetById(
            [FromRoute] int id,
            CancellationToken cancellationToken)
        {
            var course = await _courseService.GetByIdAsync(
                id, cancellationToken);

            if (course == null)
                return NotFound();

            return Ok(course);
        }

        [HttpGet]
        public async Task<ActionResult<List<CourseResponseDto>>> GetAll(
            [FromQuery] CourseQueryDto query,
            CancellationToken cancellationToken)
        {
            var courses = await _courseService.SearchAsync(
                query, cancellationToken);

            return Ok(courses);
        }

       
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<CourseResponseDto>> Create(
            [FromBody] CreateCourseDto courseDto,
            CancellationToken cancellationToken)
        {
            var createdCourse = await _courseService.CreateAsync(
                courseDto, cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdCourse.Id },
                createdCourse);
        }

        
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Update(
            [FromRoute] int id,
            [FromBody] UpdateCourseDto courseDto,
            CancellationToken cancellationToken)
        {
            bool updated = await _courseService.UpdateAsync(
                id, courseDto, cancellationToken);

            if (!updated)
                return NotFound();

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(
            [FromRoute] int id,
            CancellationToken cancellationToken)
        {
            bool deleted = await _courseService.DeleteAsync(
                id, cancellationToken);

            if (!deleted)
                return NotFound();

            return NoContent();
        }




    }
}

