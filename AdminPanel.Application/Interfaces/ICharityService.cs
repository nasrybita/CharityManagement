using AdminPanel.Application.Common;
using AdminPanel.Application.DTOs.Charity;
using AdminPanel.Application.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace AdminPanel.Application.Interfaces
{
    public interface ICharityService
    {
        Task<ApiMessage<bool>> DeleteAsync(int id);

        Task<ApiMessage<CharityDetailsDto>> GetByIdAsync(int id);

        Task<ApiMessage<PagedResultDto<CharityListItemDto>>> GetListAsync(PaginationRequestDto request);


        Task<ApiMessage<bool>> CreateAsync(CreateCharityRequestDto request, int createdByUserId);

        Task<ApiMessage<bool>> UpdateAsync(UpdateCharityRequestDto request);

       
    }
}
