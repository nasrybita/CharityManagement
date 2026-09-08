using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AdminPanel.Application.DTOs.Category;
using AdminPanel.Application.Interfaces;
using AdminPanel.Infrastructure.Persistence.Data;

using Microsoft.EntityFrameworkCore;




namespace AdminPanel.Infrastructure.Repositories
{
    public class CharityRepository : ICharityRepository
    {
        private readonly AdminPanelDbContext _context;

        public CharityRepository(AdminPanelDbContext context)
        {
            _context = context;
        }





        public async Task<Charity?> GetByIdAsync(int id)
        {
            return await _context.Charities
                .Include(x => x.Logo)
                .Include(x => x.Banner)
                .Include(x => x.SocialCharities)
                    .ThenInclude(x => x.Social)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        }







        public async Task<List<CategoryListItemDto>> GetCategoriesByCharityIdAsync(int id)
        {
            return await _context.CharityCategories
                .Where(x => x.CharityId == id)
                .Select(x => new CategoryListItemDto
                {
                    Id = x.Category.Id,
                    Name = x.Category.Name
                })
                .ToListAsync();
        }


        public async Task<(List<Charity> items, int totalCount)> GetPagedAsync(int page, int pageSize)
        {
            var query = _context.Charities
                .Where(x => !x.IsDeleted);


            var totalCount = await query.CountAsync();


            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }



   




        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }




        public async Task AddAsync(Charity charity)
        {
            await _context.Charities.AddAsync(charity);
        }





        public async Task<bool> IsNameExistAsync(string name)
        {
            return await _context.Charities
                .AnyAsync(x => x.Name == name && !x.IsDeleted);
        }





        public async Task AddCharityCategoriesAsync(List<CharityCategory> charityCategories)
        {
            if (charityCategories == null || !charityCategories.Any())
            {
                return;
            }
                

            foreach (var item in charityCategories)
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "INSERT INTO CharityCategories (CharityId, CategoryId) VALUES ({0}, {1})",
                    item.CharityId,
                    item.CategoryId
                );
            }
        }




        public async Task RemoveSocialsByCharityIdAsync(int charityId)
        {
            var socials = await _context.SocialCharities
                .Where(x => x.CharityId == charityId)
                .ToListAsync();

            if (socials.Any())
            {
                _context.SocialCharities.RemoveRange(socials);
            }
        }



        public async Task AddSocialAsync(SocialCharity socialCharity)
        {
            await _context.SocialCharities.AddAsync(socialCharity);
        }




    }
}
