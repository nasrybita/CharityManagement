using AdminPanel.Application.Common;
using AdminPanel.Application.DTOs.Campaign;
using AdminPanel.Application.DTOs.Category;
using AdminPanel.Application.DTOs.Common;
using AdminPanel.Application.Interfaces;
using AdminPanel.Core.Enums;
using AdminPanel.Infrastructure.Persistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CampaignStatusEnum = AdminPanel.Core.Enums.CampaignStatus;

namespace AdminPanel.Application.Services
{
    public class CampaignService : ICampaignService
    {
        private readonly ICampaignRepository _campaignRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IFileStorageService _fileStorageService;

        public CampaignService(
            ICampaignRepository campaignRepository,
            ICategoryRepository categoryRepository,
            IFileStorageService fileStorageService)
        {
            _campaignRepository = campaignRepository;
            _categoryRepository = categoryRepository;
            _fileStorageService = fileStorageService;
        }

        public async Task<ApiMessage<bool>> CreateAsync(CreateCampaignRequestDto request)
        {
            var result = new ApiMessage<bool>();

            if (request.CharityId <= 0)
            {
                result.HasError = true;
                result.ErrorMessage = "خیریه معتبر نیست";
                return result;
            }

            if (string.IsNullOrWhiteSpace(request.Title))
            {
                result.HasError = true;
                result.ErrorMessage = "عنوان کمپین الزامی است";
                return result;
            }

            if (request.CategoryIds == null || !request.CategoryIds.Any())
            {
                result.HasError = true;
                result.ErrorMessage = "حداقل یک دسته‌بندی باید انتخاب شود";
                return result;
            }

            // Ensure the title is unique
            if (await _campaignRepository.IsTitleExistAsync(request.Title.Trim()))
            {
                result.HasError = true;
                result.ErrorMessage = "عنوان کمپین قبلاً ثبت شده است";
                return result;
            }

            //Verify that the categories are allowed for the specified charity
            var areCategoriesValid = await _campaignRepository.AreCategoriesValidForCharityAsync(request.CharityId, request.CategoryIds);

            if (!areCategoriesValid)
            {
                result.HasError = true;
                result.ErrorMessage = "یک یا چند دسته‌بندی انتخابی در حوزه فعالیت‌های مجاز این خیریه نمی‌باشد";
                return result;
            }

            int? bannerId = null;

            if (request.BannerFile != null)
            {
                var banner = await _fileStorageService.SaveBannerAsync(request.BannerFile);
                bannerId = banner.Id;
            }

            // Create campaign entity
            var campaign = new Campaign
            {
                Title = request.Title.Trim(),
                Description = request.Description,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                TotalAmount = request.TotalAmount,
                ChargedAmount = 0, // It is ZERO at the begining
                CityId = request.CityId,
                CharityId = request.CharityId,
                CampaignStatus = (int)CampaignStatusEnum.Created,
                BannerId = bannerId,
                CreatedAt = DateTime.Now,
                ModifiedAt = DateTime.Now,
                IsDeleted = false
            };

            // Link categories to the campaign (Many-to-Many relationship)
            if (request.CategoryIds != null && request.CategoryIds.Any())
            {
                var categories = await _categoryRepository.GetByIdsAsync(request.CategoryIds);

                foreach (var category in categories)
                {
                    campaign.Categories.Add(category);
                }
            }

            await _campaignRepository.AddAsync(campaign);
            await _campaignRepository.SaveChangesAsync();

            result.Value = true;
            return result;
        }

