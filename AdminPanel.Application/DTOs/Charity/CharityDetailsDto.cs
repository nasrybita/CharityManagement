using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AdminPanel.Application.DTOs.Category;
using AdminPanel.Application.DTOs.Social;

namespace AdminPanel.Application.DTOs.Charity
{
    public class CharityDetailsDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }


        // Contact
        public string? Website { get; set; }

        public string? Address { get; set; }

        public string? Telephone { get; set; }

        public string? ManagerName { get; set; }

        public string? ContactName { get; set; }

        public string? ContactPhone { get; set; }


        // Files

        public string? LogoUrl { get; set; }

        public string? BannerUrl { get; set; }



        // Categories

        public List<CategoryListItemDto> Categories { get; set; } = new();



        // Social Media

        public List<CharitySocialDetailsDto> Socials { get; set; } = new();



        public DateTime CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }
    }
}
