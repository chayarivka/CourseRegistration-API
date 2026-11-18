
using System.ComponentModel.DataAnnotations;

namespace CourseRegistration.Core.DTOs
{
    public class CourseQueryDto
    {
        public string? Search { get; set; }

        public int? CategoryId { get; set; }

        public string SortBy { get; set; } = "StartDate";

        public bool Descending { get; set; } = false;

        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;

        [Range(1, 10)]
        public int PageSize { get; set; } = 10;
    }
}
