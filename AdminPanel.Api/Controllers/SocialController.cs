using AdminPanel.Application.Common;
using AdminPanel.Application.DTOs.Social;
using AdminPanel.Application.Interfaces;
using AdminPanel.Core.Enums;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanel.Api.Controllers
{
    [Authorize]
     [ApiController]
    [Route("api/[controller]")]
    public class SocialController : ControllerBase
    {
        private readonly ISocialService _socialService;

        public SocialController(ISocialService socialService)
        {
            _socialService = socialService;
        }


        private bool IsSystemAdmin()


        {

            //تست
            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"{claim.Type} = {claim.Value}");
            }

            //تست
            var userType = User.FindFirst("UserType")?.Value;

            return userType ==
                ((int)AdminUserType.AdminSystem).ToString();
        }



        [HttpPost]
        public async Task<ActionResult<ApiMessage<bool>>> Create(CreateSocialRequestDto request)
        {
            if (!IsSystemAdmin())
            {
                return Forbid();
            }


            var result = await _socialService.CreateAsync(request);

            if (result.HasError)
            {
                return BadRequest(result);
            }


            return Ok(result);
        }





        [HttpGet("{id}")]
        public async Task<ActionResult<ApiMessage<SocialDetailsDto>>> GetById(int id)
        {
            var result = await _socialService.GetByIdAsync(id);

            if (result.HasError)
                return BadRequest(result);


            return Ok(result);
        }





        [HttpPut("{id}")]
        public async Task<ActionResult<ApiMessage<bool>>> Update(
            int id,
            UpdateSocialRequestDto dto)
        {
            if (!IsSystemAdmin())
            {
                return Forbid();
            }


            var result = await _socialService.UpdateAsync(id, dto);

            if (result.HasError)
                return BadRequest(result);


            return Ok(result);
        }





        [HttpGet]
        public async Task<ActionResult<ApiMessage<List<SocialListItemDto>>>> GetAll()
        {
            var result = await _socialService.GetAllAsync();

            if (result.HasError)
                return BadRequest(result);


            return Ok(result);
        }





        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiMessage<bool>>> Delete(int id)
        {
            if (!IsSystemAdmin())
            {
                return Forbid();
            }


            var result = await _socialService.DeleteAsync(id);

            if (result.HasError)
                return BadRequest(result);


            return Ok(result);
        }
    }
}