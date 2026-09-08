using AdminPanel.Application.Common;
using AdminPanel.Application.DTOs.Charity;
using AdminPanel.Ui.Services;
using AdminPanel.Ui.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;


namespace AdminPanel.Ui.Pages.Account
{
    public class LoginModel : PageModel
    {

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly UserAccessService _userAccessService;

        public LoginModel(IHttpClientFactory httpClientFactory, UserAccessService userAccessService)
        {
            _httpClientFactory = httpClientFactory;
            _userAccessService = userAccessService;
        }



        [BindProperty]
        public LoginRequestViewModel Input { get; set; } = new();


        public IActionResult OnGet()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Index");
            }

            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
                

            var client = _httpClientFactory.CreateClient();

            var request = new
            {
                UserName = Input.UserName,
                Password = Input.Password
            };

            var json = JsonSerializer.Serialize(request);

            var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json"
            );

            var response = await client.PostAsync(
                "https://localhost:7209/api/Auth/login",
                content
            );

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "لاگین ناموفق");
                return Page();
            }

            var result = await response.Content.ReadAsStringAsync();
            

            var loginResponse = JsonSerializer.Deserialize<LoginResponseViewModel>(result,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });


            if (loginResponse == null || loginResponse.HasError || loginResponse.Value == null)
            {
                ModelState.AddModelError("", loginResponse?.ErrorMessage ?? "خطا در ورود");
                return Page();
            }



            var val = loginResponse.Value;


            _userAccessService.SetCurrentUser(val);


            // Create Initial Object from merged model
            //var userContext = new UserContextModel
            //{
            //    UserId = val.UserId,
            //    UserName = val.UserName,
            //    FullName = val.Name,
            //    UserType = val.UserType,
            //    CharityId = val.CharityId,
            //    Token = val.Token
            //};







            //If the User is connected to a specific charity, we recieve and merge charity information
            if (val.CharityId.HasValue)
            {
                try
                {
                    var charityClient = _httpClientFactory.CreateClient();
                    // هدر توکن احراز هویت را برای درخواست بعدی ست می‌کنیم
                    charityClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", val.Token);

                    var charityResponse = await charityClient.GetAsync($"https://localhost:7209/api/Charity/{val.CharityId.Value}");

                    if (charityResponse.IsSuccessStatusCode)
                    {
                        var charityResultJson = await charityResponse.Content.ReadAsStringAsync();
                        // Serializer for ApiMessage<CharityDetailsDto>
                        var charityData = JsonSerializer.Deserialize<ApiMessage<CharityDetailsDto>>(charityResultJson,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        if (charityData != null && !charityData.HasError && charityData.Value != null)
                        {
                            _userAccessService.SetCurrentCharity(charityData.Value);

                        }
                    }
                }
                catch
                {
                    //In case of network error in recieving charity information, login won't get cancelled
                }
            }




            //Convert merged user model into  JSON format in order to save Claim
            var serializedUser = JsonSerializer.Serialize(loginResponse.Value);


            //var claims = new List<Claim>
            //{
            //    new Claim(ClaimTypes.Name, val.UserName),
            //    new Claim("FullName", val.Name),
            //    new Claim("AccessToken", val.Token),


            //    //adding charityid and usertype for save in cookie
            //    new Claim("UserType", ((int)val.UserType).ToString()),
            //    new Claim("CharityId", val.CharityId?.ToString() ?? ""),
            //    new Claim("UserData", serializedUser) //All the data will be saved here
            //};


            var claims = new List<Claim>
{
    new Claim(
        ClaimTypes.NameIdentifier,
        val.UserId.ToString()
    ),

    new Claim(
        ClaimTypes.Name,
        val.UserName
    ),

    new Claim("FullName", val.Name),

    new Claim("AccessToken", val.Token),

    new Claim(
        "UserType",
        ((int)val.UserType).ToString()
    ),

    new Claim(
        "CharityId",
        val.CharityId?.ToString() ?? ""
    ),

    new Claim(
        "UserData",
        serializedUser
    )
};


            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal
            );

            return RedirectToPage("/Index");
        }







    }
}
