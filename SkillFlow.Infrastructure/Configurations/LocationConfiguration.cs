using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillFlow.Domain.Locations;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Infrastructure.Configurations
{
    public class LocationConfiguration : IEntityTypeConfiguration<Location>
    {
        public void Configure(EntityTypeBuilder<Location> builder)
        {
            builder.HasKey(l => l.Id);

            builder.Property(l => l.Id)
                .HasConversion(id => id.Value, v => new LocationId(v));

            builder.Property(l => l.LocationName)
                .HasConversion(l => l.Value, v => LocationName.Create(v))
                .HasMaxLength(LocationName.MaxLength);
        }
    }
}
