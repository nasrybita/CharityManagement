using System.Security.Claims;

using AdminPanel.Application.Common;
using AdminPanel.Application.DTOs.Charity;
using AdminPanel.Application.DTOs.Common;
using AdminPanel.Application.Interfaces;
using AdminPanel.Application.Services;
using AdminPanel.Core.Enums;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanel.Api.Controllers
{

    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CharityController : ControllerBase
    {
        private readonly ICharityService _charityService;

        public CharityController(ICharityService charityService)
        {
            _charityService = charityService;
        }



        //[HttpPost]
        //public async Task<ActionResult<ApiMessage<bool>>> Create([FromForm] CreateCharityRequestDto request)
        //{

        //    //test
        //    var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        //    if (!int.TryParse(userIdValue, out var userId))
        //    {
        //        return Unauthorized(new { errorMessage = "شناسه کاربر از توکن استخراج نشد." });
        //    }



        //    //
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var result = await _charityService.CreateAsync(request, userId);

        //    if (result.HasError)
        //    {
        //        return BadRequest(result);
        //    }

        //    return Ok(result);
        //}



[HttpPost]
    public async Task<IActionResult> Create([FromForm] CreateCharityRequestDto model)
    {
        var userTypeClaim = User.FindFirst("UserType")?.Value;
        if (string.IsNullOrWhiteSpace(userTypeClaim))
            return Forbid();

        if (userTypeClaim != ((int)AdminUserType.AdminSystem).ToString())
        {
            return StatusCode(403, new
            {
                hasError = true,
                errorMessage = "شما دسترسی ایجاد خیریه را ندارید."
            });
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? User.FindFirst("UserId")?.Value;

        if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out var createdByUserId))
        {
            return Unauthorized(new
            {
                hasError = true,
                errorMessage = "شناسه کاربر معتبر نیست."
            });
        }

        var result = await _charityService.CreateAsync(model, createdByUserId);
        return Ok(result);
    }






    [HttpPut]
        public async Task<ActionResult<ApiMessage<bool>>> Update([FromForm] UpdateCharityRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _charityService.UpdateAsync(request);

            if (result.HasError)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }





        [HttpGet]
        public async Task<ActionResult<ApiMessage<PagedResultDto<CharityListItemDto>>>> GetList([FromQuery] PaginationRequestDto request)
        {
            var result = await _charityService.GetListAsync(request);

            if (result.HasError)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }






        [HttpGet("{id}")]
        public async Task<ActionResult<ApiMessage<CharityDetailsDto>>> GetById(int id)
        {
            var result = await _charityService.GetByIdAsync(id);

            if (result.HasError)
            {
                return NotFound(result);
            }

            return Ok(result);
        }









        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiMessage<bool>>> Delete(int id)
        {
            var result = await _charityService.DeleteAsync(id);

            if (result.HasError)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }





    }
}
