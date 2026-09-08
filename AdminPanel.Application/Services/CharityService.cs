
using AdminPanel.Application.Common;
using AdminPanel.Application.DTOs.Charity;
using AdminPanel.Application.DTOs.Common;
using AdminPanel.Application.DTOs.Social;
using AdminPanel.Application.Interfaces;
using AdminPanel.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Application.Services
{
    public class CharityService : ICharityService
    {
        private const string ApiBaseUrl = "https://localhost:7209";

        private readonly ICharityRepository _charityRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly ICharityCategoryRepository _charityCategoryRepository;


        public CharityService(ICharityRepository charityRepository, IFileStorageService fileStorageService, ICharityCategoryRepository charityCategoryRepository)
        {
            _charityRepository = charityRepository;
            _fileStorageService = fileStorageService;
            _charityCategoryRepository = charityCategoryRepository;
        }

        // Delete (Soft Delete)
        public async Task<ApiMessage<bool>> DeleteAsync(int id)
        {
            var result = new ApiMessage<bool>();

            var charity = await _charityRepository.GetByIdAsync(id);

            if (charity == null)
            {
                result.HasError = true;
                result.ErrorMessage = "خیریه یافت نشد";
                return result;
            }

            charity.IsDeleted = true;
            charity.ModifiedAt = DateTime.Now;

            await _charityRepository.SaveChangesAsync();

            result.Value = true;
            return result;
        }

        // Get by id
        public async Task<ApiMessage<CharityDetailsDto>> GetByIdAsync(int id)
        {
            var result = new ApiMessage<CharityDetailsDto>();

            var charity = await _charityRepository.GetByIdAsync(id);

            if (charity == null)
            {
                result.HasError = true;
                result.ErrorMessage = "خیریه یافت نشد";
                return result;
            }

            var categories = await _charityRepository.GetCategoriesByCharityIdAsync(id);

            var dto = new CharityDetailsDto
            {
                Id = charity.Id,
                Name = charity.Name,
                Description = charity.Description,
                Website = charity.Website,
                Address = charity.Address,
                Telephone = charity.Telephone,
                ManagerName = charity.ManagerName,
                ContactName = charity.ContactName,
                ContactPhone = charity.ContactPhone,

                LogoUrl = BuildFileUrl(charity.Logo?.FilePath),
                BannerUrl = BuildFileUrl(charity.Banner?.FilePath),

                Categories = categories,

                Socials = charity.SocialCharities
                .Select(x => new CharitySocialDetailsDto
                {
                    Id = x.Id,
                    SocialId = x.SocialId,
                    SocialName = x.Social.Name,
                    Abbreviation = x.Social.Abbreviation,
                    Value = x.Value
                })
                .ToList(),


                CreatedAt = charity.CreatedAt,
                ModifiedAt = charity.ModifiedAt
            };

            result.Value = dto;
            return result;
        }

        // Get List with Pagination
        public async Task<ApiMessage<PagedResultDto<CharityListItemDto>>> GetListAsync(PaginationRequestDto request)
        {
            var result = new ApiMessage<PagedResultDto<CharityListItemDto>>();

            var (items, totalCount) = await _charityRepository.GetPagedAsync(request.Page, request.PageSize);

            var list = items.Select(x => new CharityListItemDto
            {
                Id = x.Id,
                Name = x.Name,
                Telephone = x.Telephone,
                Website = x.Website,
                CreatedAt = x.CreatedAt
            }).ToList();

            var paged = new PagedResultDto<CharityListItemDto>
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

        public async Task<ApiMessage<bool>> CreateAsync(CreateCharityRequestDto request, int createdByUserId)
        {
            var result = new ApiMessage<bool>();

            var charityName = request.Name?.Trim();

            if (string.IsNullOrWhiteSpace(charityName))
            {
                result.HasError = true;
                result.ErrorMessage = "نام خیریه الزامی است";
                return result;
            }

            if (await _charityRepository.IsNameExistAsync(charityName))
            {
                result.HasError = true;
                result.ErrorMessage = "نام خیریه قبلاً ثبت شده است";
                return result;
            }

            if (request.CategoryIds != null && request.CategoryIds.Count > 5)
            {
                result.HasError = true;
                result.ErrorMessage = "حداکثر ۵ دسته بندی قابل انتخاب است";
                return result;
            }

            var charity = new Charity
            {
                Name = charityName,
                Description = request.Description,
                Website = request.Website,
                Address = request.Address,
                Telephone = request.Telephone,
                ManagerName = request.ManagerName,
                ContactName = request.ContactName,
                ContactPhone = request.ContactPhone,
                CreatedByUserId = createdByUserId,
                CreatedAt = DateTime.Now,
                ModifiedAt = DateTime.Now,
                IsDeleted = false,
                SocialCharities = new List<SocialCharity>()
            };

            // Logo Upload
            if (request.LogoFile != null)
            {
                var logo = await _fileStorageService.SaveLogoAsync(request.LogoFile);
                charity.LogoId = logo.Id;
            }

            // Banner Upload
            if (request.BannerFile != null)
            {
                var banner = await _fileStorageService.SaveBannerAsync(request.BannerFile);
                charity.BannerId = banner.Id;
            }

            // Social Media
            if (request.Socials != null && request.Socials.Any())
            {
                foreach (var socialDto in request.Socials)
                {
                    if (socialDto.SocialId <= 0 || string.IsNullOrWhiteSpace(socialDto.Value))
                    {
                        continue;
                    }

                    var socialCharity = new SocialCharity
                    {
                        SocialId = socialDto.SocialId,
                        Value = socialDto.Value.Trim(),
                    };

                    charity.SocialCharities.Add(socialCharity);
                }
            }

            await _charityRepository.AddAsync(charity);
            await _charityRepository.SaveChangesAsync();

            if (request.CategoryIds != null && request.CategoryIds.Any())
            {
                var categoryIds = request.CategoryIds
                    .Distinct()
                    .Take(5)
                    .ToList();

                var charityCategories = categoryIds
                    .Select(categoryId => new CharityCategory
                    {
                        CharityId = charity.Id,
                        CategoryId = categoryId
                    })
                    .ToList();

                await _charityRepository.AddCharityCategoriesAsync(charityCategories);
            }

            await _charityRepository.SaveChangesAsync();

            result.Value = true;
            return result;
        }

        //public async Task<ApiMessage<bool>> UpdateAsync(UpdateCharityRequestDto request)
        //{
        //    var result = new ApiMessage<bool>();

        //    var charity = await _charityRepository.GetByIdAsync(request.Id);

        //    if (charity == null)
        //    {
        //        result.HasError = true;
        //        result.ErrorMessage = "خیریه مورد نظر یافت نشد";
        //        return result;
        //    }

        //    if (await _charityRepository.IsNameExistAsync(request.Name) && charity.Name != request.Name)
        //    {
        //        result.HasError = true;
        //        result.ErrorMessage = "نام خیریه تکراری است";
        //        return result;
        //    }

        //    charity.Name = request.Name;
        //    charity.Description = request.Description;
        //    charity.Website = request.Website;
        //    charity.Address = request.Address;
        //    charity.Telephone = request.Telephone;
        //    charity.ManagerName = request.ManagerName;
        //    charity.ContactName = request.ContactName;
        //    charity.ContactPhone = request.ContactPhone;
        //    charity.ModifiedAt = DateTime.Now;

        //    System.Diagnostics.Debug.WriteLine($"Name: {charity.Name}");
        //    System.Diagnostics.Debug.WriteLine($"Description: {charity.Description}");
        //    System.Diagnostics.Debug.WriteLine($"Website: {charity.Website}");

        //    await _charityRepository.SaveChangesAsync();

        //    result.Value = true;
        //    return result;
        //}




        public async Task<ApiMessage<bool>> UpdateAsync(UpdateCharityRequestDto request)
        {
            var result = new ApiMessage<bool>();

            var charity = await _charityRepository.GetByIdAsync(request.Id);

            if (charity == null)
            {
                result.HasError = true;
                result.ErrorMessage = "خیریه مورد نظر یافت نشد";
                return result;
            }

            if (await _charityRepository.IsNameExistAsync(request.Name) && charity.Name != request.Name)
            {
                result.HasError = true;
                result.ErrorMessage = "نام خیریه تکراری است";
                return result;
            }

            charity.Name = request.Name;
            charity.Description = request.Description;
            charity.Website = request.Website;
            charity.Address = request.Address;
            charity.Telephone = request.Telephone;
            charity.ManagerName = request.ManagerName;
            charity.ContactName = request.ContactName;
            charity.ContactPhone = request.ContactPhone;
            charity.ModifiedAt = DateTime.Now;

            if (request.LogoFile != null)
            {
                var logo = await _fileStorageService.SaveLogoAsync(request.LogoFile);
                charity.LogoId = logo.Id;
            }

            if (request.BannerFile != null)
            {
                var banner = await _fileStorageService.SaveBannerAsync(request.BannerFile);
                charity.BannerId = banner.Id;
            }


            // دسته‌بندی‌ها
            if (request.CategoryIds != null)
            {
                var oldCategories =
                    await _charityCategoryRepository.GetByCharityIdAsync(charity.Id);

                await _charityCategoryRepository.RemoveRangeAsync(oldCategories);

                foreach (var categoryId in request.CategoryIds.Distinct())
                {
                    await _charityCategoryRepository.AddAsync(
                        new CharityCategory
                        {
                            CharityId = charity.Id,
                            CategoryId = categoryId
                        });
                }
            }

            // شبکه‌های اجتماعی:

            // حذف شبکه‌های اجتماعی قبلی
            await _charityRepository.RemoveSocialsByCharityIdAsync(charity.Id);

            // افزودن شبکه‌های اجتماعی جدید
            if (request.Socials != null && request.Socials.Any())
            {
                foreach (var socialDto in request.Socials)
                {
                    if (socialDto.SocialId <= 0 ||
                        string.IsNullOrWhiteSpace(socialDto.Value))
                    {
                        continue;
                    }

                    await _charityRepository.AddSocialAsync(
                        new SocialCharity
                        {
                            CharityId = charity.Id,
                            SocialId = socialDto.SocialId,
                            Value = socialDto.Value.Trim()
                        });
                }
            }


            await _charityRepository.SaveChangesAsync();


            result.Value = true;
            return result;
        }








        private string? BuildFileUrl(string? filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return null;

            if (filePath.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                filePath.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                return filePath;
            }

            if (filePath.StartsWith("/"))
                return $"{ApiBaseUrl}{filePath}";

            return $"{ApiBaseUrl}/{filePath}";
        }
    }
}
