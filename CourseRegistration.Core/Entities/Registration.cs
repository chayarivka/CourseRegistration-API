
using CourseRegistration.Core.Enums;

namespace CourseRegistration.Core.Entities
{
    public class Registration
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public User User { get; set; } = null!;

        public int CourseId { get; set; }

        public Course Course { get; set; } = null!;

        public DateTime RegistrationDate { get; set; }

        public RegistrationStatus Status { get; set; }
            = RegistrationStatus.Active;
    }
}

