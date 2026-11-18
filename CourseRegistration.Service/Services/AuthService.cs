
using CourseRegistration.Core.DTOs;
using CourseRegistration.Core.Entities;
using CourseRegistration.Core.Enums;
using CourseRegistration.Core.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace CourseRegistration.Service.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;


        public AuthService(
            IUserRepository userRepository,
            IPasswordHasher<User> passwordHasher,
            IJwtTokenService jwtTokenService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
        }


        public async Task<LoginResponseDto> RegisterAsync(
            RegisterUserDto registerDto,
            CancellationToken cancellationToken = default)
        {
            bool emailExists = await _userRepository.EmailExistsAsync(
                registerDto.Email, cancellationToken);

            if (emailExists)
                throw new InvalidOperationException(
                    "Email is already registered.");

            var user = new User
            {
                FullName = registerDto.FullName,
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,
                Role = UserRole.User
            };

            user.PasswordHash = _passwordHasher.HashPassword(
                user, registerDto.Password);

            await _userRepository.AddAsync(user, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);


            return new LoginResponseDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.ToString(),
                Token = _jwtTokenService.GenerateToken(user)
            };

        }


        public async Task<LoginResponseDto?> LoginAsync(
            LoginDto loginDto,
            CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByEmailAsync(
                loginDto.Email, cancellationToken);

            if (user == null)
                return null;

            var result = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                loginDto.Password);

            if (result == PasswordVerificationResult.Failed)
                return null;


            return new LoginResponseDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.ToString(),
                Token = _jwtTokenService.GenerateToken(user)
            };

        }

    }
}
