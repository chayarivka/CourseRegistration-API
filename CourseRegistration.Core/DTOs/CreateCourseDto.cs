
using System.ComponentModel.DataAnnotations;

namespace CourseRegistration.Core.DTOs
{
    public class CreateCourseDto
    {
        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Range(0, 99999999)]
        public decimal Price { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public TimeSpan StartTime { get; set; }

        [Required]
        [StringLength(100)]
        public string LecturerName { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int NumberOfMeetings { get; set; }

        [Range(1, int.MaxValue)]
        public int Capacity { get; set; }

        [Range(1, int.MaxValue)]
        public int CategoryId { get; set; }
    }
}
