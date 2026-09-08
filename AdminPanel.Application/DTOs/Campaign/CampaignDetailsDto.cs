using AdminPanel.Application.DTOs.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Application.DTOs.Campaign
{
    public class CampaignDetailsDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? ChargedAmount { get; set; }
        public int CityId { get; set; }
        public string? CityName { get; set; }
        public int CharityId { get; set; }
        public string CharityName { get; set; } = null!;
        public int CampaignStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public int? BannerId { get; set; }
        public string? BannerUrl { get; set; }

        // لیست دسته‌بندی‌های کمپین
        public List<CategoryListItemDto> Categories { get; set; } = new List<CategoryListItemDto>();
    }
}
