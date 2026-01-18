using SkillFlow.Domain.Attendees;
using SkillFlow.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Locations
{
    public class Location : BaseEntity
    {
        public LocationId Id { get; private set; } = null!;
        public string LocationName { get; private set; } = null!;

        public Location(LocationId id, string locationName)
        {
            if (string.IsNullOrWhiteSpace(locationName))
                throw new ArgumentException("Location name is required", nameof(locationName));

            Id = id;
            LocationName = locationName;
        }

        private Location () { }

        public void UpdateLocationName(string newLocationName)
        {
            if (string.IsNullOrWhiteSpace(newLocationName))
                throw new ArgumentException("Location name can not be empry", nameof(newLocationName));

            if (LocationName == newLocationName) return;

            LocationName = newLocationName;
            UpdateTimeStamp();
        }
    }
}
