
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CourseRegistration.Tests
{
    public class DatabaseSetupTests
    {
        [Fact]
        public async Task CreateTestDatabase()
        {
            await using var context = TestDbContextFactory.Create();

            await context.Database.MigrateAsync();

            Assert.True(await context.Database.CanConnectAsync());
        }
    }
}
