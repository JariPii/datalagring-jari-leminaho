using SkillFlow.Domain.Entities.Locations;

namespace SkillFlow.Domain.Interfaces
{
    public interface ILocationRepository : IBaseRepository<Location, LocationId>
    {
        Task<Location?> GetByLocationNameAsync(LocationName name, CancellationToken ct);
        Task<bool> ExistsByIdAsync(LocationId id, CancellationToken ct = default);
        Task<bool> ExistsByNameAsync(LocationName name, CancellationToken ct = default);

        Task<IEnumerable<Location>> SearchByNameAsync(string searchTerm, CancellationToken ct = default);

        void Remove(Location location);

    }
}
