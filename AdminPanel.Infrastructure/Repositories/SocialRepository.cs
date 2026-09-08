using AdminPanel.Application.Interfaces;
using AdminPanel.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;






namespace AdminPanel.Infrastructure.Repositories
{
    public class SocialRepository : ISocialRepository
    {

        private readonly AdminPanelDbContext _context;

        public SocialRepository(AdminPanelDbContext context)
        {
            _context = context;
        }

        public async Task DeleteAsync(Social social)
        {
            social.IsDeleted = true;
            social.ModifiedAt = DateTime.UtcNow;

            _context.Socials.Update(social);
            await _context.SaveChangesAsync();
        }






        public async Task<bool> IsNameExistAsync(string name)
        {
            return await _context.Socials
                .AnyAsync(x => x.Name == name && !x.IsDeleted);
        }



        public async Task<Social?> GetByIdAsync(int id)
        {
            return await _context.Socials
                 .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        }




        public async Task AddAsync(Social social)
        {
            await _context.Socials.AddAsync(social);
        }




        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }



        public async Task UpdateAsync(Social social)
        {
            _context.Socials.Update(social);
            await _context.SaveChangesAsync();
        }


        public async Task<List<Social>> GetAllAsync()
        {
            return await _context.Socials
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }


    }
}
