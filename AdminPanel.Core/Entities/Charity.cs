using System;
using System.Collections.Generic;

//namespace AdminPanel.Infrastructure.Persistence.Models;
namespace AdminPanel.Infrastructure.Persistence.Data;
public partial class Charity
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? Website { get; set; }

    public int CreatedByUserId { get; set; }

    public string? Address { get; set; }

    public int? CityId { get; set; }

    public string? Telephone { get; set; }

    public string? ManagerName { get; set; }

    public int? LogoId { get; set; }

    public int? BannerId { get; set; }

    public string? ContactName { get; set; }

    public string? ContactPhone { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual StoredFile? Banner { get; set; }

    public virtual ICollection<Campaign> Campaigns { get; set; } = new List<Campaign>();

    public virtual StoredFile? Logo { get; set; }

    public virtual ICollection<SocialCharity> SocialCharities { get; set; } = new List<SocialCharity>();
}
