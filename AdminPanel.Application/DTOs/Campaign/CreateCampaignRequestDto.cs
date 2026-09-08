using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace AdminPanel.Application.DTOs.Campaign
{
    public class CreateCampaignRequestDto
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public int CityId { get; set; }
        public int CharityId { get; set; }
        public int? BannerId { get; set; }

        // File Upload Section
        public IFormFile? LogoFile { get; set; }
        public IFormFile? BannerFile { get; set; }


        // دسته‌بندی‌های انتخاب شده برای کمپین
        public List<int> CategoryIds { get; set; } = new List<int>();
    }
}
