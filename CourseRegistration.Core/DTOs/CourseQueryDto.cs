
using System.ComponentModel.DataAnnotations;

namespace CourseRegistration.Core.DTOs
{
    public class CourseQueryDto : IValidatableObject
    {
        public string? Search { get; set; }

        public int? CategoryId { get; set; }

        public string SortBy { get; set; } = "StartDate";

        public bool Descending { get; set; } = false;

        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;

        [Range(1, 10)]
        public int PageSize { get; set; } = 10;

        public long GetOffset() => ((long)Page - 1) * PageSize;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (GetOffset() > int.MaxValue)
            {
                yield return new ValidationResult(
                    "The requested page exceeds the supported pagination offset.",
                    new[] { nameof(Page) });
            }
        }
    }
}
