
using System.ComponentModel.DataAnnotations;

namespace CourseRegistration.Core.DTOs
{
    public class UpdateCourseDto : CreateCourseDto
    {
        [Required]
        [MinLength(8)]
        [MaxLength(8)]
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    }
}

