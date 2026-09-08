using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AdminPanel.Application.Common;
using AdminPanel.Application.DTOs.Category;
using AdminPanel.Application.DTOs.Charity;
using AdminPanel.Application.DTOs.Common;
using AdminPanel.Infrastructure.Persistence.Data;



namespace AdminPanel.Application.Interfaces
{
    public interface ICharityRepository
    {
        Task AddAsync(Charity charity);

        Task<bool> IsNameExistAsync(string name);

        Task SaveChangesAsync();

        Task<Charity?> GetByIdAsync(int id);

        Task<List<CategoryListItemDto>> GetCategoriesByCharityIdAsync(int id);

        Task<(List<Charity> items, int totalCount)> GetPagedAsync(int page, int pageSize);

        Task AddCharityCategoriesAsync(List<CharityCategory> charityCategories);

        Task RemoveSocialsByCharityIdAsync(int charityId);

        Task AddSocialAsync(SocialCharity socialCharity);

    }
}
