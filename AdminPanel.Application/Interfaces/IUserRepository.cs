using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdminPanel.Application.DTOs.User;
using AdminPanel.Application.DTOs.Users;
using AdminPanel.Infrastructure.Persistence.Data;
using AdminPanel.Core.Enums;

namespace AdminPanel.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<List<UserDto>> GetAllAsync();

        Task<AdminUserDetailsDto?> GetByIdAsync(int id);

        Task<AdminUser?> GetUserEntityAsync(int id);

        Task<AdminUser?> GetUserWithCharityAsync(int id);

        Task<Charity?> GetCharityAsync(int id);

        Task<UserDto?> GetUserByIdAsync(int id);

        Task<List<UserDto>> GetUsersByCharityIdAsync(int charityId);


        Task UpdateUserAsync(AdminUser user);

        Task UpdateCharityAsync(Charity charity);

        Task SaveChangesAsync();

        Task DeleteAsync(AdminUser user);
    }
}
