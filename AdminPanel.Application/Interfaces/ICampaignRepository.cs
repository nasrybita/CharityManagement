using AdminPanel.Infrastructure.Persistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Application.Interfaces
{
    public interface ICampaignRepository
    {
        Task AddAsync(Campaign campaign);
        Task<Campaign?> GetByIdAsync(int id);
        Task<Campaign?> GetByIdWithCategoriesAsync(int id);
        Task SaveChangesAsync();
        Task<bool> IsTitleExistAsync(string title);

 
        //Method for recieving filtered campaigns
        Task<(List<Campaign> items, int totalCount)> GetFilteredPagedAsync(
            int? charityId,
            int? categoryId,
            int? campaignStatus,
            int page, 
            int pageSize);

        
        //Helper method to check if categories are related to the specific charity or not
        Task<bool> AreCategoriesValidForCharityAsync(int charityId, List<int> categoryIds);

    }
}
