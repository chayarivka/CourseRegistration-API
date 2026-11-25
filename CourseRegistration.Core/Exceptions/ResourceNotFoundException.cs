namespace CourseRegistration.Core.Exceptions
{
    public sealed class ResourceNotFoundException : Exception
    {
        public ResourceNotFoundException(string message) : base(message)
        {
        }
    }
}
