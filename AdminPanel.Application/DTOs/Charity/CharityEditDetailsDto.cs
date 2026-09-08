using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AdminPanel.Application.DTOs.Category;
using AdminPanel.Application.DTOs.Social;

namespace AdminPanel.Application.DTOs.Charity
{
    public class CharityEditDetailsDto
    {

        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string? Website { get; set; }
        public string? Description { get; set; }

        public string? Telephone { get; set; }
        public string? Address { get; set; }

        public string? ManagerName { get; set; }
        public string? ContactName { get; set; }
        public string? ContactPhone { get; set; }


        public string? LogoUrl { get; set; }
        public string? BannerUrl { get; set; }


        public List<CategoryListItemDto> Categories { get; set; } = new();

        //public List<CharitySocialItemDto> Socials { get; set; } = new();

        public List<CharitySocialDetailsDto> Socials { get; set; } = new();
    }
}

