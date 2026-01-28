using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillFlow.Domain.Locations;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Infrastructure.Configurations
{
    public class LocationConfiguration : BaseEntityConfiguration<Location>
    {
        public override void Configure(EntityTypeBuilder<Location> builder)
        {
            base.Configure(builder);

            builder.HasKey(l => l.Id);

            builder.Property(l => l.Id)
                .HasConversion(id => id.Value, v => new LocationId(v));

            builder.Property(l => l.LocationName)
                .HasConversion(l => l.Value, v => LocationName.Create(v))
                .HasMaxLength(LocationName.MaxLength);
        }
    }
}
