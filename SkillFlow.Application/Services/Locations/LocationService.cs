using Microsoft.EntityFrameworkCore;
using SkillFlow.Application.DTOs;
using SkillFlow.Application.DTOs.Locations;
using SkillFlow.Application.Interfaces;
using SkillFlow.Domain.Entities.Locations;
using SkillFlow.Domain.Exceptions;
using SkillFlow.Domain.Interfaces;

namespace SkillFlow.Application.Services.Locations
{
    public class LocationService(ILocationRepository repository, IUnitOfWork unitOfWork) : ILocationService
    {
        public async Task<LocationDTO> CreateLocationAsync(CreateLocationDTO dto, CancellationToken ct)
        {
            var locationName = LocationName.Create(dto.LocationName);

            if (await repository.ExistsByNameAsync(locationName, ct))
                throw new LocationNameAllreadyExistsException(locationName);

            var location = Location.Create(locationName);

            await repository.AddAsync(location, ct);

            await unitOfWork.SaveChangesAsync(ct);

            return MapToDTO(location);
        }

        public async Task DeleteLocationAsync(Guid id, CancellationToken ct)
        {
            var locationId = new LocationId(id);

            var location = await repository.GetByIdAsync(locationId, ct) ??
                throw new LocationNotFoundException(locationId);

            repository.Remove(location);

            try
            {
                await unitOfWork.SaveChangesAsync(ct);
            }
            catch (DbUpdateException)
            {
                throw new LocationInUseException(location.LocationName);
            }
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

        public async Task<LocationDTO> UpdateLocationAsync(Guid id, UpdateLocationDTO dto, CancellationToken ct)
        {
            var locationId = new LocationId(id);

            var location = await repository.GetByIdAsync(locationId, ct) ??
                throw new LocationNotFoundException(locationId);

            var newName = LocationName.Create(dto.LocationName ?? location.LocationName.Value);
            if (location.LocationName != newName && await repository.ExistsByNameAsync(newName, ct))
                throw new LocationNameAllreadyExistsException(newName);

            location.UpdateLocationName(newName);

            await repository.UpdateAsync(location, dto.RowVersion, ct);

            await unitOfWork.SaveChangesAsync(ct);

            return MapToDTO(location);
        }

        public static LocationDTO MapToDTO(Location location)
        {
            return new LocationDTO
            {
                Id = location.Id.Value,
                LocationName = location.LocationName.Value,
                RowVersion = location.RowVersion
            };
        }

        public async Task<PagedResultDTO<LocationDTO>> GetLocationsPagedAsync(int page, int pageSize, string? q, CancellationToken ct = default)
        {
            var result = await repository.GetLocationsPagedAsync(page, pageSize, q, ct);

            var items = result.Items.Select(MapToDTO).ToList();

            return new PagedResultDTO<LocationDTO>
            {
                Items = items,
                Page = result.Page,
                PageSize = result.PageSize,
                Total = result.Total
            };
        }
    }
}