        public async Task<ApiMessage<bool>> UpdateAsync(UpdateCampaignRequestDto request, AdminUserType userRole)
        {
            var result = new ApiMessage<bool>();
            var campaign = await _campaignRepository.GetByIdWithCategoriesAsync(request.Id);

            if (campaign == null)
            {
                result.HasError = true;
                result.ErrorMessage = "کمپین یافت نشد";
                return result;
            }

            // Check campaignStatus before editing campaign
            //Campaign can not be edited if its status is "Approved" or "Reactivated"
            if (userRole != AdminUserType.AdminSystem)
            {
                var status = (CampaignStatusEnum)campaign.CampaignStatus;
                if (status == CampaignStatusEnum.Approved || status == CampaignStatusEnum.Reactivated)
                {
                    result.HasError = true;
                    result.ErrorMessage = "امکان ویرایش کمپین در وضعیت فعال (تایید شده یا فعال‌سازی مجدد) وجود ندارد. ابتدا وضعیت کمپین را غیرفعال کنید.";
                    return result;
                }
            }

            // Ensure the new name is unique
            if (campaign.Title != request.Title.Trim() && await _campaignRepository.IsTitleExistAsync(request.Title.Trim()))
            {
                result.HasError = true;
                result.ErrorMessage = "عنوان کمپین تکراری است";
                return result;
            }

            // Verify that the categories are allowed for the charity
            var areCategoriesValid = await _campaignRepository.AreCategoriesValidForCharityAsync(request.CharityId, request.CategoryIds);

            if (!areCategoriesValid)
            {
                result.HasError = true;
                result.ErrorMessage = "یک یا چند دسته‌بندی انتخابی در حوزه فعالیت‌های مجاز این خیریه نمی‌باشد";
                return result;
            }

            // Banner handling
            var oldBannerId = campaign.BannerId;

            if (request.RemoveBanner)
            {
                // Remove banner relation from campaign
                campaign.BannerId = null;

                // Also delete stored file if exists
                if (oldBannerId.HasValue)
                {
                    await _fileStorageService.DeleteAsync(oldBannerId.Value);
                }
            }
            else if (request.BannerFile != null)
            {
                // Replace with new banner
                var banner = await _fileStorageService.SaveBannerAsync(request.BannerFile);
                campaign.BannerId = banner.Id;

                // Remove previous banner
                if (oldBannerId.HasValue && oldBannerId.Value != banner.Id)
                    await _fileStorageService.DeleteAsync(oldBannerId.Value);
            }

            campaign.Title = request.Title.Trim();
            campaign.Description = request.Description;
            campaign.StartDate = request.StartDate;
            campaign.EndDate = request.EndDate;
            campaign.TotalAmount = request.TotalAmount;
            campaign.CityId = request.CityId;
            campaign.ModifiedAt = DateTime.Now;

            // Update the categories' many-to-many relationship
            campaign.Categories.Clear();
            if (request.CategoryIds != null && request.CategoryIds.Any())
            {
                var categories = await _categoryRepository.GetByIdsAsync(request.CategoryIds);

                foreach (var category in categories)
                {
                    campaign.Categories.Add(category);
                }
            }

            await _campaignRepository.SaveChangesAsync();
            result.Value = true;
            return result;
        }

        public async Task<ApiMessage<bool>> DeleteAsync(int id, AdminUserType userRole)
        {
            var result = new ApiMessage<bool>();
            var campaign = await _campaignRepository.GetByIdAsync(id);

            if (campaign == null)
            {
                result.HasError = true;
                result.ErrorMessage = "کمپین یافت نشد";
                return result;
            }

            // Check campaignStatus before deleting campaign
            //Campaign can not be deleted if its status is "Approved" or "Reactivated"
            if (userRole != AdminUserType.AdminSystem)
            {
                var status = (CampaignStatusEnum)campaign.CampaignStatus;
                if (status == CampaignStatusEnum.Approved || status == CampaignStatusEnum.Reactivated)
                {
                    result.HasError = true;
                    result.ErrorMessage = "امکان حذف کمپین در وضعیت فعال وجود ندارد. وضعیت کمپین نباید تایید شده یا فعال‌سازی مجدد باشد.";
                    return result;
                }
            }

            campaign.IsDeleted = true;
            campaign.ModifiedAt = DateTime.Now;

            await _campaignRepository.SaveChangesAsync();
            result.Value = true;
            return result;
        }

