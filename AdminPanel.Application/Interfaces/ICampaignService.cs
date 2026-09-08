using AdminPanel.Application.Common;
using AdminPanel.Application.DTOs.Campaign;
using AdminPanel.Application.DTOs.Common;
using AdminPanel.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Application.Interfaces
{
    public interface ICampaignService
    {
        Task<ApiMessage<bool>> CreateAsync(CreateCampaignRequestDto request);
        Task<ApiMessage<bool>> UpdateAsync(UpdateCampaignRequestDto request, AdminUserType userRole);
        Task<ApiMessage<bool>> DeleteAsync(int id, AdminUserType userRole);
        Task<ApiMessage<CampaignDetailsDto>> GetByIdAsync(int id);


        // Retrieve the list of campaigns with filters
        Task<ApiMessage<PagedResultDto<CampaignListItemDto>>> GetListAsync(
                         PaginationRequestDto request,
                         int? charityId,
                         int? categoryId,
                         int? campaignStatus);


        Task<ApiMessage<bool>> ChangeStatusAsync(int id, CampaignStatus newStatus, AdminUserType userRole);

    }
}
