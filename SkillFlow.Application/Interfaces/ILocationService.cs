using SkillFlow.Application.DTOs.Locations;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Application.Interfaces
{
    public interface ILocationService
    {
        Task<IEnumerable<LocationDTO>> GetAllLocationsAsync(CancellationToken ct = default);

        Task<LocationDTO> GetLocationByIdAsync(Guid id, CancellationToken ct = default);

        Task<LocationDTO> CreateLocationAsync(CreateLocationDTO dto, CancellationToken ct = default);

        Task<IEnumerable<LocationDTO>> SearchLocationsAsync(string searchTerm, CancellationToken ct = default);

        Task UpdateLocationAsync(UpdateLocationDTO dto, CancellationToken ct = default);

        Task DeleteLocationAsync(Guid id, CancellationToken ct = default);
    }
}
