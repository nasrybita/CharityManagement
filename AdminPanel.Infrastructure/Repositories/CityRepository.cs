using AdminPanel.Application.DTOs.City;
using AdminPanel.Application.Interfaces;
using AdminPanel.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Infrastructure.Repositories
{
    public class CityRepository : ICityRepository
    {


        private readonly AdminPanelDbContext _context;

        public CityRepository(AdminPanelDbContext context)
        {
            _context = context;
        }


        public async Task<List<CityLookupDto>> GetCitiesAsync()
        {
            return await _context.Cities
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new CityLookupDto
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();
        }







        public async Task<int> ImportCitiesFromExcelAsync(IEnumerable<City> cities)
        {
            var validCities = cities
        .Where(c => !string.IsNullOrWhiteSpace(c.Name))
        .ToList();

            if (validCities.Count == 0)
            {
                return 0;
            }

            await _context.Cities.AddRangeAsync(validCities);

            return await _context.SaveChangesAsync();
        }






    }
}
