using AdminPanel.Application.Common;
using AdminPanel.Application.DTOs.Social;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace AdminPanel.Application.Interfaces
{
    public interface ISocialService
    {
        Task<ApiMessage<bool>> CreateAsync(CreateSocialRequestDto request);

        Task<ApiMessage<List<SocialListItemDto>>> GetAllAsync();
        Task<ApiMessage<SocialDetailsDto>> GetByIdAsync(int id);
        Task<ApiMessage<bool>> UpdateAsync(int id, UpdateSocialRequestDto dto);
        Task<ApiMessage<bool>> DeleteAsync(int id);
    }
}