        public async Task<ApiMessage<CampaignDetailsDto>> GetByIdAsync(int id)
        {
            var result = new ApiMessage<CampaignDetailsDto>();
            var campaign = await _campaignRepository.GetByIdWithCategoriesAsync(id);

            if (campaign == null)
            {
                result.HasError = true;
                result.ErrorMessage = "کمپین یافت نشد";
                return result;
            }

            var dto = new CampaignDetailsDto
            {
                Id = campaign.Id,
                Title = campaign.Title,
                Description = campaign.Description,
                StartDate = campaign.StartDate,
                EndDate = campaign.EndDate,
                TotalAmount = campaign.TotalAmount,
                ChargedAmount = campaign.ChargedAmount,
                CityId = campaign.CityId,
                CityName = campaign.City?.Name,
                CharityId = campaign.CharityId,
                CharityName = campaign.Charity.Name,
                CampaignStatus = campaign.CampaignStatus,
                CreatedAt = campaign.CreatedAt,
                ModifiedAt = campaign.ModifiedAt,
                BannerId = campaign.BannerId,
                BannerUrl = campaign.Banner?.FilePath,
                Categories = campaign.Categories.Select(cat => new CategoryListItemDto
                {
                    Id = cat.Id,
                    Name = cat.Name,
                    CreatedAt = cat.CreatedAt
                }).ToList()
            };

            result.Value = dto;
            return result;
        }

        public async Task<ApiMessage<PagedResultDto<CampaignListItemDto>>> GetListAsync(
            PaginationRequestDto request,
            int? charityId,
            int? categoryId,
            int? campaignStatus)
        {
            var result = new ApiMessage<PagedResultDto<CampaignListItemDto>>();

            var (items, totalCount) = await _campaignRepository.GetFilteredPagedAsync(
                charityId,
                categoryId,
                campaignStatus,
                request.Page,
                request.PageSize);

            var list = items.Select(x => new CampaignListItemDto
            {
                Id = x.Id,
                Title = x.Title,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                TotalAmount = x.TotalAmount,
                ChargedAmount = x.ChargedAmount,
                CharityName = x.Charity.Name,
                CampaignStatus = x.CampaignStatus,
                CreatedAt = x.CreatedAt
            }).ToList();

            var paged = new PagedResultDto<CampaignListItemDto>
            {
                Items = list,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };

            result.Value = paged;
            return result;
        }

        //For systemAdmin to be able to change campaignStatus
        public async Task<ApiMessage<bool>> ChangeStatusAsync(int id, CampaignStatusEnum newStatus, AdminUserType userRole)
        {
            var result = new ApiMessage<bool>();
            var campaign = await _campaignRepository.GetByIdAsync(id);

            if (campaign == null)
            {
                result.HasError = true;
                result.ErrorMessage = "کمپین یافت نشد";
                return result;
            }

            var currentStatus = (CampaignStatusEnum)campaign.CampaignStatus;

            //If client suspended a campaign, change it to SuspendedBySystem or SuspendedByCharity according to user role
            if (newStatus == CampaignStatusEnum.Suspended)
            {
                if (userRole == AdminUserType.AdminSystem)
                    newStatus = CampaignStatusEnum.SuspendedBySystem;
                else if (IsCharityUser(userRole))
                    newStatus = CampaignStatusEnum.SuspendedByCharity;
            }


            if (newStatus == CampaignStatusEnum.Rejected)
            {
                if (userRole == AdminUserType.AdminSystem)
                    newStatus = CampaignStatusEnum.RejectedBySystem;
                else if (IsCharityUser(userRole))
                    newStatus = CampaignStatusEnum.RejectedByCharity;
            }




            //Prevent status change to the exact current staus
            if (currentStatus == newStatus)
            {
                result.HasError = true;
                result.ErrorMessage = "کمپین در حال حاضر در همین وضعیت قرار دارد.";
                return result;
            }



            //If campaign has been suspended by system, only system can reactivate it
            if (currentStatus == CampaignStatusEnum.SuspendedBySystem &&
                newStatus == CampaignStatusEnum.Reactivated &&
                userRole != AdminUserType.AdminSystem)
            {
                result.HasError = true;
                result.ErrorMessage = "این کمپین توسط مدیریت سیستم تعلیق شده و فقط مدیریت سیستم اجازه فعال‌سازی مجدد آن را دارد.";
                return result;
            }


            //If campaign has been suspended by charity, only charity can reactivate it
            if (currentStatus == CampaignStatusEnum.SuspendedByCharity &&
                newStatus == CampaignStatusEnum.Reactivated &&
                !IsCharityUser(userRole))
            {
                result.HasError = true;
                result.ErrorMessage = "این کمپین توسط خیریه غیرفعال شده و فقط خود خیریه اجازه فعال‌سازی مجدد آن را دارد.";
                return result;
            }



            // If campaign has been rejected by system, only system can approve it again
            if (currentStatus == CampaignStatusEnum.RejectedBySystem &&
                newStatus == CampaignStatusEnum.Approved &&
                userRole != AdminUserType.AdminSystem)
            {
                result.HasError = true;
                result.ErrorMessage = "این کمپین توسط مدیریت سیستم رد شده و فقط مدیریت سیستم اجازه تایید مجدد آن را دارد.";
                return result;
            }



            // If campaign has been rejected by charity, only charity can approve it again
            if (currentStatus == CampaignStatusEnum.RejectedByCharity &&
                newStatus == CampaignStatusEnum.Approved &&
                !IsCharityUser(userRole))
            {
                result.HasError = true;
                result.ErrorMessage = "این کمپین توسط خیریه رد شده و فقط خود خیریه اجازه تایید مجدد آن را دارد.";
                return result;
            }



            //General control of all transitions
            if (!IsValidTransition(currentStatus, newStatus, userRole))
            {
                result.HasError = true;
                result.ErrorMessage = "شما اجازه انجام این تغییر وضعیت را ندارید.";
                return result;
            }

            campaign.CampaignStatus = (int)newStatus;
            campaign.ModifiedAt = DateTime.Now;

            await _campaignRepository.SaveChangesAsync();

            result.Value = true;
            return result;

        }



