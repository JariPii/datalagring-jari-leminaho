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
        public LocationName LocationName { get; private set; }

        public Location(LocationId id, LocationName locationName)
        {
            Id = id;
            LocationName = locationName;
        }

        private Location () { }

        public void UpdateLocationName(LocationName updatedLocationName)
        {           

            if (LocationName == updatedLocationName) return;

            LocationName = updatedLocationName;
            UpdateTimeStamp();
        }
    }
}
