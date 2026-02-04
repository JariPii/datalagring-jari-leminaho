using SkillFlow.Application.DTOs.Locations;
using SkillFlow.Application.Interfaces;
using SkillFlow.Domain.Entities.Locations;
using SkillFlow.Domain.Exceptions;
using SkillFlow.Domain.Interfaces;

namespace SkillFlow.Application.Services.Locations
{
    public class LocationService(ILocationRepository repository) : ILocationService
    {
        public async Task<LocationDTO> CreateLocationAsync(CreateLocationDTO dto, CancellationToken ct)
        {
            var locationName = LocationName.Create(dto.Name);

            if (await repository.ExistsByNameAsync(locationName, ct))
                throw new LocationNameAllreadyExistsException(locationName);

            var location = Location.Create(locationName);

            await repository.AddAsync(location, ct);

            return MapToDTO(location);
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

            return [.. locations.Select(MapToDTO)];
        }

        public async Task<LocationDTO> GetLocationByIdAsync(Guid id, CancellationToken ct)
        {
            var locationId = new LocationId(id);
            var location = await repository.GetByIdAsync(locationId, ct) ??
                throw new LocationNotFoundException(locationId);

            return MapToDTO(location);
        }

        public async Task<IEnumerable<LocationDTO>> SearchLocationsAsync(string searchTerm, CancellationToken ct)
        {
            var locations = await repository.SearchByNameAsync(searchTerm, ct);

            return [.. locations.Select(MapToDTO)];
        }

        public async Task<LocationDTO> UpdateLocationAsync(UpdateLocationDTO dto, CancellationToken ct)
        {
            var locationId = new LocationId(dto.Id);

            var location = await repository.GetByIdAsync(locationId, ct) ??
                throw new LocationNotFoundException(locationId);

            var newName = LocationName.Create(dto.Name ?? location.LocationName.Value);
            if (location.LocationName != newName && await repository.ExistsByNameAsync(newName, ct))
                throw new LocationNameAllreadyExistsException(newName);

            location.UpdateLocationName(newName);

            await repository.UpdateAsync(location, dto.RowVersion, ct);
            return MapToDTO(location);
        }

        private static LocationDTO MapToDTO(Location location)
        {
            return new LocationDTO
            {
                Id = location.Id.Value,
                LocationName = location.LocationName.Value,
                RowVersion = location.RowVersion
            };
        }
    }
}
