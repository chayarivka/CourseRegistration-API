
using CourseRegistration.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseRegistration.Data.Configurations
{
    public class RegistrationConfiguration : IEntityTypeConfiguration<Registration>
    {
        public void Configure(EntityTypeBuilder<Registration> builder)
        {
            builder.HasOne(r => r.User)
                .WithMany(u => u.Registrations)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.Course)
                .WithMany(c => c.Registrations)
                .HasForeignKey(r => r.CourseId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