        private static bool IsValidTransition(CampaignStatusEnum current, CampaignStatusEnum next, AdminUserType role)
        {
            return (current, next) switch
            {
                // Created
                (CampaignStatusEnum.Created, CampaignStatusEnum.Approved)
                    => role == AdminUserType.AdminSystem || IsCharityUser(role),

                (CampaignStatusEnum.Created, CampaignStatusEnum.RejectedBySystem)
                    => role == AdminUserType.AdminSystem,

                (CampaignStatusEnum.Created, CampaignStatusEnum.RejectedByCharity)
                    => IsCharityUser(role),


                // Rejected legacy
                // برای داده‌های قدیمی که فقط Rejected دارند و مشخص نیست توسط چه کسی رد شده‌اند
                (CampaignStatusEnum.Rejected, CampaignStatusEnum.Approved)
                    => role == AdminUserType.AdminSystem || IsCharityUser(role),


                // Rejected by system
                (CampaignStatusEnum.RejectedBySystem, CampaignStatusEnum.Approved)
                    => role == AdminUserType.AdminSystem,


                // Rejected by charity
                (CampaignStatusEnum.RejectedByCharity, CampaignStatusEnum.Approved)
                    => IsCharityUser(role),


                // Approved
                (CampaignStatusEnum.Approved, CampaignStatusEnum.SuspendedBySystem)
                    => role == AdminUserType.AdminSystem,

                (CampaignStatusEnum.Approved, CampaignStatusEnum.SuspendedByCharity)
                    => IsCharityUser(role),


                // Reactivated
                (CampaignStatusEnum.Reactivated, CampaignStatusEnum.SuspendedBySystem)
                    => role == AdminUserType.AdminSystem,

                (CampaignStatusEnum.Reactivated, CampaignStatusEnum.SuspendedByCharity)
                    => IsCharityUser(role),


                // Suspended by system
                (CampaignStatusEnum.SuspendedBySystem, CampaignStatusEnum.Reactivated)
                    => role == AdminUserType.AdminSystem,


                // Suspended by charity
                (CampaignStatusEnum.SuspendedByCharity, CampaignStatusEnum.Reactivated)
                    => IsCharityUser(role),


                // Suspended legacy
                (CampaignStatusEnum.Suspended, CampaignStatusEnum.SuspendedBySystem)
                    => role == AdminUserType.AdminSystem,

                (CampaignStatusEnum.Suspended, CampaignStatusEnum.SuspendedByCharity)
                    => IsCharityUser(role),

                (CampaignStatusEnum.Suspended, CampaignStatusEnum.Reactivated)
                    => role == AdminUserType.AdminSystem || IsCharityUser(role),

                _ => false
            };
        }



        private static bool IsCharityUser(AdminUserType role)
        {
            return role == AdminUserType.CharityAdmin ||
                   role == AdminUserType.CharityUser;
        }
    }
}
