using Microsoft.EntityFrameworkCore;
using SkillFlow.Domain.Entities.Locations;
using SkillFlow.Domain.Interfaces;

namespace SkillFlow.Infrastructure.Repositories
{
    public class LocationRepository(SkillFlowDbContext context) : BaseRespository<Location, LocationId>(context), ILocationRepository
    {

        public override async Task<bool> DeleteAsync(LocationId id, CancellationToken ct)
        {
            var location = await _context.Locations.FirstOrDefaultAsync(l => l.Id == id, ct);

            if (location is null) return false;
            try
            {
                _context.Locations.Remove(location);
                await _context.SaveChangesAsync(ct);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ExistsByNameAsync(LocationName name, CancellationToken ct)
        {
            return await _context.Locations.AnyAsync(l => l.LocationName == name, ct);
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
