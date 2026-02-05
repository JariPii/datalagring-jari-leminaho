using SkillFlow.Domain.Interfaces;
using SkillFlow.Domain.Primitives;

namespace SkillFlow.Domain.Entities.Locations
{
    public class Location : BaseEntity<LocationId>, IAggregateRoot
    {
        public LocationName LocationName { get; private set; }

        public static Location Create(LocationName name) => new(LocationId.New(), name);

        protected Location(LocationId id, LocationName locationName)
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
