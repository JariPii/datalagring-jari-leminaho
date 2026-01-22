using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillFlow.Domain.Attendees;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Infrastructure.Configurations
{
    public class InstructorConfiguration : IEntityTypeConfiguration<Instructor>
    {
        public void Configure(EntityTypeBuilder<Instructor> builder)
        {
            builder.HasMany(i => i.Competences)
                .WithMany(c => c.Instructors)
                .UsingEntity(j => j.ToTable("InstructorCompetences"));

            builder.Metadata
                .FindNavigation(nameof(Instructor.Competences))?
                .SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
