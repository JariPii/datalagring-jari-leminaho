using Microsoft.EntityFrameworkCore;
using SkillFlow.Domain.Interfaces;
using SkillFlow.Domain.Locations;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Infrastructure.Repositories
{
    internal class LocationRepository : ILocationRepository
    {
        private readonly SkillFlowDbContext _context;

        public LocationRepository(SkillFlowDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Location location)
        {
            await _context.Locations.AddAsync(location);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(LocationId id)
        {
            var location = await _context.Locations.FindAsync(id);
            if (location is null) return false;
            try
            {
                _context.Locations.Remove(location);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ExistsByIdAsync(LocationId id)
        {
            return await _context.Locations.AnyAsync(l => l.Id == id);
        }

        public async Task<bool> ExistsByNameAsync(LocationName name)
        {
            return await _context.Locations.AnyAsync(l => l.LocationName == name);
        }

        public async Task<IEnumerable<Location>> GetAllAsync()
        {
            return await _context.Locations.ToListAsync();
        }

        public async Task<Location?> GetByIdAsync(LocationId id) => await _context.Locations.FindAsync(id);

        public async Task<IEnumerable<Location>> SearchByNameAsync(string searchTerm)
        {
            var searchPattern = $"%{searchTerm}%";

            return await _context.Locations
                .FromSqlInterpolated($"SELECT * FROM Locations WHERE LocationName LIKE {searchPattern}")
                .ToListAsync();
        }

        public async Task UpdateAsync(Location location)
        {
            _context.Locations.Update(location);
            await _context.SaveChangesAsync();
        }
    }
}
