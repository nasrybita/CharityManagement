using AdminPanel.Application.DTOs.City;
using AdminPanel.Infrastructure.Persistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Application.Interfaces
{
    public interface ICityRepository
    {
        Task<List<CityLookupDto>> GetCitiesAsync();


        //For automatic import of data from cities excell in to database
        Task<int> ImportCitiesFromExcelAsync(IEnumerable<City> cities);

    }
}
