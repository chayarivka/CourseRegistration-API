
namespace CourseRegistration.Core.DTOs
{
    public class CourseResponseDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public DateTime StartDate { get; set; }

        public TimeSpan StartTime { get; set; }

        public string LecturerName { get; set; } = string.Empty;

        public int NumberOfMeetings { get; set; }

        public int Capacity { get; set; }

        public int AvailableSeats { get; set; }

        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    }
}
