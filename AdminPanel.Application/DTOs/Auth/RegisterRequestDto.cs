using AdminPanel.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Application.DTOs.Auth
{
    public class RegisterRequestDto
    {
        public string Name { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Mobile { get; set; } = string.Empty;

        public AdminUserType UserType { get; set; }


        //We should have this field so that when user logs in we know which charity Id belongs to the user and which campaigns to show to user
        public int? CharityId { get; set; }
    }
}
