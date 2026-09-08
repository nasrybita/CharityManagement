using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Application.DTOs.Users
{
    public class UpdateUserByCharityAdminDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string Mobile { get; set; } = string.Empty;

        public bool Sex { get; set; }
    }
}
