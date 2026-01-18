using SkillFlow.Domain.Attendees;
using SkillFlow.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Locations
{
    public class Location : BaseEntity
    {
        public LocationId Id { get; private set; }
        public string LocationName { get; private set; } = null!;

        public Location(LocationId id, string locationName)
        {
            if (id.Value == Guid.Empty)
                throw new ArgumentException("Location Id can not be empty", nameof(id));

            if (string.IsNullOrWhiteSpace(locationName))
                throw new ArgumentException("Location name is required", nameof(locationName));

            Id = id;
            LocationName = locationName.NormalizeName();
        }

        protected Location () { }

        public void UpdateLocationName(string newLocationName)
        {
            if (string.IsNullOrWhiteSpace(newLocationName))
                throw new ArgumentException("Location name can not be empty", nameof(newLocationName));

            var normalizedName = newLocationName.NormalizeName();

            if (LocationName == normalizedName) return;

            LocationName = normalizedName;
            UpdateTimeStamp();
        }
    }
}
