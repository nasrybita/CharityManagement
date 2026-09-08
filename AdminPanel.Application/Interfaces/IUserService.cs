using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdminPanel.Application.Common;
using AdminPanel.Application.DTOs.Users;
using AdminPanel.Application.DTOs.User;
using AdminPanel.Core.Enums;

namespace AdminPanel.Application.Interfaces
{
    public interface IUserService
    {
        //Task<ApiMessage<List<UserDto>>> GetAllAsync();

        Task<ApiMessage<List<UserDto>>> GetAllAsync(int currentUserId);


        Task<ApiMessage<AdminUserDetailsDto>> GetByIdAsync(
     int id,
     int currentUserId
 );



        Task<ApiMessage<bool>> ResetPasswordAsync(
  int id,
    ResetPasswordRequestDto dto,
    int currentUserId);




        Task<ApiMessage<bool>> UpdateBySystemAsync(
            int id,
            UpdateUserBySystemDto dto,
            int currentUserId);



        Task<ApiMessage<bool>> UpdateByCharityAdminAsync(
            int id,
            UpdateUserByCharityAdminDto dto,
            int currentUserId);


        Task<ApiMessage<bool>> DeleteAsync(
    int id,
    int currentUserId);
    }
}
