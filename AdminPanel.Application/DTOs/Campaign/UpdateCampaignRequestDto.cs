using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Application.DTOs.Campaign
{
    public class UpdateCampaignRequestDto : CreateCampaignRequestDto
    {
        public int Id { get; set; }
        public int CampaignStatus { get; set; }


        //I added this so that in edit campaign page we can romove current image
        public bool RemoveBanner { get; set; }

    }
}
