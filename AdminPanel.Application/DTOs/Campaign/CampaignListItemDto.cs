using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Application.DTOs.Campaign
{
    public class CampaignListItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? ChargedAmount { get; set; }
        public string CharityName { get; set; } = null!;
        public int CampaignStatus { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
