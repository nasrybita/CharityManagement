using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

using AdminPanel.Application.Common;
using AdminPanel.Application.DTOs.Charity;
using AdminPanel.Application.DTOs.Common;
using AdminPanel.Core.Enums;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminPanel.Ui.Pages.Management
{
    public class CharitiesModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CharitiesModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public bool CanCreateCharity { get; set; }
        public bool CanEditAllCharities { get; set; }
        public bool CanEditOwnCharityOnly { get; set; }
        public int? CurrentCharityId { get; set; }
        public int? FilterCharityId { get; set; } // ذخیره فیلتر آیدی خیریه

        public void OnGet(int? myCharityId)
        {
            LoadPermissions(myCharityId);
        }

        private void LoadPermissions(int? myCharityId)
        {
            var userTypeValue = User.FindFirst("UserType")?.Value;
            var charityIdValue = User.FindFirst("CharityId")?.Value;

            int.TryParse(userTypeValue, out var role);
            int.TryParse(charityIdValue, out var charityId);

            CurrentCharityId = charityId == 0 ? null : charityId;

            var isAdminSystem = role == (int)AdminUserType.AdminSystem;
            var isCharityAdmin = role == (int)AdminUserType.CharityAdmin;

            CanCreateCharity = isAdminSystem;
            CanEditAllCharities = isAdminSystem;
            CanEditOwnCharityOnly = isCharityAdmin;

            // تنظیم فیلتر آیدی خیریه بر اساس نقش
            if (!isAdminSystem)
            {
                FilterCharityId = CurrentCharityId;
            }
            else
            {
                FilterCharityId = myCharityId;
            }
        }

        private string? GetAccessToken()
        {
            return User.FindFirst("AccessToken")?.Value;
        }

        private string? GetUserType()
        {
            return User.FindFirst("UserType")?.Value;
        }

        private int? GetCurrentCharityId()
        {
            var charityIdValue = User.FindFirst("CharityId")?.Value;
            return int.TryParse(charityIdValue, out var charityId) ? charityId : null;
        }

        private bool IsAdminSystem()
        {
            return GetUserType() == ((int)AdminUserType.AdminSystem).ToString();
        }

        private bool IsCharityAdmin()
        {
            return GetUserType() == ((int)AdminUserType.CharityAdmin).ToString();
        }

        private bool CanManageThisCharity(int charityId)
        {
            if (IsAdminSystem())
                return true;

            if (IsCharityAdmin())
            {
                var currentCharityId = GetCurrentCharityId();
                return currentCharityId.HasValue && currentCharityId.Value == charityId;
            }

            return false;
        }

        public async Task<IActionResult> OnGetAllCharitiesAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();

                var token = GetAccessToken();
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                }

                var url = "https://localhost:7209/api/Charity?Page=1&PageSize=1000";
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return new JsonResult(new { data = new List<CharityListItemDto>() });
                }

                var rawJson = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var result = JsonSerializer.Deserialize<ApiMessage<PagedResultDto<CharityListItemDto>>>(rawJson, options);

                if (result?.Value?.Items == null)
                {
                    return new JsonResult(new { data = new List<CharityListItemDto>() });
                }

                var items = result.Value.Items;

                // اعمال فیلتر بر اساس FilterCharityId تنظیم شده در OnGet
                var userType = GetUserType();
                if (userType != ((int)AdminUserType.AdminSystem).ToString())
                {
                    var currentCharityId = GetCurrentCharityId();
                    items = items
                        .Where(x => currentCharityId.HasValue && x.Id == currentCharityId.Value)
                        .ToList();
                }

                return new JsonResult(new { data = items });
            }
            catch
            {
                return new JsonResult(new { data = new List<CharityListItemDto>() });
            }
        }

        public async Task<IActionResult> OnPostUpdateCharityAsync(UpdateCharityRequestDto request)
        {
            if (!CanManageThisCharity(request.Id))
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return new JsonResult(new
                {
                    success = false,
                    message = "شما دسترسی ویرایش این خیریه را ندارید."
                });
            }

            var client = _httpClientFactory.CreateClient();

            var token = GetAccessToken();
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await client.PutAsJsonAsync("https://localhost:7209/api/Charity", request);

            if (response.IsSuccessStatusCode)
                return new JsonResult(new { success = true });

            var error = await response.Content.ReadAsStringAsync();
            return new JsonResult(new
            {
                success = false,
                message = string.IsNullOrWhiteSpace(error) ? "خطا در بروزرسانی" : error
            });
        }

        //public async Task<IActionResult> OnPostDeleteCharityAsync(int id)
        //{
        //    if (!CanManageThisCharity(id))
        //    {
        //        Response.StatusCode = (int)HttpStatusCode.Forbidden;
        //        return new JsonResult(new
        //        {
        //            success = false,
        //            message = "شما دسترسی حذف این خیریه را ندارید."
        //        });
        //    }

        //    var client = _httpClientFactory.CreateClient();
        //    var token = GetAccessToken();

        //    if (!string.IsNullOrEmpty(token))
        //    {
        //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //    }

        //    try
        //    {
        //        var response = await client.DeleteAsync($"https://localhost:7209/api/Charity/{id}");

        //        if (response.IsSuccessStatusCode)
        //            return new JsonResult(new { success = true });

        //        var errorDetail = await response.Content.ReadAsStringAsync();
        //        return new JsonResult(new { success = false, message = errorDetail });
        //    }
        //    catch (Exception ex)
        //    {
        //        return new JsonResult(new { success = false, message = ex.Message });
        //    }
        //}








        public async Task<IActionResult> OnPostDeleteCharityAsync(int id)
        {
            // ۱. بررسی اینکه کاربر حتماً ادمین سیستم باشد (جلوگیری از حذف توسط CharityAdmin)
            if (!IsAdminSystem())
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return new JsonResult(new
                {
                    success = false,
                    message = "شما دسترسی حذف خیریه را ندارید."
                });
            }


            // ۲. بررسی‌های امنیتی و منطقی دیگر (بدون تغییر)
            if (!CanManageThisCharity(id))
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return new JsonResult(new
                {
                    success = false,
                    message = "شما دسترسی حذف این خیریه را ندارید."
                });
            }

            var client = _httpClientFactory.CreateClient();
            var token = GetAccessToken();

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            try
            {
                var response = await client.DeleteAsync($"https://localhost:7209/api/Charity/{id}");

                if (response.IsSuccessStatusCode)
                    return new JsonResult(new { success = true });

                var errorDetail = await response.Content.ReadAsStringAsync();
                return new JsonResult(new { success = false, message = errorDetail });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

    }
}
