
namespace CourseRegistration.Core.DTOs
{
    public class MyCourseDto
    {
        public int RegistrationId { get; set; }

        public int CourseId { get; set; }

        public string CourseName { get; set; } = string.Empty;

        public string LecturerName { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }

        public TimeSpan StartTime { get; set; }

        public DateTime RegistrationDate { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}
