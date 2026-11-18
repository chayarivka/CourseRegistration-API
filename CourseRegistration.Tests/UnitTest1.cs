using Xunit;

namespace CourseRegistration.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void FirstTest()
        {
            int result = 2 + 3;

            Assert.Equal(5, result);
        }
    }
}