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
    public class CampaignRepository : ICampaignRepository
    {

        private readonly AdminPanelDbContext _context;

        public CampaignRepository(AdminPanelDbContext context)
        {
            _context = context;
        }




        public async Task AddAsync(Campaign campaign)
        {
            await _context.Campaigns.AddAsync(campaign);
        }



        public async Task<Campaign?> GetByIdAsync(int id)
        {
            return await _context.Campaigns
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        }


        //Finds a campaign with a specific ID only if it hasn't been deleted before and it returns categories and charity related to that campaign
        public async Task<Campaign?> GetByIdWithCategoriesAsync(int id)
        {
            return await _context.Campaigns
                .Include(x => x.Categories)
                .Include(x => x.Charity)
                .Include(x => x.City)
                .Include(x => x.Banner)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        }



        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }



        public async Task<bool> IsTitleExistAsync(string title)
        {
            return await _context.Campaigns
                .AnyAsync(x => x.Title == title && !x.IsDeleted);
        }



        public async Task<(List<Campaign> items, int totalCount)> GetFilteredPagedAsync(
            int? charityId,
            int? categoryId,
            int? campaignStatus,
            int page,
            int pageSize)
        {
            var query = _context.Campaigns
                .Include(c => c.Charity)
                .Include(c => c.Categories)
                .Where(x => !x.IsDeleted);


            // Charity filter
            if (charityId.HasValue)
            {
                query = query.Where(x => x.CharityId == charityId.Value);
            }


            //Category filter
            if (categoryId.HasValue)
            {
                query = query.Where(x => x.Categories.Any(c => c.Id == categoryId.Value));
            }


            // campaignStatus  filter
            if (campaignStatus.HasValue)
            {
                query = query.Where(x => x.CampaignStatus == campaignStatus.Value);
            }


            var totalCount = await query.CountAsync();


            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();


            return (items, totalCount);
        }



        public async Task<bool> AreCategoriesValidForCharityAsync(int charityId, List<int> categoryIds)
        {

            if (categoryIds == null || !categoryIds.Any())
            {
                return true;
            }


            // Retrieve allowed charity categories from the CharityCategories join table
            var allowedCategoryIds = await _context.CharityCategories
                .Where(cc => cc.CharityId == charityId)
                .Select(cc => cc.CategoryId)
                .ToListAsync();


            // Ensure all requested categories are within the charity's allowed categories list
            return categoryIds.All(id => allowedCategoryIds.Contains(id));
        }





    }
}
