using Microsoft.EntityFrameworkCore;
using SkillFlow.Domain.Entities.Locations;
using SkillFlow.Domain.Interfaces;
using SkillFlow.Domain.Primitives;
using System.Linq.Expressions;

namespace SkillFlow.Infrastructure.Repositories
{
    public class LocationRepository(SkillFlowDbContext context) : BaseRespository<Location, LocationId>(context), ILocationRepository
    {
        public async Task<bool> ExistsByNameAsync(LocationName name, CancellationToken ct)
        {
            return await _context.Locations.AnyAsync(l => l.LocationName == name, ct);
        }

        public async Task<Location?> GetByLocationNameAsync(LocationName name, CancellationToken ct)
        {
            return await _context.Locations.FirstOrDefaultAsync(l => l.LocationName == name, ct);
        }

        public async Task<PagedResult<Location>> GetLocationsPagedAsync(int page, int pageSize, string? q, CancellationToken ct = default)
        {
            Expression<Func<Location, bool>>? filter = null;

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim();

                filter = l => EF.Functions.Like(l.LocationName.Value, $"%{term}%");
            }

            return await GetPagedAsync(page, pageSize, filter, ct: ct);
        }

        public void Remove(Location location)
        {
            _context.Locations.Remove(location);
        }

        public async Task<IEnumerable<Location>> SearchByNameAsync(string searchTerm, CancellationToken ct)
        {
            var searchPattern = $"%{searchTerm}%";

            return await _context.Locations
                .FromSqlInterpolated($"SELECT * FROM Locations WHERE LocationName LIKE {searchPattern}")
                .ToListAsync(ct);
        }
    }
}
