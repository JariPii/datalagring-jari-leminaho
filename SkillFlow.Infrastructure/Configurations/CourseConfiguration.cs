using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillFlow.Domain.Courses;
using SkillFlow.Domain.Entities.Courses;

namespace SkillFlow.Infrastructure.Configurations
{
    public class CourseConfiguration : BaseEntityConfiguration<Course, CourseId>
    {
        public override void Configure(EntityTypeBuilder<Course> builder)
        {
            base.Configure(builder);

            builder.Property(c => c.Id).HasConversion(id => id.Value, v => new CourseId(v));

            var codeProp = builder.Property(c => c.CourseCode)
                .HasConversion(code => code.Value, v => CourseCode.FromValue(v))
                .HasMaxLength(10)
                .IsRequired();

            codeProp.Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);

            builder.HasIndex(c => c.CourseCode).IsUnique();

            builder.Property(n => n.CourseName)
                .HasConversion(v => v.Value, v => CourseName.Create(v))
                .HasMaxLength(CourseName.MaxLength)
                .IsRequired();

            builder.Property(d => d.CourseDescription)
                .HasConversion(v => v.Value, v => CourseDescription.Create(v))
                .HasMaxLength(CourseDescription.MaxLength)
                .IsRequired();

            builder.Property(c => c.CourseType)
                .HasConversion<string>()
                .HasMaxLength(3)
                .IsRequired();
        }
    }
}
