//using AdminPanel.Infrastructure.Persistence.Models;
using AdminPanel.Infrastructure.Persistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Application.Interfaces
{
    public interface IAdminUserRepository
    {
        Task<AdminUser?> GetByUserNameAsync(string userName);


        Task<bool> IsUserNameExistAsync(string userName);


        Task AddAsync(AdminUser user);


        Task SaveChangesAsync();
    }
}
