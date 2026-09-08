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
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AdminPanelDbContext _context;

        public CategoryRepository(AdminPanelDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsNameExistAsync(string name)
        {
            return await _context.Categories
                .AnyAsync(x => x.Name == name && !x.IsDeleted);
        }

        public async Task AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        }







        public async Task<(List<Category> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, long? charityId = null)
        {
            IQueryable<Category> query = _context.Categories.Where(x => !x.IsDeleted);

            Console.WriteLine($"Repository charityId = {charityId}");

            if (charityId.HasValue)
            {
                var ids = await _context.CharityCategories
                    .Where(cc => cc.CharityId == charityId.Value)
                    .Select(cc => cc.CategoryId)
                    .ToListAsync();

                Console.WriteLine("CategoryIds:");
                foreach (var id in ids)
                    Console.WriteLine(id);

                query = query.Where(c => ids.Contains(c.Id));
            }

            Console.WriteLine(query.ToQueryString());

            int totalCount = await query.CountAsync();

            Console.WriteLine($"TotalCount = {totalCount}");

            var items = await query.ToListAsync();

            return (items, totalCount);
        }
        //public async Task<(List<Category> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, long? charityId = null)
        //{
        //    //تست 
        //    Console.WriteLine($"Repository CharityId = {charityId}");
        //    //تست
        //    IQueryable<Category> query = _context.Categories.Where(x => !x.IsDeleted);

        //   // فیلتر خیریه در صورت وجود
        //    if (charityId.HasValue)
        //    {
        //        var categoryIds = _context.CharityCategories
        //            .Where(cc => cc.CharityId == charityId.Value)
        //            .Select(cc => cc.CategoryId);

        //        query = query.Where(c => categoryIds.Contains(c.Id));
        //    }


        //            //تست 
        //            if (charityId.HasValue)
        //            {
        //                // ۱. بررسی کن آیا اصلا رابطه‌ای در جدول واسط وجود دارد؟
        //                var count = await _context.CharityCategories.CountAsync(cc => cc.CharityId == charityId.Value);
        //                System.Diagnostics.Debug.WriteLine($"DEBUG: Found {count} records in CharityCategories for CharityId {charityId.Value}");

        //                var categoryIds = _context.CharityCategories
        //                    .Where(cc => cc.CharityId == charityId.Value)
        //                    .Select(cc => cc.CategoryId);

        //                query = query.Where(c => categoryIds.Contains(c.Id));
        //            }
        ////تست

        //int totalCount = await query.CountAsync();

        //    var items = await query
        //        .OrderByDescending(x => x.CreatedAt)
        //        .Skip((page - 1) * pageSize)
        //        .Take(pageSize)
        //        .ToListAsync();

        //    return (items, totalCount);
        //}

        public async Task<List<Category>> GetByIdsAsync(List<int> categoryIds)
        {
            return await _context.Categories
                .Where(c => categoryIds.Contains(c.Id) && !c.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<Category>> GetByCharityAsync(int charityId)
        {
            return await _context.CharityCategories
                .Where(x => x.CharityId == charityId && !x.Category.IsDeleted)
                .Select(x => x.Category)
                .ToListAsync();
        }
    }
}
