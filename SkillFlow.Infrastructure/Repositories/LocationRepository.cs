using Microsoft.EntityFrameworkCore;
using SkillFlow.Domain.Entities.Locations;
using SkillFlow.Domain.Interfaces;

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
