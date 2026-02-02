using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillFlow.Domain.Entities.Attendees;
using SkillFlow.Domain.Entities.CourseSessions;

namespace SkillFlow.Infrastructure.Configurations
{
    public class EnrollmentConfiguration : BaseEntityConfiguration<Enrollment, EnrollmentId>
    {
        public override void Configure(EntityTypeBuilder<Enrollment> builder)
        {
            base.Configure(builder);

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id)
                .HasConversion(id => id.Value, v => new EnrollmentId(v));

            builder.Property(e => e.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(e => e.StudentId)
                .HasConversion(id => id.Value, v => new AttendeeId(v));

            builder.Property(e => e.CourseSessionId)
                .HasConversion(id => id.Value, v => new CourseSessionId(v));

            builder.HasOne(e => e.Student)
                .WithMany()
                .HasForeignKey(e => e.StudentId);

            builder.HasOne(e => e.CourseSession)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.CourseSessionId);

            builder.HasIndex(e => new { e.StudentId, e.CourseSessionId }).IsUnique();
        }
    }
}
