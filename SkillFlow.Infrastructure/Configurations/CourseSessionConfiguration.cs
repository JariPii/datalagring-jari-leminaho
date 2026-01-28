using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillFlow.Domain.Entities.Courses;
using SkillFlow.Domain.Entities.CourseSessions;
using SkillFlow.Domain.Entities.Locations;

namespace SkillFlow.Infrastructure.Configurations
{
    public class CourseSessionConfiguration : BaseEntityConfiguration<CourseSession>
    {
        public override void Configure(EntityTypeBuilder<CourseSession> builder)
        {
            base.Configure(builder);

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .HasConversion(id => id.Value, v => new CourseSessionId(v));

            builder.HasOne(c => c.Course)
                .WithMany()
                .HasPrincipalKey(c => c.CourseCode)
                .HasForeignKey(c => c.CourseCode)
                .IsRequired();

            builder.Property(c => c.CourseCode)
                .HasConversion(c => c.Value, v => CourseCode.FromValue(v))
                .HasMaxLength(12)
                .IsRequired();

            builder.HasOne(x => x.Location)
                .WithMany()
                .HasForeignKey(x => x.LocationId)
                .IsRequired();

            builder.Property(x => x.LocationId)
                .HasConversion(id => id.Value, v => new LocationId(v));

            builder.HasMany(c => c.Enrollments)
                .WithOne(e => e.CourseSession)
                .HasForeignKey(c => c.CourseSessionId);

            builder.Metadata
                .FindNavigation(nameof(CourseSession.Enrollments))?
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(c => c.Instructors)
                .WithMany()
                .UsingEntity(j => j.ToTable("CourseSessionInstructors"));

            builder.Metadata.FindNavigation(nameof(CourseSession.Instructors))?
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.Property(t => t.StartDate).IsRequired();
            builder.Property(t => t.EndDate).IsRequired();
            builder.Property(c => c.Capacity).IsRequired();

            builder.HasIndex(s => s.StartDate);
            builder.HasIndex(s => s.EndDate);
            builder.HasIndex(s => s.CourseCode);
        }
    }
}
