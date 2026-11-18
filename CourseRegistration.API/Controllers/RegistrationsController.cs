
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
