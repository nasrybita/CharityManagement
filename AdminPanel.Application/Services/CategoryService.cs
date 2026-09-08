using AdminPanel.Application.Common;
using AdminPanel.Application.DTOs.Category;
using AdminPanel.Application.DTOs.Common;
using AdminPanel.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Microsoft.AspNetCore.Http;
using AdminPanel.Infrastructure.Persistence.Data;

namespace AdminPanel.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<ApiMessage<bool>> CreateAsync(CreateCategoryRequestDto request)
        {
            var result = new ApiMessage<bool>();

            if (await _categoryRepository.IsNameExistAsync(request.Name))
            {
                result.HasError = true;
                result.ErrorMessage = "عنوان دسته بندی قبلا ثبت شده است";
                return result;
            }

            var category = new Category
            {
                Name = request.Name,
                CreatedAt = DateTime.Now,
                ModifiedAt = DateTime.Now,
                IsDeleted = false
            };

            await _categoryRepository.AddAsync(category);
            await _categoryRepository.SaveChangesAsync();

            result.Value = true;
            return result;
        }

        public async Task<ApiMessage<bool>> UpdateAsync(UpdateCategoryRequestDto request)
        {
            var result = new ApiMessage<bool>();

            var category = await _categoryRepository.GetByIdAsync(request.Id);

            if (category == null)
            {
                result.HasError = true;
                result.ErrorMessage = "Category not found";
                return result;
            }

            if (await _categoryRepository.IsNameExistAsync(request.Name) &&
                category.Name != request.Name)
            {
                result.HasError = true;
                result.ErrorMessage = "Category name already exists";
                return result;
            }

            category.Name = request.Name;
            category.ModifiedAt = DateTime.Now;

            await _categoryRepository.SaveChangesAsync();

            result.Value = true;
            return result;
        }

        public async Task<ApiMessage<PagedResultDto<CategoryListItemDto>>> GetListAsync(PaginationRequestDto request)
        {
            var result = new ApiMessage<PagedResultDto<CategoryListItemDto>>();


            // Handle non-positive values by assigning a default
            int page = request.Page <= 0 ? 1 : request.Page;
            int pageSize = request.PageSize <= 0 ? 50 : request.PageSize;


            var (items, totalCount) = await _categoryRepository.GetPagedAsync(
                page,
                pageSize,
                null);


            var list = items.Select(x => new CategoryListItemDto
            {
                Id = x.Id,
                Name = x.Name,
                CreatedAt = x.CreatedAt
            }).ToList();


            result.Value = new PagedResultDto<CategoryListItemDto>
            {
                Items = list,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };


            return result;
        }



        public async Task<ApiMessage<bool>> DeleteAsync(int id)
        {
            var result = new ApiMessage<bool>();

            var category = await _categoryRepository.GetByIdAsync(id);

            if (category == null)
            {
                result.HasError = true;
                result.ErrorMessage = "Category not found";
                return result;
            }

            category.IsDeleted = true;
            category.ModifiedAt = DateTime.Now;

            await _categoryRepository.SaveChangesAsync();

            result.Value = true;
            return result;
        }



        public async Task<ApiMessage<CategoryDetailsDto>> GetByIdAsync(int id)
        {
            var result = new ApiMessage<CategoryDetailsDto>();

            var category = await _categoryRepository.GetByIdAsync(id);

            if (category == null)
            {
                result.HasError = true;
                result.ErrorMessage = "Category not found";
                return result;
            }

            result.Value = new CategoryDetailsDto
            {
                Id = category.Id,
                Name = category.Name,
                CreatedAt = category.CreatedAt,
                ModifiedAt = category.ModifiedAt
            };

            return result;
        }




        public async Task<ApiMessage<List<CategoryListItemDto>>> GetByCharityAsync(int charityId)
        {
            var result = new ApiMessage<List<CategoryListItemDto>>();

            var categories = await _categoryRepository.GetByCharityAsync(charityId);

            result.Value = categories.Select(x => new CategoryListItemDto
            {
                Id = x.Id,
                Name = x.Name,
                CreatedAt = x.CreatedAt
            }).ToList();

            return result;
        }




    }
}
