using AdminPanel.Application.DTOs.City;
using AdminPanel.Application.Interfaces;
using AdminPanel.Infrastructure.Persistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AdminPanel.Application.Services
{
    public class CityService : ICityService
    {

        private readonly ICityRepository _cityRepository;

        public CityService(ICityRepository cityRepository)
        {
            _cityRepository = cityRepository;
        }




        public async Task<List<CityLookupDto>> GetCitiesAsync()
        {
            return await _cityRepository.GetCitiesAsync();
        }




        public async Task<int> ImportCitiesFromExcelAsync(IEnumerable<City> cities)
        {
            return await _cityRepository.ImportCitiesFromExcelAsync(cities);
        }


    }
}
