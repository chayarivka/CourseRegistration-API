
using CourseRegistration.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseRegistration.Data.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(c => c.Name)
                .IsUnique();

            builder.HasData(
                new Category { Id = 1, Name = "התפתחות שפה" },
                new Category { Id = 2, Name = "הפרעות דיבור" },
                new Category { Id = 3, Name = "תקשורת תומכת וחליפית" },
                new Category { Id = 4, Name = "שטף דיבור" }
            );

        }
    }
}
