using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AdminPanel.Application.DTOs.Charity;
using AdminPanel.Core.Enums; // اگر AdminUserType اینجاست

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminPanel.Ui.Pages.Management
{
    public class CreateCharityModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CreateCharityModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public CreateCharityRequestDto Charity { get; set; } = new();

        public IActionResult OnGet()
        {
            var userTypeClaim = User.FindFirst("UserType")?.Value;

            if (string.IsNullOrWhiteSpace(userTypeClaim))
                return Forbid();

            if (userTypeClaim != ((int)AdminUserType.AdminSystem).ToString())
                return Forbid();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userTypeClaim = User.FindFirst("UserType")?.Value;

            if (string.IsNullOrWhiteSpace(userTypeClaim))
                return Forbid();

            if (userTypeClaim != ((int)AdminUserType.AdminSystem).ToString())
                return Forbid();

            if (!ModelState.IsValid)
                return Page();

            try
            {
                var client = _httpClientFactory.CreateClient();

                var token = User.FindFirst("AccessToken")?.Value;
                if (!string.IsNullOrWhiteSpace(token))
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);

                using var content = new MultipartFormDataContent();

                // فیلدهای متنی
                content.Add(new StringContent(Charity.Name ?? string.Empty), "Name");
                content.Add(new StringContent(Charity.Description ?? string.Empty), "Description");
                content.Add(new StringContent(Charity.Website ?? string.Empty), "Website");
                content.Add(new StringContent(Charity.Address ?? string.Empty), "Address");
                content.Add(new StringContent(Charity.Telephone ?? string.Empty), "Telephone");
                content.Add(new StringContent(Charity.ManagerName ?? string.Empty), "ManagerName");
                content.Add(new StringContent(Charity.ContactName ?? string.Empty), "ContactName");
                content.Add(new StringContent(Charity.ContactPhone ?? string.Empty), "ContactPhone");

                // اگر DTO این‌ها را دارد، این بخش را نگه دار
                // در غیر این صورت حذفش کن
                if (Charity.CategoryIds != null)
                {
                    foreach (var categoryId in Charity.CategoryIds)
                    {
                        content.Add(new StringContent(categoryId.ToString()), "CategoryIds");
                    }
                }

                // اگر Socials در DTO وجود دارد
                // مثلا Socials[0].SocialId و Socials[0].Value
                if (Charity.Socials != null)
                {
                    for (int i = 0; i < Charity.Socials.Count; i++)
                    {
                        var social = Charity.Socials[i];
                        content.Add(new StringContent(social.SocialId.ToString()), $"Socials[{i}].SocialId");
                        content.Add(new StringContent(social.Value ?? string.Empty), $"Socials[{i}].Value");
                    }
                }

                // فایل لوگو
                if (Charity.LogoFile != null)
                {
                    var logoContent = new StreamContent(Charity.LogoFile.OpenReadStream());
                    logoContent.Headers.ContentType =
                        new MediaTypeHeaderValue(Charity.LogoFile.ContentType);
                    content.Add(logoContent, "LogoFile", Charity.LogoFile.FileName);
                }

                // فایل بنر
                if (Charity.BannerFile != null)
                {
                    var bannerContent = new StreamContent(Charity.BannerFile.OpenReadStream());
                    bannerContent.Headers.ContentType =
                        new MediaTypeHeaderValue(Charity.BannerFile.ContentType);
                    content.Add(bannerContent, "BannerFile", Charity.BannerFile.FileName);
                }

                var response = await client.PostAsync("https://localhost:7209/api/Charity", content);

                if (response.IsSuccessStatusCode)
                    return RedirectToPage("./Charities");

                if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    ModelState.AddModelError(string.Empty, "شما دسترسی ایجاد خیریه را ندارید.");
                    return Page();
                }

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    ModelState.AddModelError(string.Empty, "نشست کاربری شما معتبر نیست. لطفاً دوباره وارد شوید.");
                    return Page();
                }

                var serverMessage = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"خطا در ثبت خیریه. {serverMessage}");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"خطا در ثبت اطلاعات: {ex.Message}");
            }

            return Page();
        }
    }
}
