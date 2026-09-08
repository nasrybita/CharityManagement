using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Application.DTOs.Social
{
    public class CharitySocialDetailsDto
    {
        public int Id { get; set; }

        public int SocialId { get; set; }

        public string SocialName { get; set; } = null!;

        public string? Abbreviation { get; set; }

        public string? IconUrl { get; set; }

        public string Value { get; set; } = null!;
    }
}
