
using CourseRegistration.Core.DTOs;
using CourseRegistration.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CourseRegistration.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<LoginResponseDto>> Register(
            [FromBody] RegisterUserDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _authService.RegisterAsync(
                dto, cancellationToken);

            return Ok(result);
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<LoginResponseDto>> Login(
            [FromBody] LoginDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _authService.LoginAsync(
                dto, cancellationToken);

            if (result == null)
                return Unauthorized(new
                {
                    Message = "Invalid email or password."
                });

            return Ok(result);
        }
    }
}
