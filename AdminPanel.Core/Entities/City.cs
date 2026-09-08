using System;
using System.Collections.Generic;

//namespace AdminPanel.Infrastructure.Persistence.Models;
namespace AdminPanel.Infrastructure.Persistence.Data;
public partial class City
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Abbreviation { get; set; } = null!;

    public string CityId { get; set; } = null!;

    public virtual ICollection<Campaign> Campaigns { get; set; } = new List<Campaign>();
}
