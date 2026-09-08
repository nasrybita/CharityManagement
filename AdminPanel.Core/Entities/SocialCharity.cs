using System;
using System.Collections.Generic;

//namespace AdminPanel.Infrastructure.Persistence.Models;
namespace AdminPanel.Infrastructure.Persistence.Data;
public partial class SocialCharity
{
    public int Id { get; set; }

    public int SocialId { get; set; }

    public int CharityId { get; set; }

    public string Value { get; set; } = null!;

    public virtual Charity Charity { get; set; } = null!;

    public virtual Social Social { get; set; } = null!;
}
