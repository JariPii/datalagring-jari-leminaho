using SkillFlow.Domain.Interfaces;
using SkillFlow.Domain.Primitives;

namespace SkillFlow.Domain.Entities.Locations
{
    public class Location : BaseEntity<LocationId>, IAggregateRoot
    {
        //public LocationId Id { get; private set; }
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
