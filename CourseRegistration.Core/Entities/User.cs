
using CourseRegistration.Core.Enums;

namespace CourseRegistration.Core.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }


        public UserRole Role { get; set; } = UserRole.User;


        public ICollection<Registration> Registrations { get; set; }
            = new List<Registration>();
    }
}

