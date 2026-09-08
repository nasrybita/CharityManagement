using System;
using System.Collections.Generic;

//namespace AdminPanel.Infrastructure.Persistence.Models;
namespace AdminPanel.Infrastructure.Persistence.Data;


public partial class AdminUser
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Mobile { get; set; } = null!;

    public int? CharityId { get; set; }

    public bool UserType { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public bool IsAdmin { get; set; }

    public bool IsRoot { get; set; }

    public bool Sex { get; set; }
}
