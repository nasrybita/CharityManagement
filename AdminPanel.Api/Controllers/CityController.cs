using AdminPanel.Application.Interfaces;
using AdminPanel.Infrastructure.Persistence.Data;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanel.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CityController : ControllerBase
    {
        private readonly ICityService _cityService;

        public CityController(ICityService cityService)
        {
            _cityService = cityService;
        }



        [HttpGet]
        public async Task<IActionResult> GetCities()
        {
            var cities = await _cityService.GetCitiesAsync();
            return Ok(new
            {
                hasError = false,
                value = cities
            });
        }









        [HttpPost("import-excel")]
        [AllowAnonymous]
        public async Task<IActionResult> ImportCitiesFromExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { hasError = true, message = "لطفا فایل اکسل را انتخاب کنید." });
            }

            if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { hasError = true, message = "فرمت فایل باید xlsx باشد." });
            }

            var cities = new List<City>();

            // Read file with ClosedXML
            using (var stream = file.OpenReadStream())
            {
                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet(1); // Sheet one

                    //Row one is title, so we'll begin from row two
                    var rows = worksheet.RowsUsed()
                        .Where(row => !row.IsEmpty())
                        .Skip(1);

                    foreach (var row in rows)
                    {
                        var cityName = row.Cell(4).GetString().Trim();      //"City" column
                        var latinName = row.Cell(5).GetString().Trim();     //"LatinName" column
                        var cityCode = row.Cell(6).GetString().Trim();      //"cityCode" column

                        if (string.IsNullOrWhiteSpace(cityName))
                            continue;

                        cities.Add(new City
                        {
                            Name = cityName,
                            Abbreviation = latinName,
                            CityId = cityCode
                        });
                    }
                }
            }

            var insertedCount = await _cityService.ImportCitiesFromExcelAsync(cities);

            return Ok(new
            {
                hasError = false,
                message = $"{insertedCount} شهر با موفقیت وارد دیتابیس شد.",
                totalRows = cities.Count
            });
        }








    }
}
