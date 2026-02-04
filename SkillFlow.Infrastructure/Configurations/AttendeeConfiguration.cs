using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillFlow.Domain.Attendees;
using SkillFlow.Domain.Entities.Attendees;
using SkillFlow.Domain.Enums;

namespace SkillFlow.Infrastructure.Configurations
{
    public class AttendeeConfiguration : BaseEntityConfiguration<Attendee, AttendeeId>
    {
        public override void Configure(EntityTypeBuilder<Attendee> builder)
        {
            base.Configure(builder);

            builder.Property(a => a.Id)
                .HasConversion(id => id.Value, v => new AttendeeId(v));

            builder.Property(a => a.Email)
                .HasConversion(e => e.Value, v => Email.Create(v))
                .HasMaxLength(150)
                .IsRequired();

            builder.HasIndex(a => a.Email).IsUnique();

            builder.ComplexProperty(a => a.Name, name =>            
            {
                name.Property(p => p.FirstName)
                .HasColumnName("FirstName")
                .IsRequired()
                .HasMaxLength(AttendeeName.MaxLength);

                name.Property(p => p.LastName)
                .HasColumnName("LastName")
                .IsRequired()
                .HasMaxLength(AttendeeName.MaxLength);
            });

            builder.Property(a => a.PhoneNumber)
                .HasConversion(p => p.HasValue ? p.Value.Value : null, v => v != null ? PhoneNumber.Create(v) : null)
                .IsRequired(false);

            builder.Property(a => a.Role)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.HasDiscriminator(a => a.Role)
                .HasValue<Student>(Role.Student)
                .HasValue<Instructor>(Role.Instructor);
        }
    }
}
