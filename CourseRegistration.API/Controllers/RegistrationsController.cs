
using CourseRegistration.Core.DTOs;
using CourseRegistration.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseRegistration.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RegistrationsController : ControllerBase
    {
        private readonly IRegistrationService _registrationService;

        public RegistrationsController(
            IRegistrationService registrationService)
        {
            _registrationService = registrationService;
        }

        [HttpPost("{courseId}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register(
            int courseId,
            CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst(
                System.Security.Claims.ClaimTypes.NameIdentifier);

            if (userIdClaim == null ||
                !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized();
            }

            await _registrationService.RegisterToCourseAsync(
                userId, courseId, cancellationToken);

            return Ok("Registration completed successfully.");
        }

        [HttpDelete("{courseId}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CancelRegistration(
            int courseId,
            CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst(
                System.Security.Claims.ClaimTypes.NameIdentifier);

            if (userIdClaim == null ||
                !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized();
            }

            await _registrationService.CancelRegistrationAsync(
                userId, courseId, cancellationToken);

            return Ok("Registration cancelled successfully.");
        }

        [HttpGet("my-courses")]
        [ProducesResponseType(typeof(List<MyCourseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMyCourses(
            CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst(
                System.Security.Claims.ClaimTypes.NameIdentifier);

            if (userIdClaim == null ||
                !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized();
            }

            var courses = await _registrationService.GetMyCoursesAsync(
                userId, cancellationToken);

            return Ok(courses);
        }



    }
}
