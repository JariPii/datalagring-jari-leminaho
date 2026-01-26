using SkillFlow.Application.DTOs.Locations;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Application.Interfaces
{
    public interface ILocationService
    {
        Task<IEnumerable<LocationDTO>> GetAllLocationtsAsync();

        Task<LocationDTO> GetLocationByIdAsync(Guid id);

        Task<LocationDTO> CreateLocationAsync(CreateLocationDTO dto);

        Task<IEnumerable<LocationDTO>> SearchLocationsAsync(string searchTerm);

        Task UpdateLocationAsync(UpdateLocationDTO dto);

        Task DeleteLocationAsync(Guid id);
    }
}
