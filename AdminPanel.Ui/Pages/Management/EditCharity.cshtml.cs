using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

using AdminPanel.Application.Common;
using AdminPanel.Application.DTOs.Category;
using AdminPanel.Application.DTOs.Charity;
using AdminPanel.Application.DTOs.Common;
using AdminPanel.Application.DTOs.Social;
using AdminPanel.Application.ViewModel;
using AdminPanel.Core.Enums;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminPanel.Ui.Pages.Management
{
    public class EditCharityModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EditCharityModel(
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        // استفاده از ViewModel جدید برای Binding
        [BindProperty]
        public EditCharityViewModel Input { get; set; } = new();

        public CharityDetailsDto CurrentCharity { get; set; } = new();
        public List<CategoryListItemDto> AllCategories { get; set; } = new();
        public List<SocialDetailsDto> SocialTypes { get; set; } = new();

        [BindProperty]
        public bool RemoveLogo { get; set; }

        [BindProperty]
        public bool RemoveBanner { get; set; }

        public string? CurrentLogoUrl { get; set; }
        public string? CurrentBannerUrl { get; set; }

     



        public async Task<IActionResult> OnGetAsync(int id)
        {
            var userType = User.FindFirst("UserType")?.Value;
            var charityIdClaim = User.FindFirst("CharityId")?.Value;

            if (!int.TryParse(userType, out var role))
                return Unauthorized();

            if (role != (int)AdminUserType.AdminSystem)
            {
                if (!int.TryParse(charityIdClaim, out var userCharityId))
                    return Forbid();

                if (id != userCharityId)
                    return Forbid();
            }

            var client = CreateAuthorizedClient();
            await LoadLookupsAsync(client);

            var response = await client.GetAsync($"https://localhost:7209/api/Charity/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiMessage<CharityDetailsDto>>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result?.Value == null)
                return NotFound();

            CurrentCharity = result.Value;
            CurrentLogoUrl = CurrentCharity.LogoUrl;
            CurrentBannerUrl = CurrentCharity.BannerUrl;

            Input = new EditCharityViewModel
            {
                Id = CurrentCharity.Id,
                Name = CurrentCharity.Name,
                Website = CurrentCharity.Website,
                Description = CurrentCharity.Description,
                Telephone = CurrentCharity.Telephone,
                Address = CurrentCharity.Address,
                ManagerName = CurrentCharity.ManagerName,
                ContactName = CurrentCharity.ContactName,
                ContactPhone = CurrentCharity.ContactPhone,
                CategoryIds = CurrentCharity.Categories?.Select(x => x.Id).ToList() ?? new List<int>(),
                SocialMedias = CurrentCharity.Socials?.Select(x => new CharitySocialMediaViewModel
                {
                    Id = x.Id,
                    SocialId = x.SocialId,
                    SocialName = x.SocialName,
                    Value = x.Value,
                    IconUrl = x.IconUrl
                }).ToList() ?? new List<CharitySocialMediaViewModel>()
            };

            return Page();
        }











        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                var client = CreateAuthorizedClient();
                await LoadLookupsAsync(client);
                return Page();
            }

            try
            {
                var client = CreateAuthorizedClient();
                using var content = new MultipartFormDataContent();

                content.Add(new StringContent(Input.Id.ToString()), "Id");
                content.Add(new StringContent(Input.Name ?? ""), "Name");
                content.Add(new StringContent(Input.Description ?? ""), "Description");
                content.Add(new StringContent(Input.Website ?? ""), "Website");
                content.Add(new StringContent(Input.Telephone ?? ""), "Telephone");
                content.Add(new StringContent(Input.Address ?? ""), "Address");
                content.Add(new StringContent(Input.ManagerName ?? ""), "ManagerName");
                content.Add(new StringContent(Input.ContactName ?? ""), "ContactName");
                content.Add(new StringContent(Input.ContactPhone ?? ""), "ContactPhone");

                // ارسال دسته‌بندی‌ها
                if (Input.CategoryIds != null)
                {
                    foreach (var categoryId in Input.CategoryIds)
                    {
                        content.Add(new StringContent(categoryId.ToString()), "CategoryIds");
                    }
                }

                // ارسال شبکه‌های اجتماعی
                if (Input.SocialMedias != null)
                {
                    //for (int i = 0; i < Input.SocialMedias.Count; i++)
                    //{
                    //    content.Add(new StringContent(Input.SocialMedias[i].SocialId.ToString() ?? "0"), $"Socials[{i}].SocialId");
                    //    content.Add(new StringContent(Input.SocialMedias[i].Value ?? ""), $"Socials[{i}].Value");
                    //    // اگر نیاز است آیدی اصلی هم ارسال شود (برای ویرایش رکوردهای موجود)
                    //    if (Input.SocialMedias[i].Id.HasValue)
                    //    {
                    //        content.Add(new StringContent(Input.SocialMedias[i].Id.Value.ToString()), $"Socials[{i}].Id");
                    //    }
                    //}



                    for (int i = 0; i < Input.SocialMedias.Count; i++)
                    {
                        var social = Input.SocialMedias[i];

                        content.Add(
     new StringContent(
         (Input.SocialMedias[i].SocialId ?? 0).ToString()
     ),
     $"Socials[{i}].SocialId"
 );


                        content.Add(
                            new StringContent(
                                Input.SocialMedias[i].SocialName ?? ""
                            ),
                            $"Socials[{i}].SocialName"
                        );


                        content.Add(
                            new StringContent(
                                Input.SocialMedias[i].Value ?? ""
                            ),
                            $"Socials[{i}].Value"
                        );


                        if (social.Id.HasValue)
                        {
                            content.Add(
                                new StringContent(social.Id.Value.ToString()),
                                $"Socials[{i}].Id");
                        }
                    }
                }

                content.Add(new StringContent(RemoveLogo.ToString()), "RemoveLogo");
                content.Add(new StringContent(RemoveBanner.ToString()), "RemoveBanner");

                // اگر فایلی انتخاب شده باشد
                if (Input.LogoFile != null)
                {
                    var streamContent = new StreamContent(Input.LogoFile.OpenReadStream());
                    streamContent.Headers.ContentType = new MediaTypeHeaderValue(Input.LogoFile.ContentType);
                    content.Add(streamContent, "LogoFile", Input.LogoFile.FileName);
                }

                if (Input.BannerFile != null)
                {
                    var streamContent = new StreamContent(Input.BannerFile.OpenReadStream());
                    streamContent.Headers.ContentType = new MediaTypeHeaderValue(Input.BannerFile.ContentType);
                    content.Add(streamContent, "BannerFile", Input.BannerFile.FileName);
                }

                var response = await client.PutAsync("https://localhost:7209/api/Charity", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("./Charities");
                }

                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", error);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"خطا در ذخیره‌سازی: {ex.Message}");
            }

            // لود مجدد Lookupها برای بازگشت به صفحه در صورت خطا
            var clientReload = CreateAuthorizedClient();
            await LoadLookupsAsync(clientReload);
            return Page();
        }

        private HttpClient CreateAuthorizedClient()
        {
            var client = _httpClientFactory.CreateClient();
            var token = User.FindFirst("AccessToken")?.Value;
            if (!string.IsNullOrWhiteSpace(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            return client;
        }




        private async Task LoadLookupsAsync(HttpClient client)
        {
            try
            {
                // =========================
                // Categories
                // =========================

                var categoryResponse = await client.GetAsync(
                    "https://localhost:7209/api/Category"
                );


                if (categoryResponse.IsSuccessStatusCode)
                {
                    var catResult =
                        await categoryResponse.Content
                        .ReadFromJsonAsync<
                            ApiMessage<PagedResultDto<CategoryListItemDto>>
                        >();


                    if (catResult?.Value != null)
                    {
                        AllCategories = catResult.Value.Items;
                    }
                }



                // =========================
                // Socials
                // =========================

                var socialResponse = await client.GetAsync(
                    "https://localhost:7209/api/Social"
                );


                if (socialResponse.IsSuccessStatusCode)
                {

                    var socialResult =
                        await socialResponse.Content
                        .ReadFromJsonAsync<
                            ApiMessage<List<SocialListItemDto>>
                        >();



                    if (socialResult?.Value != null)
                    {
                        SocialTypes = socialResult.Value
                            .Select(x => new SocialDetailsDto
                            {
                                Id = x.Id,
                                Name = x.Name,
                                Abbreviation = x.Abbreviation,
                                CreatedAt = x.CreatedAt
                            })
                            .ToList();
                    }

                }

            }
            catch (Exception ex)
            {
                ModelState.AddModelError(
                    "",
                    $"خطا در بارگذاری اطلاعات پایه: {ex.Message}"
                );
            }
        }
    }
}
