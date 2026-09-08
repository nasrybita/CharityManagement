using AdminPanel.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Application.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(int userId, string userName, AdminUserType userType, int? charityId);

    }
}
