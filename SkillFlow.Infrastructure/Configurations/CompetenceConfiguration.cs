using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillFlow.Domain.Courses;
using SkillFlow.Domain.Entities.Courses;

namespace SkillFlow.Infrastructure.Configurations
{
    public class CompetenceConfiguration : BaseEntityConfiguration<Competence>
    {
        public override void Configure(EntityTypeBuilder<Competence> builder)
        {
            base.Configure(builder);

            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id)
                .HasConversion(id => id.Value, v => new CompetenceId(v));

            builder.Property(c => c.Name)
                .HasConversion(c => c.Value, v => CompetenceName.Create(v))
                .HasMaxLength(CompetenceName.MaxLength);

            builder.HasMany(c => c.Instructors)
                .WithMany(i => i.Competences)
                .UsingEntity(j => j.ToTable("InstructorCompetences"));

            builder.Metadata
                .FindNavigation(nameof(Competence.Instructors))?
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.HasIndex(c => c.Name).IsUnique();
        }
    }
}
