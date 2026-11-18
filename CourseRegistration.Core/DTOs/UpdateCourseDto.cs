
using System.ComponentModel.DataAnnotations;

namespace CourseRegistration.Core.DTOs
{
    public class UpdateCourseDto : CreateCourseDto
    {
        [Required]
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    }
}

