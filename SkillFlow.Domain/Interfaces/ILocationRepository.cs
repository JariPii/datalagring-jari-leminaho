using SkillFlow.Domain.Locations;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Interfaces
{
    public interface ILocationRepository
    {
        Task<Location?> GetByIdAsync(LocationId id);

        Task<bool> ExistsByIdAsync(LocationId id);
        Task<bool> ExistsByNameAsync(LocationName name);

        Task<IEnumerable<Location>> GetAllAsync();
        Task<IEnumerable<Location>> SearchByNameAsync(string searchTerm);

        Task AddAsync(Location location);
        Task UpdateAsync(Location location);
        Task<bool> DeleteAsync(LocationId id);
    }
}
