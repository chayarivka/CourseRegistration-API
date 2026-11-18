
using CourseRegistration.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseRegistration.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasIndex(u => u.Email)
                .IsUnique();

            builder.Property(u => u.FullName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(u => u.PasswordHash)
                .IsRequired();

            builder.Property(u => u.PhoneNumber)
                .HasMaxLength(20);

            builder.Property(u => u.Role)
                .HasConversion<string>()
                .HasMaxLength(20);
        }
    }
}

