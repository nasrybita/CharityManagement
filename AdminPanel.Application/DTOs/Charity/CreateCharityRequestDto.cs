using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AdminPanel.Application.DTOs.Category;
using AdminPanel.Application.DTOs.Social;

using Microsoft.AspNetCore.Http;

namespace AdminPanel.Application.DTOs.Charity
{
    public class CreateCharityRequestDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Website { get; set; }
        public string? Address { get; set; }
        public string? Telephone { get; set; }
        public string? ManagerName { get; set; }
        public string? ContactName { get; set; }
        public string? ContactPhone { get; set; }


        // Category Section
        public List<int>? CategoryIds { get; set; }


        // File Upload Section
        public IFormFile? LogoFile { get; set; } 
        public IFormFile? BannerFile { get; set; } 


    

        // داده‌هایی که قبلاً برای این خیریه ثبت شده‌اند
        public List<CategoryListItemDto> Categories { get; set; } = new();
        public List<CharitySocialDetailsDto> Socials { get; set; } = new();
    }

    //public class CharitySocialItemDto
    //{
    //    public int SocialId { get; set; }
    //    public string SocialName { get; set; } = string.Empty;
    //    public string? IconUrl { get; set; }
    //    public string Value { get; set; } = string.Empty;



    //}
}
