
namespace CourseRegistration.Core.Entities
{
    public class Course
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
        public int OccupiedSeats { get; set; } = 0;
        public int CategoryId { get; set; }

        public Category Category { get; set; } = null!;

        public ICollection<Registration> Registrations { get; set; }
            = new List<Registration>();

        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    }
}

