using System.Security.Claims;

using AdminPanel.Application.Common;
using AdminPanel.Application.DTOs.User;
using AdminPanel.Application.DTOs.Users;
using AdminPanel.Application.Interfaces;
using AdminPanel.Core.Enums;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace AdminPanel.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IUserService _userService;


        public UserController(IUserService userService)
        {
            _userService = userService;
        }



        private int GetCurrentUserId()
        {
            return int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );
        }



        private AdminUserType GetCurrentUserType()
        {
            return (AdminUserType)int.Parse(
                User.FindFirst("UserType")!.Value
            );
        }








        // دریافت لیست کاربران

        [HttpGet]
        public async Task<ActionResult<ApiMessage<List<UserDto>>>> GetAll()
        {

            var currentUserId = GetCurrentUserId();

            var userType = GetCurrentUserType();



            // SystemAdmin -> همه کاربران

            if (userType == AdminUserType.AdminSystem)
            {
                return Ok(
                    await _userService.GetAllAsync(currentUserId)
                );
            }




            // CharityAdmin -> فقط کاربران خیریه خودش
            // این محدودیت باید داخل Service اعمال شود

            if (userType == AdminUserType.CharityAdmin)
            {
                return Ok(
                    await _userService.GetAllAsync(currentUserId)
                );
            }





            // CharityUser -> فقط خودش

            if (userType == AdminUserType.CharityUser)
            {
                return Ok(
                    await _userService.GetByIdAsync(
                        currentUserId,
                        currentUserId
                    )
                );
            }



            return Forbid();
        }








        [HttpGet("user-types")]
        public ActionResult<ApiMessage<List<UserTypeOptionDto>>> GetUserTypes()
        {
            var currentUserType = GetCurrentUserType();

            var userTypes = new List<UserTypeOptionDto>();

            if (currentUserType == AdminUserType.AdminSystem)
            {
                userTypes.Add(new UserTypeOptionDto
                {
                    Id = (int)AdminUserType.AdminSystem,
                    Key = AdminUserType.AdminSystem.ToString(),
                    Title = "System Admin"
                });

                userTypes.Add(new UserTypeOptionDto
                {
                    Id = (int)AdminUserType.CharityAdmin,
                    Key = AdminUserType.CharityAdmin.ToString(),
                    Title = "Charity Admin"
                });

                userTypes.Add(new UserTypeOptionDto
                {
                    Id = (int)AdminUserType.CharityUser,
                    Key = AdminUserType.CharityUser.ToString(),
                    Title = "Charity User"
                });
            }
            else if (currentUserType == AdminUserType.CharityAdmin)
            {
                userTypes.Add(new UserTypeOptionDto
                {
                    Id = (int)AdminUserType.CharityUser,
                    Key = AdminUserType.CharityUser.ToString(),
                    Title = "Charity User"
                });
            }
            else
            {
                return Forbid();
            }

            return Ok(userTypes);
        }










        // جزئیات کاربر

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiMessage<AdminUserDetailsDto>>> GetById(int id)
        {

            var currentUserId = GetCurrentUserId();


            return Ok(
                await _userService.GetByIdAsync(
                    id,
                    currentUserId
                )
            );

        }









        // ویرایش توسط SystemAdmin

        [HttpPut("system/{id:int}")]
        public async Task<ActionResult<ApiMessage<bool>>> UpdateBySystem(
            int id,
            UpdateUserBySystemDto dto)
        {


            if (GetCurrentUserType() != AdminUserType.AdminSystem)
            {
                return Forbid();
            }



            return Ok(
                await _userService.UpdateBySystemAsync(
                    id,
                    dto,
                    GetCurrentUserId()
                )
            );

        }










        // ویرایش توسط CharityAdmin

 [HttpPut("charity-admin/{id:int}")]
public async Task<ActionResult<ApiMessage<bool>>> UpdateByCharityAdmin(
    int id,
    UpdateUserByCharityAdminDto dto)
{
    var currentUserType = GetCurrentUserType();

    if (currentUserType != AdminUserType.CharityAdmin && 
        currentUserType != AdminUserType.AdminSystem)
    {
        return Forbid();
    }

    return Ok(
        await _userService.UpdateByCharityAdminAsync(
            id,
            dto,
            GetCurrentUserId()
        )
    );
}










        // حذف

        #region Delete User


        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiMessage<bool>>> Delete(int id)
        {
            var currentUserType = GetCurrentUserType();

            // فقط AdminSystem و CharityAdmin اجازه حذف دارند
            if (currentUserType != AdminUserType.AdminSystem &&
                currentUserType != AdminUserType.CharityAdmin)
            {
                return Forbid();
            }

            return Ok(
                await _userService.DeleteAsync(
                    id,
                    GetCurrentUserId()
                )
            );
        }

        #endregion







        #region Reset Password


        [Authorize]
        [HttpPut("change-password/{id}")]
        public async Task<IActionResult> ChangePassword(
        int id,
        [FromBody] ResetPasswordRequestDto dto)
        {
            try
            {
                var userIdClaim =
                    User.FindFirst(
                        System.Security.Claims.ClaimTypes.NameIdentifier
                    )?.Value;


                if (!int.TryParse(userIdClaim, out int currentUserId))
                {
                    return Unauthorized(new ApiMessage<bool>
                    {
                        HasError = true,
                        ErrorMessage = "کاربر جاری شناسایی نشد",
                        Value = false
                    });
                }


                var result =
                    await _userService.ResetPasswordAsync(
                        id,
                        dto,
                        currentUserId
                    );


                if (result.HasError)
                {
                    return BadRequest(result);
                }


                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest(new ApiMessage<bool>
                {
                    HasError = true,
                    ErrorMessage = ex.Message,
                    Value = false
                });
            }
        }
        #endregion

    }
}