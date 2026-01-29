using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillFlow.Domain.Courses;
using SkillFlow.Domain.Entities.Courses;

namespace SkillFlow.Infrastructure.Configurations
{
    public class CourseConfiguration : BaseEntityConfiguration<Course>
    {
        public override void Configure(EntityTypeBuilder<Course> builder)
        {
            base.Configure(builder);

            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasConversion(id => id.Value, v => new CourseId(v));

            builder.Property(c => c.CourseCode)
                .HasConversion(c => c.Value, v => CourseCode.FromValue(v))
                .HasMaxLength(12)
                .IsRequired();

            builder.HasIndex(c => c.CourseCode).IsUnique();

            builder.Property(n => n.CourseName)
                .HasConversion(v => v.Value, v => CourseName.Create(v))
                .HasMaxLength(CourseName.MaxLength)
                .IsRequired();

            builder.Property(d => d.CourseDescription)
                .HasConversion(v => v.Value, v => CourseDescription.Create(v))
                .HasMaxLength(CourseDescription.MaxLength)
                .IsRequired();
        }
    }
}
