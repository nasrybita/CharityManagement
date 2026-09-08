using AdminPanel.Application.Interfaces;
using AdminPanel.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace AdminPanel.Infrastructure.Repositories
{
    public class AdminUserRepository : IAdminUserRepository
    {
        private readonly AdminPanelDbContext _context;

        public AdminUserRepository(AdminPanelDbContext context)
        {
            _context = context;
        }




        public async Task<AdminUser?> GetByUserNameAsync(string userName)
        {
            return await _context.AdminUsers
                .FirstOrDefaultAsync(x =>
                    x.UserName == userName &&
                    x.IsDeleted == false);
        }



        public async Task<bool> IsUserNameExistAsync(string userName)
        {
            return await _context.AdminUsers
                .AnyAsync(x => x.UserName == userName);
        }



        public async Task AddAsync(AdminUser user)
        {
            await _context.AdminUsers.AddAsync(user);
        }




        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }




    }
}
