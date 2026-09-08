using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Application.DTOs.Charity
{
    public class CharityListItemDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Telephone { get; set; }

        public string? Website { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
