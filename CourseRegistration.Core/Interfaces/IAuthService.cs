
using CourseRegistration.Core.DTOs;

namespace CourseRegistration.Core.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> RegisterAsync(
            RegisterUserDto registerDto,
            CancellationToken cancellationToken = default);

        Task<LoginResponseDto?> LoginAsync(
            LoginDto loginDto,
            CancellationToken cancellationToken = default);
    }
}
