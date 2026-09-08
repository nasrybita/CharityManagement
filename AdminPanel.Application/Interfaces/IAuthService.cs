using AdminPanel.Application.Common;
using AdminPanel.Application.DTOs.Auth;
using AdminPanel.Core.Enums;

namespace AdminPanel.Application.Interfaces
{
    public interface IAuthService
    {
        Task<ApiMessage<LoginResponseDto>> LoginAsync(LoginRequestDto request);

        //We have added creatorUserType and creatorCharityId(so we know information of the user who is trying to add a new user)
        Task<ApiMessage<bool>> RegisterAsync(
            RegisterRequestDto request, 
            AdminUserType creatorUserType,
            int? creatorCharityId);
    }
}
