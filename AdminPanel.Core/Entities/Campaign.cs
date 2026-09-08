using System;
using System.Collections.Generic;

//namespace AdminPanel.Infrastructure.Persistence.Models;
namespace AdminPanel.Infrastructure.Persistence.Data;


public partial class Campaign
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public decimal? TotalAmount { get; set; }

    public decimal? ChargedAmount { get; set; }

    public int? BannerId { get; set; }

    public int CityId { get; set; }

    public int CharityId { get; set; }

    public int CampaignStatus { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual StoredFile? Banner { get; set; }

    public virtual Charity Charity { get; set; } = null!;

    public virtual City City { get; set; } = null!;

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
}
