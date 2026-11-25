
using CourseRegistration.Core.Entities;
using CourseRegistration.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CourseRegistration.Tests
{
    public  class TestDbContextFactory
    {
        public static AppDbContext Create(bool enableRetries = false)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.Test.json")
                .Build();

            var connectionString =
                configuration.GetConnectionString("TestConnection");

            if (string.IsNullOrWhiteSpace(connectionString) ||
                !connectionString.Contains("Database=CourseRegistrationTestDB;",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Test database connection is missing or incorrect.");
            }

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(connectionString, sql =>
                {
                    if (enableRetries)
                    {
                        sql.EnableRetryOnFailure();
                    }
                })
                .Options;

            return new AppDbContext(options);
        }


    }
}
