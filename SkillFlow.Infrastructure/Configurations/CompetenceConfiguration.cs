using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillFlow.Domain.Attendees;
using SkillFlow.Domain.Courses;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Infrastructure.Configurations
{
    public class CompetenceConfiguration : IEntityTypeConfiguration<Competence>
    {
        public void Configure(EntityTypeBuilder<Competence> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id)
                .HasConversion(id => id.Value, v => new CompetenceId(v));

            builder.Property(c => c.Name)
                .HasConversion(c => c.Value, v => CompetenceName.Create(v))
                .HasMaxLength(CompetenceName.MaxLength);

            builder.HasMany(c => c.Instructors)
                .WithMany(i => i.Competences)
                .UsingEntity(j => j.ToTable("InstructorCompetence"));

            builder.Metadata
                .FindNavigation(nameof(Competence.Instructors))?
                .SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
