using AdminPanel.Infrastructure.Persistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Application.Interfaces
{
    public interface ICategoryRepository
    {

        Task<bool> IsNameExistAsync(string name);

        Task AddAsync(Category category);

        Task<Category?> GetByIdAsync(int id);

        Task SaveChangesAsync();

        // اضافه کردن پارامتر اختیاری charityId به انتهای متد
        Task<(List<Category> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, long? charityId = null);


        //Task<(List<Category> items, int totalCount)> GetPagedAsync(int page, int pageSize);


        //To recieve categories based on list of IDs
        Task<List<Category>> GetByIdsAsync(List<int> categoryIds);

        // دسته بندی های مجاز یک خیریه
        Task<List<Category>> GetByCharityAsync(int charityId);
    }
}
