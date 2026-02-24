using SkillFlow.Application.DTOs;
using SkillFlow.Application.DTOs.Locations;

namespace SkillFlow.Application.Interfaces
{
    public interface ILocationService
    {
        Task<IEnumerable<LocationDTO>> GetAllLocationsAsync(CancellationToken ct = default);

        Task<LocationDTO> GetLocationByIdAsync(Guid id, CancellationToken ct = default);

        Task<LocationDTO> CreateLocationAsync(CreateLocationDTO dto, CancellationToken ct = default);

        Task<IEnumerable<LocationDTO>> SearchLocationsAsync(string searchTerm, CancellationToken ct = default);

        Task<LocationDTO> UpdateLocationAsync(Guid id, UpdateLocationDTO dto, CancellationToken ct = default);

        Task DeleteLocationAsync(Guid id, CancellationToken ct = default);
        Task<PagedResultDTO<LocationDTO>> GetLocationsPagedAsync(int page, int pageSize, string? q, CancellationToken ct = default);
    }
}
