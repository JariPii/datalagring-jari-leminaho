using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillFlow.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Infrastructure.Configurations
{
    public abstract class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
        where TEntity : BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.Property(e => e.CreatedAt).HasColumnType("datetime2").IsRequired();
            builder.Property(e => e.UpdatedAt).HasColumnType("datetime2").IsRequired(false);

            builder.Property(e => e.RowVersion).IsRowVersion();
        }
    }
}
