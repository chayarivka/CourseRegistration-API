
using CourseRegistration.Core.Entities;

namespace CourseRegistration.Core.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user);
    }
}
