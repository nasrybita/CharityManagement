using AdminPanel.Core.Enums;
using AdminPanel.Infrastructure.Persistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Core.Common
{
    public static class AdminUserRoleHelper
    {
        public static AdminUserType Resolve(AdminUser user)
        {
            if (user.IsRoot)
                return AdminUserType.AdminSystem;

            if (user.IsAdmin && user.CharityId.HasValue)
                return AdminUserType.CharityAdmin;

            return AdminUserType.CharityUser;
        }
    }
}
