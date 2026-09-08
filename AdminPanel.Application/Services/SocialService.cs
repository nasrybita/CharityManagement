using AdminPanel.Application.Common;
using AdminPanel.Application.DTOs.Social;
using AdminPanel.Application.Interfaces;
using AdminPanel.Infrastructure.Persistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Application.Services
{
    public class SocialService : ISocialService
    {

        private readonly ISocialRepository _socialRepository;


        public SocialService(ISocialRepository socialRepository)
        {

            _socialRepository = socialRepository;
        }


        public async Task<ApiMessage<bool>> CreateAsync(CreateSocialRequestDto request)
        {
            var result = new ApiMessage<bool>();

            if (await _socialRepository.IsNameExistAsync(request.Name))
            {
                result.HasError = true;
                result.ErrorMessage = "نام شبکه اجتماعی، قبلا ثبت شده است.";
                return result;
            }

            var social = new Social
            {
                Name = request.Name,
                Abbreviation = request.Abbreviation,
                CreatedAt = DateTime.Now,
                ModifiedAt = DateTime.Now,
                IsDeleted = false
            };

            await _socialRepository.AddAsync(social);
            await _socialRepository.SaveChangesAsync();

            result.Value = true;

            return result;
        }



        public async Task<ApiMessage<List<SocialListItemDto>>> GetAllAsync()
        {
            var result = new ApiMessage<List<SocialListItemDto>>();

            var socials = await _socialRepository.GetAllAsync();

            var list = socials.Select(x => new SocialListItemDto
            {
                Id = x.Id,
                Name = x.Name,
                Abbreviation = x.Abbreviation,
                CreatedAt = x.CreatedAt
            }).ToList();

            result.Value = list;

            return result;
        }




        public async Task<ApiMessage<SocialDetailsDto>> GetByIdAsync(int id)
        {
            var social = await _socialRepository.GetByIdAsync(id);

            if (social == null)
            {
                return new ApiMessage<SocialDetailsDto>
                {
                    HasError = true,
                    ErrorMessage = "شبکه اجتماعی مورد نظر یافت نشد"
                };
            }

            return new ApiMessage<SocialDetailsDto>
            {
                HasError = false,
                Value = new SocialDetailsDto
                {
                    Id = social.Id,
                    Name = social.Name,
                    Abbreviation = social.Abbreviation,
                    CreatedAt = social.CreatedAt, 
                    ModifiedAt = social.ModifiedAt
                }
            };
        }


        public async Task<ApiMessage<bool>> UpdateAsync(int id, UpdateSocialRequestDto dto)
        {
            var social = await _socialRepository.GetByIdAsync(id);


            if (social == null)
            {
                return new ApiMessage<bool>
                {
                    HasError = true,
                    ErrorMessage = "شبکه اجتماعی برای ویرایش پیدا نشد"
                };

            }

            social.Name = dto.Name;
            social.Abbreviation = dto.Abbreviation;
            social.ModifiedAt = DateTime.Now;

            await _socialRepository.UpdateAsync(social);
            await _socialRepository.SaveChangesAsync();

            return new ApiMessage<bool>
            {
                HasError = false,
                Value = true
            };

        }







        public async Task<ApiMessage<bool>> DeleteAsync(int id)
        {
            var social = await _socialRepository.GetByIdAsync(id);


            if (social == null)
            {
                return new ApiMessage<bool>
                {
                    HasError = true,
                    ErrorMessage = "شبکه اجتماعی برای حذف پیدا نشد"
                };
            }


            await _socialRepository.DeleteAsync(social);
            await _socialRepository.SaveChangesAsync();


            return new ApiMessage<bool>
            {
                HasError = false,
                Value = true
            };

        }

    }
}
