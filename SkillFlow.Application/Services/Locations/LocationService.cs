using SkillFlow.Application.DTOs.Locations;
using SkillFlow.Application.Interfaces;
using SkillFlow.Domain.Exceptions;
using SkillFlow.Domain.Interfaces;
using SkillFlow.Domain.Locations;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Application.Services.Locations
{
    public class LocationService(ILocationRepository repository) : ILocationService
    {
        public async Task<LocationDTO> CreateLocationAsync(CreateLocationDTO dto)
        {
            var locationName = LocationName.Create(dto.Name);

            if (await repository.ExistsByNameAsync(locationName))
                throw new LocationNameAllreadyExistsException(locationName);

            var location = new Location(LocationId.New(), locationName);

            await repository.AddAsync(location);

            return new LocationDTO
            {
                Id = location.Id.Value,
                LocationName = location.LocationName.Value
            };
        }

        public async Task DeleteLocationAsync(Guid id)
        {
            var locationId = new LocationId(id);

            var success = await repository.DeleteAsync(locationId);

            if (!success)
                throw new LocationNotFoundException(locationId);
        }

        public async Task<IEnumerable<LocationDTO>> GetAllLocationsAsync()
        {
            var locations = await repository.GetAllAsync();

            return locations.Select(l => new LocationDTO
            {
                Id = l.Id.Value,
                LocationName = l.LocationName.Value
            });
        }

        public async Task<LocationDTO> GetLocationByIdAsync(Guid id)
        {
            var locationId = new LocationId(id);
            var location = await repository.GetByIdAsync(locationId) ??
                throw new LocationNotFoundException(locationId);

            return new LocationDTO
            {
                Id = location.Id.Value,
                LocationName = location.LocationName.Value
            };
        }

        public async Task<IEnumerable<LocationDTO>> SearchLocationsAsync(string searchTerm)
        {
            var locations = await repository.SearchByNameAsync(searchTerm);

            return locations.Select(l => new LocationDTO
            {
                Id = l.Id.Value,
                LocationName = l.LocationName.Value
            });
        }

        public async Task UpdateLocationAsync(UpdateLocationDTO dto)
        {
            var locationId = new LocationId(dto.Id);

            var location = await repository.GetByIdAsync(locationId) ??
                throw new LocationNotFoundException(locationId);

            location.UpdateLocationName(LocationName.Create(dto.Name ?? location.LocationName.Value));

            await repository.UpdateAsync(location);
        }
    }
}
