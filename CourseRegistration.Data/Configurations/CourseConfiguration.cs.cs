
using CourseRegistration.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseRegistration.Data.Configurations
{
    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.HasOne(c => c.Category)
                .WithMany(c => c.Courses)
                .HasForeignKey(c => c.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(c => c.RowVersion)
                .IsRowVersion();

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(c => c.Description)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(c => c.LecturerName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Price)
                .HasPrecision(10, 2);

        }
    }
}

