using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AdminPanel.Application.Interfaces;
using AdminPanel.Infrastructure.Persistence.Data;

using Microsoft.EntityFrameworkCore;

namespace AdminPanel.Infrastructure.Repositories
{
    public class CharityCategoryRepository : ICharityCategoryRepository
    {
        private readonly AdminPanelDbContext _context;

        public CharityCategoryRepository(AdminPanelDbContext context)
        {
            _context = context;
        }


        public async Task<List<CharityCategory>> GetByCharityIdAsync(int charityId)
        {
            return await _context.CharityCategories
                .Where(x => x.CharityId == charityId)
                .ToListAsync();
        }


        public async Task RemoveRangeAsync(List<CharityCategory> items)
        {
            _context.CharityCategories.RemoveRange(items);
            await _context.SaveChangesAsync();
        }


        public async Task AddAsync(CharityCategory entity)
        {
            await _context.CharityCategories.AddAsync(entity);
            await _context.SaveChangesAsync();
        }
    }

}
