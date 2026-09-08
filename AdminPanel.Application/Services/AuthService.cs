using AdminPanel.Application.Common;
using AdminPanel.Application.DTOs.Auth;
using AdminPanel.Application.Interfaces;
using AdminPanel.Application.Security;
using AdminPanel.Core.Common;
using AdminPanel.Core.Enums;
using AdminPanel.Infrastructure.Persistence.Data;
using System.Data;


namespace AdminPanel.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAdminUserRepository _adminUserRepository;
        private readonly PasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;

        public AuthService(IAdminUserRepository adminUserRepository, IJwtService jwtService)
        {
            _adminUserRepository = adminUserRepository;
            _jwtService = jwtService;
            _passwordHasher = new PasswordHasher();
        }



        public async Task<ApiMessage<LoginResponseDto>> LoginAsync(LoginRequestDto request)
        {
            if (request == null)
            {
                return new ApiMessage<LoginResponseDto>
                {
                    HasError = true,
                    ErrorMessage = "اطلاعات ورود ارسال نشده است",
                    Value = null
                };
            }


            //if (string.IsNullOrWhiteSpace(request.UserName))
            //{
            //    return new ApiMessage<LoginResponseDto>
            //    {
            //        HasError = true,
            //        ErrorMessage = "نام کاربری را وارد کنید",
            //        Value = null
            //    };
            //}


            //if (string.IsNullOrWhiteSpace(request.Password))
            //{
            //    return new ApiMessage<LoginResponseDto>
            //    {
            //        HasError = true,
            //        ErrorMessage = "رمز عبور را وارد کنید",
            //        Value = null
            //    };
            //}


            var user = await _adminUserRepository.GetByUserNameAsync(request.UserName);


            if (user == null)
            {
                return new ApiMessage<LoginResponseDto>
                {
                    HasError = true,
                    ErrorMessage = "نام کاربری یا رمز عبور اشتباه است",
                    Value = null
                };
            }

            
            var isValidPassword = _passwordHasher.Verify(user.PasswordHash, request.Password);

            
            if (!isValidPassword)
            {
                return new ApiMessage<LoginResponseDto>
                {
                    HasError = true,
                    ErrorMessage = "نام کاربری یا رمز عبور اشتباه است",
                    Value = null
                };
            }




            // Using a Helper to determine the role based on database fields
            var detectedUserType = AdminUserRoleHelper.Resolve(user);



            // Pass the detected role to JwtService
            var token = _jwtService.GenerateToken(
                user.Id, 
                user.UserName,
                detectedUserType,
                user.CharityId
                );



            var loginResponse = new LoginResponseDto
            {
                UserId = user.Id,
                Name = user.Name,
                UserName = user.UserName,
                Token = token,
                UserType = detectedUserType,
                CharityId = user.CharityId
            };


            return new ApiMessage<LoginResponseDto>
            {
                HasError = false,
                ErrorMessage = null,
                Value = loginResponse
            };

        }




        public async Task<ApiMessage<bool>> RegisterAsync(
            RegisterRequestDto request,
            AdminUserType creatorUserType,
            int? creatorCharityId)
        {

            if (request == null)
            {
                return new ApiMessage<bool>
                {
                    HasError = true,
                    ErrorMessage = "اطلاعات ثبت نام ارسال نشده است"
                };
            }


            if (await _adminUserRepository.IsUserNameExistAsync(request.UserName))
            {
                return new ApiMessage<bool>
                {
                    HasError = true,
                    ErrorMessage = "نام کاربری قبلا ثبت شده است"
                };
            }




            // Apply security rules based on the creator's role
            if (creatorUserType == AdminUserType.CharityAdmin)
            {
                // A CharityAdmin user can only create CharityUser accounts, and their CharityId will automatically be set to the admin's CharityId.
                request.UserType = AdminUserType.CharityUser;
                request.CharityId = creatorCharityId;

                if (request.CharityId == null)
                {
                    return new ApiMessage<bool> { HasError = true, ErrorMessage = "خیریه شما نامشخص است؛ امکان ایجاد کاربر وجود ندارد." };
                }
            }
            else if (creatorUserType == AdminUserType.AdminSystem)
            {

                // Only root (SystemAdmin) can create a CharityAdmin. Here, because of root access, the value provided in the DTO is معتبر.

                // If root wants to create a CharityAdmin or CharityUser, they must select a charity.
                if (request.UserType == AdminUserType.CharityAdmin || request.UserType == AdminUserType.CharityUser)
                {
                    if (request.CharityId == null || request.CharityId <= 0)
                    {
                        return new ApiMessage<bool> { HasError = true, ErrorMessage = "انتخاب خیریه برای این نقش الزامی است." };
                    }
                }


                // When root creates a new root user, no charity should be selected.
                if (request.UserType == AdminUserType.AdminSystem)
                {
                    request.CharityId = null;
                }

            }
            else
            {

                // Regular users (CharityUser) and anonymous users are not allowed to create users.
                return new ApiMessage<bool> 
                { 
                    HasError = true, 
                    ErrorMessage = "شما دسترسی لازم برای ایجاد کاربر را ندارید." 
                };

            }









            // Map the selected role to the corresponding flags in the AdminUser entity
            bool isRoot = false;
            bool isAdmin = false;



            if (request.UserType == AdminUserType.AdminSystem)
            {
                isRoot = true;
                isAdmin = true;
            }
            else if (request.UserType == AdminUserType.CharityAdmin)
            {
                isRoot = false;
                isAdmin = true;
            }



            var user = new AdminUser()
            {
                Name = request.Name,
                UserName = request.UserName,
                PasswordHash = _passwordHasher.Hash(request.Password),
                Mobile = request.Mobile,
                CreatedAt = DateTime.Now,

                // Save the values from the DTO
                CharityId = request.CharityId,
                IsRoot = isRoot,
                IsAdmin = isAdmin,
                IsDeleted = false,
                Sex = true, //Default value for sex


                // Legacy user type field in the database
                UserType = false
            };




            await _adminUserRepository.AddAsync(user);
            await _adminUserRepository.SaveChangesAsync();




            return new ApiMessage<bool>
            {
                HasError = false,
                Value = true
            };




        }

    }
}
