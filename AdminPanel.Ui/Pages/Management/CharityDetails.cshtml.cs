using System.Net.Http.Headers;
using System.Text.Json;
using AdminPanel.Ui.Models.Common;
using AdminPanel.Application.DTOs.Charity;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminPanel.Ui.Pages.Management
{
    public class CharityDetailsModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CharityDetailsModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public CharityDetailsDto? Charity { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (id <= 0)
                return RedirectToPage("./Charities");

            var client = _httpClientFactory.CreateClient();
            var token = User.FindFirst("AccessToken")?.Value;

            if (!string.IsNullOrWhiteSpace(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            try
            {
                var url = $"https://localhost:7209/api/Charity/{id}";
                var response = await client.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true

                  
                };

                if (!response.IsSuccessStatusCode)
                {
                    return RedirectToPage("./Charities");
                }

                var result = JsonSerializer.Deserialize<ApiMessage<CharityDetailsDto>>(content, options);

                if (result == null || result.HasError || result.Value == null)
                {
                    return RedirectToPage("./Charities");
                }

                Charity = result.Value;
                return Page();
            }
            catch
            {
                return RedirectToPage("./Charities");
            }
        }

    }
}

