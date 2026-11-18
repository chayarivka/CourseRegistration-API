
using CourseRegistration.Core.Entities;
using CourseRegistration.Core.Enums;
using CourseRegistration.Data.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CourseRegistration.Data.Seed
{
    public static class AdminSeeder
    {
        public static async Task SeedAsync(
            AppDbContext context,
            IPasswordHasher<User> passwordHasher,
            IConfiguration configuration,
            bool isDevelopment)
        {
            string? email = configuration["AdminSeed:Email"];
            string? password = configuration["AdminSeed:Password"];
            string? fullName = configuration["AdminSeed:FullName"];

            bool resetPassword = bool.TryParse(
     configuration["AdminSeed:ResetPassword"],
     out bool shouldReset) && shouldReset;

            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                return;
            }

            // Reset an existing admin password only in Development.
            if (isDevelopment && resetPassword)
            {
                var existingAdmin = await context.Users
                    .FirstOrDefaultAsync(u =>
                        u.Email == email &&
                        u.Role == UserRole.Admin);

                if (existingAdmin == null)
                {
                    throw new InvalidOperationException(
                        "Admin account for password reset was not found.");
                }

                existingAdmin.PasswordHash =
                    passwordHasher.HashPassword(existingAdmin, password);

                await context.SaveChangesAsync();
                return;
            }

            // Do not create another admin if one already exists.
            bool adminExists = await context.Users
                .AnyAsync(u => u.Role == UserRole.Admin);

            if (adminExists)
                return;

            if (string.IsNullOrWhiteSpace(fullName))
                return;

            var admin = new User
            {
                FullName = fullName,
                Email = email,
                Role = UserRole.Admin
            };

            admin.PasswordHash =
                passwordHasher.HashPassword(admin, password);

            context.Users.Add(admin);
            await context.SaveChangesAsync();
        }
    }
}
