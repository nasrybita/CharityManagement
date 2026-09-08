using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Application.DTOs.City
{
    //I created this DTO for displaying list of cities in create campaign page
    public class CityLookupDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
