using SkillFlow.Domain.Primitives;

namespace SkillFlow.Domain.Entities.Locations
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
