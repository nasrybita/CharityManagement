using System;
using System.Collections.Generic;

//namespace AdminPanel.Infrastructure.Persistence.Models
namespace AdminPanel.Infrastructure.Persistence.Data;

public partial class CampaignStatus
{
    public int Status { get; set; }

    public string Name { get; set; } = null!;
}
