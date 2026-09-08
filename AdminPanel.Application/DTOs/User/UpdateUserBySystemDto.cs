using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AdminPanel.Core.Enums;

namespace AdminPanel.Application.DTOs.Users
{
    public class UpdateUserBySystemDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string Mobile { get; set; } = string.Empty;

        public bool Sex { get; set; }

        public AdminUserType UserType { get; set; }

        // اطلاعات خیریه
        public int? CharityId { get; set; }

        public string? CharityName { get; set; }
    }
}
