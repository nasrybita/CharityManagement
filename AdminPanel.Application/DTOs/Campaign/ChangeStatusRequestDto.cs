using AdminPanel.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Application.DTOs.Campaign
{
    public class ChangeStatusRequestDto
    {
        public CampaignStatus Status { get; set; }

    }
}
