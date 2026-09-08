using AdminPanel.Application.Common;
using AdminPanel.Application.DTOs.Category;
using AdminPanel.Application.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<ApiMessage<bool>> CreateAsync(CreateCategoryRequestDto request);


        Task<ApiMessage<bool>> UpdateAsync(UpdateCategoryRequestDto request);


        Task<ApiMessage<PagedResultDto<CategoryListItemDto>>> GetListAsync(PaginationRequestDto request);



        Task<ApiMessage<bool>> DeleteAsync(int id);



        Task<ApiMessage<CategoryDetailsDto>> GetByIdAsync(int id);

        

        Task<ApiMessage<List<CategoryListItemDto>>> GetByCharityAsync(int charityId);


    }
}
