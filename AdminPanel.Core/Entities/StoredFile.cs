using System;
using System.Collections.Generic;

//namespace AdminPanel.Infrastructure.Persistence.Models;
namespace AdminPanel.Infrastructure.Persistence.Data;
public partial class StoredFile
{
    public int Id { get; set; }

    public string FileName { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public string FileType { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<Campaign> Campaigns { get; set; } = new List<Campaign>();

    public virtual ICollection<Charity> CharityBanners { get; set; } = new List<Charity>();

    public virtual ICollection<Charity> CharityLogos { get; set; } = new List<Charity>();
}
