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
        public async Task<LocationDTO> CreateLocationAsync(CreateLocationDTO dto, CancellationToken ct)
        {
            var locationName = LocationName.Create(dto.Name);

            if (await repository.ExistsByNameAsync(locationName, ct))
                throw new LocationNameAllreadyExistsException(locationName);

            var location = new Location(LocationId.New(), locationName);

            await repository.AddAsync(location, ct);

            return MatToDTO(location);
            //return new LocationDTO
            //{
            //    Id = location.Id.Value,
            //    LocationName = location.LocationName.Value
            //};
        }

        public async Task DeleteLocationAsync(Guid id, CancellationToken ct)
        {
            var locationId = new LocationId(id);

            var success = await repository.DeleteAsync(locationId, ct);

            if (!success)
                throw new LocationNotFoundException(locationId);
        }

        public async Task<IEnumerable<LocationDTO>> GetAllLocationsAsync(CancellationToken ct)
        {
            var locations = await repository.GetAllAsync(ct);

            return [.. locations.Select(MatToDTO)];
            //return locations.Select(l => new LocationDTO
            //{
            //    Id = l.Id.Value,
            //    LocationName = l.LocationName.Value
            //});
        }

        public async Task<LocationDTO> GetLocationByIdAsync(Guid id, CancellationToken ct)
        {
            var locationId = new LocationId(id);
            var location = await repository.GetByIdAsync(locationId, ct) ??
                throw new LocationNotFoundException(locationId);

            return MatToDTO(location);
            //return new LocationDTO
            //{
            //    Id = location.Id.Value,
            //    LocationName = location.LocationName.Value
            //};
        }

        public async Task<IEnumerable<LocationDTO>> SearchLocationsAsync(string searchTerm, CancellationToken ct)
        {
            var locations = await repository.SearchByNameAsync(searchTerm, ct);

            return [.. locations.Select(MatToDTO)];
            //return locations.Select(l => new LocationDTO
            //{
            //    Id = l.Id.Value,
            //    LocationName = l.LocationName.Value
            //});
        }

        public async Task UpdateLocationAsync(UpdateLocationDTO dto, CancellationToken ct)
        {
            var locationId = new LocationId(dto.Id);

            var location = await repository.GetByIdAsync(locationId, ct) ??
                throw new LocationNotFoundException(locationId);

            location.UpdateLocationName(LocationName.Create(dto.Name ?? location.LocationName.Value));

            await repository.UpdateAsync(location, ct);
        }

        private static LocationDTO MatToDTO(Location location)
        {
            return new LocationDTO
            {
                Id = location.Id.Value,
                LocationName = location.LocationName.Value
            };
        }
    }
}
