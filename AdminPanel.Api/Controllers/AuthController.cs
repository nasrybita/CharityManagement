using AdminPanel.Application.Common;
using AdminPanel.Application.DTOs.Auth;
using AdminPanel.Application.Interfaces;
using AdminPanel.Core.Enums;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanel.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }



        [HttpPost("login")]
        public async Task<ActionResult<ApiMessage<LoginResponseDto>>> Login([FromBody] LoginRequestDto request)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.LoginAsync(request);

            if (result.HasError)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }






        [Authorize] // User must be logged in to be able to create a new user
        [HttpPost("register")]
        public async Task<ActionResult<ApiMessage<bool>>> Register([FromBody] RegisterRequestDto request)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            // Extract current user information from token
            var userTypeClaim = User.FindFirst("UserType")?.Value;
            var charityIdClaim = User.FindFirst("CharityId")?.Value;

            if (string.IsNullOrEmpty(userTypeClaim) || !Enum.TryParse<AdminUserType>(userTypeClaim, out var creatorUserType))
            {
                return Unauthorized(new ApiMessage<bool>
                {

                    HasError = true,
                    ErrorMessage = "اعتبارسنجی نقش کاربری با خطا مواجه شد."

                });
            }

            int? creatorCharityId = null;
            if (int.TryParse(charityIdClaim, out var parsedCharityId))
            {
                creatorCharityId = parsedCharityId;
            }




            // Register a new user based on the request data and the creator's role and charity scope.
            var result = await _authService.RegisterAsync(request, creatorUserType, creatorCharityId);

            if (result.HasError)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }



    }
}
