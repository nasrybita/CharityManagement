using AdminPanel.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Application.DTOs.Auth
{
    public class LoginResponseDto
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public AdminUserType UserType { get; set; }


        // I use this field for dto so that we know which campaign to show to user (user should see campaigns related to a specific charity)
        public int? CharityId { get; set; }


        //I commented these three fields for now, if needed you can uncomment them later
        //public bool IsAdmin { get; set; }
        //public bool IsRoot { get; set; }
        //public bool Sex { get; set; }

    }
}
