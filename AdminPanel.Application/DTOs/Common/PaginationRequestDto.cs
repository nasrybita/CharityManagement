using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Application.DTOs.Common
{
    public class PaginationRequestDto
    {
        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;

    }
}
