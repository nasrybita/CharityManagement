using AdminPanel.Application.Common;
using AdminPanel.Application.DTOs.Campaign;
using AdminPanel.Application.DTOs.Common;
using AdminPanel.Application.Interfaces;
using AdminPanel.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CampaignStatusEnum = AdminPanel.Core.Enums.CampaignStatus;


namespace AdminPanel.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CampaignController : ControllerBase
    {


        private readonly ICampaignService _campaignService;

        public CampaignController(ICampaignService campaignService)
        {
            _campaignService = campaignService;
        }



        [HttpPost]
        public async Task<ActionResult<ApiMessage<bool>>> Create([FromForm] CreateCampaignRequestDto request)
        {

            // Extract user role and charity ID from the token
            var userRole = GetCurrentUserRole();
            var userCharityId = GetUserCharityId();



            if (userRole == null)
            {
                return Forbid();
            }



            // AdminSystem role is not authorized to create campaigns
            if (userRole == AdminUserType.AdminSystem)
            {
                return Forbid();
            }

            // Only CharityAdmin and CharityUser are allowed
            if (userRole != AdminUserType.CharityAdmin &&
                userRole != AdminUserType.CharityUser)
            {
                return Forbid();
            }

            // Allowed user must have its own charity
            if (!userCharityId.HasValue)
            {
                return Forbid();
            }




            // Get CharityId from user claim not from form
            request.CharityId = userCharityId.Value;





            var result = await _campaignService.CreateAsync(request);


            if (result.HasError)
            {
                return BadRequest(result);
            }

            return Ok(result);



        }







        [HttpPut]
        public async Task<ActionResult<ApiMessage<bool>>> Update([FromForm] UpdateCampaignRequestDto request)
        {


            var userRole = GetCurrentUserRole();
            var userCharityId = GetUserCharityId();


            if (userRole == null)
            {
                return Forbid();
            }



            // System Admin has full control. 
            // CharityAdmin and CharityUser are allowed to edit only their own charity's campaigns.
            if (userRole != AdminUserType.AdminSystem &&
                userRole != AdminUserType.CharityAdmin &&
                userRole != AdminUserType.CharityUser)
            {
                return Forbid();
            }



            // Load the campaign to verify ownership
            var campaignDb = await _campaignService.GetByIdAsync(request.Id);

            if (campaignDb.HasError || campaignDb.Value == null)
            {
                return NotFound(campaignDb);
            }



            // Check access permissions for CharityAdmin and CharityUser
            if (userRole != AdminUserType.AdminSystem)
            {

                if (!userCharityId.HasValue || userCharityId.Value != campaignDb.Value.CharityId)
                {
                    return Forbid();
                }


                // Force the charity ID to belong to the current user's charity
                request.CharityId = userCharityId.Value;

            }
            else
            {
                // If AdminSystem is editing, ensure we preserve or assign the correct Charity ID from the DB
                request.CharityId = campaignDb.Value.CharityId;
            }



            var result = await _campaignService.UpdateAsync(request, userRole.Value);

            if (result.HasError)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
















        [HttpGet]
        public async Task<ActionResult<ApiMessage<PagedResultDto<CampaignListItemDto>>>> GetList(
            [FromQuery] PaginationRequestDto request,
            [FromQuery] int? charityId,
            [FromQuery] int? categoryId,
            [FromQuery] int? campaignStatus)
        {

            var userRole = GetCurrentUserRole();
            var userCharityId = GetUserCharityId();



            if (userRole == null)
            {
                return Forbid();
            }



            // System Admin has full access to all charities and can filter as desired
            if (userRole == AdminUserType.AdminSystem)
            {
                var result = await _campaignService.GetListAsync(request, charityId, categoryId, campaignStatus);

                return Ok(result);
            }



            // CharityAdmin and CharityUser roles can only view campaigns belonging to their own charity
            if (userCharityId.HasValue)
            {
                var result = await _campaignService.GetListAsync(request, userCharityId.Value, categoryId, campaignStatus);
                return Ok(result);
            }

            return Forbid();



        }








        [HttpGet("{id}")]
        public async Task<ActionResult<ApiMessage<CampaignDetailsDto>>> GetById(int id)
        {

            var userRole = GetCurrentUserRole();
            var userCharityId = GetUserCharityId();



            var result = await _campaignService.GetByIdAsync(id);


            if (result.HasError || result.Value == null)
            {
                return NotFound(result);
            }




            //Non - system - admin users are not allowed to view the details of campaigns belonging to other charities
            if (userRole != AdminUserType.AdminSystem)
            {
                if (!userCharityId.HasValue || userCharityId.Value != result.Value.CharityId)
                {
                    return Forbid();
                }
            }

            return Ok(result);



        }











        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiMessage<bool>>> Delete(int id)
        {
            var userRole = GetCurrentUserRole();
            var userCharityId = GetUserCharityId();


            if (userRole == null)
            {
                return Forbid();
            }



            // Check access permissions for CharityAdmin
            if (userRole != AdminUserType.AdminSystem)
            {
                var campaignDb = await _campaignService.GetByIdAsync(id);

                if (campaignDb.HasError || campaignDb.Value == null)
                {
                    return NotFound(campaignDb);
                }

                if (!userCharityId.HasValue || userCharityId.Value != campaignDb.Value.CharityId)
                {
                    return Forbid();
                }
            }



            var result = await _campaignService.DeleteAsync(id, userRole.Value);

            if (result.HasError)
            {
                return BadRequest(result);
            }

            return Ok(result);

        }








        [HttpPatch("{id}/change-status")]
        public async Task<ActionResult<ApiMessage<bool>>> ChangeStatus(int id, [FromBody] ChangeStatusRequestDto request)
        {
            var userRole = GetCurrentUserRole();
            var userCharityId = GetUserCharityId();


            if (userRole == null)
            {
                return Forbid();
            }


            // Only these three roles are allowed
            if (userRole != AdminUserType.AdminSystem &&
                userRole != AdminUserType.CharityAdmin &&
                userRole != AdminUserType.CharityUser)
            {
                return Forbid();
            }



            // Check if Status value is valid or not
            if (!Enum.IsDefined(typeof(CampaignStatusEnum), request.Status))
            {
                return BadRequest(new ApiMessage<bool>
                {
                    HasError = true,
                    ErrorMessage = "وضعیت کمپین معتبر نیست"
                });
            }


            
            //Recieve campaign to check its ownership
            var campaignResult = await _campaignService.GetByIdAsync(id);

            if (campaignResult.HasError || campaignResult.Value == null)
            {
                return NotFound(campaignResult);
            }




            //Charityusers are allowed only to manage their own charity campaigns
            if (userRole != AdminUserType.AdminSystem)
            {
                if (!userCharityId.HasValue ||
                    campaignResult.Value.CharityId != userCharityId.Value)
                {
                    return Forbid();
                }
            }



            var result = await _campaignService.ChangeStatusAsync(id, (CampaignStatusEnum)request.Status, userRole.Value);

            if (result.HasError)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }











        #region Helper Methods


        private AdminUserType? GetCurrentUserRole()
        {
            var userTypeClaim = User.FindFirst("UserType")?.Value;

            if (int.TryParse(userTypeClaim, out var roleId) && Enum.IsDefined(typeof(AdminUserType), roleId))
            {
                return (AdminUserType)roleId;
            }

            return null;
        }


        private int? GetUserCharityId()
        {
            var userCharityIdClaim = User.FindFirst("CharityId")?.Value;

            if (int.TryParse(userCharityIdClaim, out var charityId))
            {
                return charityId;
            }

            return null;
        }


        #endregion












    }
}
