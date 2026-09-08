using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdminPanel.Application.Common;
using AdminPanel.Application.DTOs.Users;
using AdminPanel.Application.DTOs.User;
using AdminPanel.Application.Interfaces;
using AdminPanel.Core.Common;
using AdminPanel.Core.Enums;
using AdminPanel.Application.Security;

namespace AdminPanel.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        private readonly PasswordHasher _passwordHasher;

        public UserService(IUserRepository userRepository, PasswordHasher passwordHasher)
        {
            _passwordHasher = passwordHasher;
            _userRepository = userRepository;
        }




        #region Get All Users

        public async Task<ApiMessage<List<UserDto>>> GetAllAsync(int currentUserId)
        {
            try
            {
                var currentUser =
                    await _userRepository.GetUserEntityAsync(currentUserId);



                if (currentUser == null)
                {
                    return new ApiMessage<List<UserDto>>
                    {
                        HasError = true,
                        ErrorMessage = "کاربر جاری یافت نشد"
                    };
                }



                var currentRole =
                    AdminUserRoleHelper.Resolve(currentUser);



                List<UserDto> users;



                switch (currentRole)
                {

                    case AdminUserType.AdminSystem:

                        users =
                            await _userRepository.GetAllAsync();

                        break;



                    case AdminUserType.CharityAdmin:

                        if (!currentUser.CharityId.HasValue)
                        {
                            return new ApiMessage<List<UserDto>>
                            {
                                HasError = true,
                                ErrorMessage = "مدیر خیریه به خیریه‌ای متصل نیست"
                            };
                        }


                        users =
                            await _userRepository
                            .GetUsersByCharityIdAsync(
                                currentUser.CharityId.Value
                            );

                        break;



                    case AdminUserType.CharityUser:


                        var user =
                            await _userRepository
                            .GetUserByIdAsync(currentUserId);



                        users = user == null
                            ? new List<UserDto>()
                            : new List<UserDto>
                            {
                                user
                            };


                        break;



                    default:

                        users = new List<UserDto>();

                        break;
                }




                foreach (var user in users)
                {
                    SetUserType(user);


                    if (user.CharityId.HasValue)
                    {
                        var charity =
                            await _userRepository
                            .GetCharityAsync(user.CharityId.Value);


                        if (charity != null)
                        {
                            user.CharityName = charity.Name;
                        }
                    }
                }




                return new ApiMessage<List<UserDto>>
                {
                    HasError = false,
                    Value = users
                };

            }
            catch (Exception ex)
            {
                return new ApiMessage<List<UserDto>>
                {
                    HasError = true,
                    ErrorMessage = ex.Message
                };
            }
        }

        #endregion











        #region Get By Id

        public async Task<ApiMessage<AdminUserDetailsDto>> GetByIdAsync(
            int id,
            int currentUserId)
        {
            try
            {
                var currentUser =
                    await _userRepository.GetUserEntityAsync(currentUserId);


                if (currentUser == null)
                {
                    return new ApiMessage<AdminUserDetailsDto>
                    {
                        HasError = true,
                        ErrorMessage = "کاربر جاری یافت نشد"
                    };
                }



                var currentRole =
                    AdminUserRoleHelper.Resolve(currentUser);



                var targetUser =
                    await _userRepository.GetUserEntityAsync(id);



                if (targetUser == null)
                {
                    return new ApiMessage<AdminUserDetailsDto>
                    {
                        HasError = true,
                        ErrorMessage = "کاربر یافت نشد"
                    };
                }



                // CharityAdmin فقط کاربران خیریه خودش
                if (currentRole == AdminUserType.CharityAdmin &&
                   targetUser.CharityId != currentUser.CharityId)
                {
                    return new ApiMessage<AdminUserDetailsDto>
                    {
                        HasError = true,
                        ErrorMessage =
                        "شما اجازه مشاهده این کاربر را ندارید"
                    };
                }



                // CharityUser فقط خودش
                if (currentRole == AdminUserType.CharityUser &&
                   targetUser.Id != currentUserId)
                {
                    return new ApiMessage<AdminUserDetailsDto>
                    {
                        HasError = true,
                        ErrorMessage =
                        "شما فقط اطلاعات خودتان را می‌توانید مشاهده کنید"
                    };
                }



                var result =
                    await _userRepository.GetByIdAsync(id);



                return new ApiMessage<AdminUserDetailsDto>
                {
                    HasError = false,
                    Value = result
                };

            }
            catch (Exception ex)
            {
                return new ApiMessage<AdminUserDetailsDto>
                {
                    HasError = true,
                    ErrorMessage = ex.Message
                };
            }
        }

        #endregion


        #region old code
        //public async Task<ApiMessage<AdminUserDetailsDto>> GetByIdAsync(int id)
        //{
        //    try
        //    {

        //        var user =
        //            await _userRepository.GetUserEntityAsync(id);



        //        if (user == null)
        //        {
        //            return new ApiMessage<AdminUserDetailsDto>
        //            {
        //                HasError = true,
        //                ErrorMessage = "کاربر یافت نشد"
        //            };
        //        }



        //        var result =
        //            await _userRepository.GetByIdAsync(id);



        //        return new ApiMessage<AdminUserDetailsDto>
        //        {
        //            HasError = false,
        //            Value = result
        //        };

        //    }
        //    catch (Exception ex)
        //    {
        //        return new ApiMessage<AdminUserDetailsDto>
        //        {
        //            HasError = true,
        //            ErrorMessage = ex.Message
        //        };
        //    }
        //}


        #endregion





        #region Update By System


        public async Task<ApiMessage<bool>> UpdateBySystemAsync(
            int id,
            UpdateUserBySystemDto dto,
            int currentUserId)
        {

            try
            {

                var currentUser =
                    await _userRepository
                    .GetUserEntityAsync(currentUserId);



                if (currentUser == null)
                {
                    return new ApiMessage<bool>
                    {
                        HasError = true,
                        ErrorMessage = "کاربر جاری یافت نشد"
                    };
                }



                var currentRole =
                    AdminUserRoleHelper.Resolve(currentUser);



                if (currentRole != AdminUserType.AdminSystem)
                {
                    return new ApiMessage<bool>
                    {
                        HasError = true,
                        ErrorMessage =
                        "فقط مدیر سیستم اجازه این عملیات را دارد"
                    };
                }



                var user =
                    await _userRepository.GetUserEntityAsync(id);



                if (user == null)
                {
                    return new ApiMessage<bool>
                    {
                        HasError = true,
                        ErrorMessage = "کاربر یافت نشد"
                    };
                }



                user.Name = dto.Name;

                user.UserName = dto.UserName;

                user.Mobile = dto.Mobile;

                user.Sex = dto.Sex;




                if (dto.UserType == AdminUserType.AdminSystem)
                {
                    user.IsRoot = true;
                    user.IsAdmin = false;
                    user.CharityId = null;
                }
                else
                {
                    user.IsRoot = false;

                    user.IsAdmin =
                        dto.UserType == AdminUserType.CharityAdmin;

                    user.CharityId =
                        dto.CharityId;
                }



                await _userRepository.UpdateUserAsync(user);



                return new ApiMessage<bool>
                {
                    HasError = false,
                    Value = true
                };

            }
            catch (Exception ex)
            {
                return new ApiMessage<bool>
                {
                    HasError = true,
                    ErrorMessage = ex.Message
                };

            }
        }
        #endregion






        #region Update By CharityAdmin


        public async Task<ApiMessage<bool>> UpdateByCharityAdminAsync(
            int id,
            UpdateUserByCharityAdminDto dto,
            int currentUserId)
        {
            try
            {
                var currentUser =
                    await _userRepository.GetUserEntityAsync(currentUserId);


                if (currentUser == null)
                {
                    return new ApiMessage<bool>
                    {
                        HasError = true,
                        ErrorMessage = "کاربر جاری یافت نشد"
                    };
                }



                var currentRole =
                    AdminUserRoleHelper.Resolve(currentUser);



                if (currentRole != AdminUserType.CharityAdmin)
                {
                    return new ApiMessage<bool>
                    {
                        HasError = true,
                        ErrorMessage = "فقط مدیر خیریه اجازه این عملیات را دارد"
                    };
                }



                var targetUser =
                    await _userRepository.GetUserEntityAsync(id);



                if (targetUser == null)
                {
                    return new ApiMessage<bool>
                    {
                        HasError = true,
                        ErrorMessage = "کاربر یافت نشد"
                    };
                }



                // فقط کاربران خیریه خودش
                if (targetUser.CharityId != currentUser.CharityId)
                {
                    return new ApiMessage<bool>
                    {
                        HasError = true,
                        ErrorMessage =
                        "شما فقط کاربران خیریه خود را می‌توانید ویرایش کنید"
                    };
                }



                // جلوگیری از تغییر مدیر سیستم
                if (targetUser.IsRoot)
                {
                    return new ApiMessage<bool>
                    {
                        HasError = true,
                        ErrorMessage =
                        "امکان ویرایش مدیر سیستم وجود ندارد"
                    };
                }



                targetUser.Name = dto.Name;

                targetUser.UserName = dto.UserName;

                targetUser.Mobile = dto.Mobile;

                targetUser.Sex = dto.Sex;



                // عمداً:
                // CharityId تغییر نمی‌کند
                // Role تغییر نمی‌کند
                // IsRoot تغییر نمی‌کند



                await _userRepository.UpdateUserAsync(targetUser);



                return new ApiMessage<bool>
                {
                    HasError = false,
                    Value = true
                };

            }
            catch (Exception ex)
            {
                return new ApiMessage<bool>
                {
                    HasError = true,
                    ErrorMessage = ex.Message
                };
            }
        }


        #endregion




        #region Delete User


        public async Task<ApiMessage<bool>> DeleteAsync(
            int id,
            int currentUserId)
        {
            try
            {

                var currentUser =
                    await _userRepository.GetUserEntityAsync(currentUserId);



                if (currentUser == null)
                {
                    return new ApiMessage<bool>
                    {
                        HasError = true,
                        ErrorMessage = "کاربر جاری یافت نشد"
                    };
                }



                var currentRole =
                    AdminUserRoleHelper.Resolve(currentUser);



                var targetUser =
                    await _userRepository.GetUserEntityAsync(id);



                if (targetUser == null)
                {
                    return new ApiMessage<bool>
                    {
                        HasError = true,
                        ErrorMessage = "کاربر یافت نشد"
                    };
                }



                // AdminSystem
                if (currentRole == AdminUserType.AdminSystem)
                {

                    if (targetUser.IsRoot)
                    {
                        return new ApiMessage<bool>
                        {
                            HasError = true,
                            ErrorMessage =
                            "امکان حذف مدیر کل سیستم وجود ندارد"
                        };
                    }

                }



                // CharityAdmin
                else if (currentRole == AdminUserType.CharityAdmin)
                {
                    if (targetUser.CharityId != currentUser.CharityId)
                    {
                        return new ApiMessage<bool>
                        {
                            HasError = true,
                            ErrorMessage =
                            "شما فقط کاربران خیریه خود را می‌توانید حذف کنید"
                        };
                    }


                    var targetRole = AdminUserRoleHelper.Resolve(targetUser);


                    if (targetRole != AdminUserType.CharityUser)
                    {
                        return new ApiMessage<bool>
                        {
                            HasError = true,
                            ErrorMessage =
                            "مدیر خیریه فقط می‌تواند کاربران خیریه را حذف کند"
                        };
                    }
                }



                else
                {
                    return new ApiMessage<bool>
                    {
                        HasError = true,
                        ErrorMessage =
                        "شما اجازه حذف کاربر را ندارید"
                    };
                }



                await _userRepository.DeleteAsync(targetUser);



                return new ApiMessage<bool>
                {
                    HasError = false,
                    Value = true
                };

            }
            catch (Exception ex)
            {
                return new ApiMessage<bool>
                {
                    HasError = true,
                    ErrorMessage = ex.Message
                };
            }
        }


        #endregion





        #region Set User Type

        private void SetUserType(UserDto user)
        {
            if (user.IsRoot)
            {
                user.UserType = AdminUserType.AdminSystem;
            }

            else if (user.IsAdmin)
            {
                user.UserType = AdminUserType.CharityAdmin;
            }

            else
            {
                user.UserType = AdminUserType.CharityUser;
            }
        }

        #endregion






        #region Reset Password

        public async Task<ApiMessage<bool>> ResetPasswordAsync(
            int id,
            ResetPasswordRequestDto dto,
            int currentUserId)
        {
            try
            {
                var currentUser =
                    await _userRepository.GetUserEntityAsync(currentUserId);


                if (currentUser == null)
                {
                    return new ApiMessage<bool>
                    {
                        HasError = true,
                        ErrorMessage = "کاربر جاری یافت نشد"
                    };
                }


                var targetUser =
                    await _userRepository.GetUserEntityAsync(id);


                if (targetUser == null)
                {
                    return new ApiMessage<bool>
                    {
                        HasError = true,
                        ErrorMessage = "کاربر مورد نظر یافت نشد"
                    };
                }



                // بررسی تکرار رمز
                if (dto.NewPassword != dto.ConfirmPassword)
                {
                    return new ApiMessage<bool>
                    {
                        HasError = true,
                        ErrorMessage = "رمز عبور جدید و تکرار آن یکسان نیست"
                    };
                }



                var currentRole =
                    AdminUserRoleHelper.Resolve(currentUser);



                // =============================
                // تغییر رمز خود کاربر
                // =============================

                if (id == currentUserId)
                {

                    if (string.IsNullOrEmpty(dto.CurrentPassword))
                    {
                        return new ApiMessage<bool>
                        {
                            HasError = true,
                            ErrorMessage = "رمز عبور فعلی الزامی است"
                        };
                    }



                    var isValid =
                        _passwordHasher.Verify(
                            currentUser.PasswordHash,
                            dto.CurrentPassword
                        );


                    if (!isValid)
                    {
                        return new ApiMessage<bool>
                        {
                            HasError = true,
                            ErrorMessage = "رمز عبور فعلی اشتباه است"
                        };
                    }

                }



                // =============================
                // تغییر رمز دیگر کاربران
                // =============================

                else
                {

                    if (currentRole == AdminUserType.AdminSystem)
                    {

                        //if (targetUser.IsRoot)
                        //{
                        //    return new ApiMessage<bool>
                        //    {
                        //        HasError = true,
                        //        ErrorMessage =
                        //        "امکان تغییر رمز مدیر سیستم وجود ندارد"
                        //    };
                        //}

                    }



                    else if (currentRole == AdminUserType.CharityAdmin)
                    {

                        var targetRole =
                            AdminUserRoleHelper.Resolve(targetUser);



                        if (targetRole != AdminUserType.CharityUser)
                        {
                            return new ApiMessage<bool>
                            {
                                HasError = true,
                                ErrorMessage =
                                "مدیر خیریه فقط می‌تواند رمز کاربران خیریه را تغییر دهد"
                            };
                        }



                        if (targetUser.CharityId != currentUser.CharityId)
                        {
                            return new ApiMessage<bool>
                            {
                                HasError = true,
                                ErrorMessage =
                                "شما فقط کاربران خیریه خود را می‌توانید تغییر دهید"
                            };
                        }

                    }


                    else
                    {
                        return new ApiMessage<bool>
                        {
                            HasError = true,
                            ErrorMessage =
                            "شما اجازه تغییر رمز کاربران را ندارید"
                        };
                    }

                }



                targetUser.PasswordHash =
                    _passwordHasher.Hash(dto.NewPassword);



                await _userRepository.UpdateUserAsync(targetUser);



                return new ApiMessage<bool>
                {
                    HasError = false,
                    Value = true
                };

            }
            catch (Exception ex)
            {
                return new ApiMessage<bool>
                {
                    HasError = true,
                    ErrorMessage = ex.Message
                };
            }
        }

       


        #endregion





    }
}
