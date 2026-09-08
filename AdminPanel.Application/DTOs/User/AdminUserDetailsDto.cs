using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AdminPanel.Core.Enums;

namespace AdminPanel.Application.DTOs.Users
{
    public class AdminUserDetailsDto
    {

        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;


        public string UserName { get; set; } = string.Empty;


        public string Mobile { get; set; } = string.Empty;



        public int? CharityId { get; set; }

        public string? CharityName { get; set; }

        public AdminUserType UserType { get; set; }


        public string UserTypeTitle { get; set; } = string.Empty;



        public bool IsRoot { get; set; }

        public bool IsAdmin { get; set; }


        public bool Sex { get; set; }


        public DateTime CreatedAt { get; set; }
    }
}
