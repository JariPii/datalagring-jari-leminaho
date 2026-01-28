using SkillFlow.Domain.Locations;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Interfaces
{
    public interface ILocationRepository
    {
        Task<Location?> GetByIdAsync(LocationId id, CancellationToken ct = default);

        Task<bool> ExistsByIdAsync(LocationId id, CancellationToken ct = default);
        Task<bool> ExistsByNameAsync(LocationName name, CancellationToken ct = default);

        Task<IEnumerable<Location>> GetAllAsync(CancellationToken ct = default);
        Task<IEnumerable<Location>> SearchByNameAsync(string searchTerm, CancellationToken ct = default);

        Task AddAsync(Location location, CancellationToken ct = default);
        Task UpdateAsync(Location location, CancellationToken ct = default);
        Task<bool> DeleteAsync(LocationId id, CancellationToken ct = default);
    }
}
