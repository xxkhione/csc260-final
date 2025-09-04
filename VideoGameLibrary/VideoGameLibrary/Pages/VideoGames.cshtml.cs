using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using UserData.Models.DTOs;

namespace VideoGameLibrary.Pages
{
    public class VideoGamesModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        [BindProperty]
        public required int DeleteId { get; set; }
        [BindProperty]
        public string? SearchKey { get; set; }
        [BindProperty]
        public string? Genre { get; set; }
        [BindProperty]
        public string? Platform { get; set; }
        [BindProperty]
        public string? ESRBRating { get; set; }

        public List<VideoGameDto> ShownGames { get; set; } = new();

        public VideoGamesModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task OnGetAsync()
        {
            var token = User.FindFirst("Token")?.Value;
            if (string.IsNullOrEmpty(token))
            {
                return;
            }

            var client = _httpClientFactory.CreateClient("UserDataMicroservice");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync($"api/users/videogames");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                ShownGames = JsonSerializer.Deserialize<List<VideoGameDto>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<VideoGameDto>();
            }
            else
            {
                ShownGames = new List<VideoGameDto>();
            }
        }

        public async Task<IActionResult> OnPostFilter()
        {
            var token = User.FindFirst("Token")?.Value;
            if (string.IsNullOrEmpty(token))
            {
                return Page();
            }

            var query = new StringBuilder("?");
            if (!string.IsNullOrEmpty(Genre))
            {
                query.Append($"genre={Uri.EscapeDataString(Genre)}&");
            }
            if (!string.IsNullOrEmpty(Platform))
            {
                query.Append($"platform={Uri.EscapeDataString(Platform)}&");
            }
            if (!string.IsNullOrEmpty(ESRBRating))
            {
                query.Append($"esrbRating={Uri.EscapeDataString(ESRBRating)}&");
            }

            var client = _httpClientFactory.CreateClient("UserDataMicroservice");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync($"api/users/videogames{query.ToString().TrimEnd('&')}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                ShownGames = JsonSerializer.Deserialize<List<VideoGameDto>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<VideoGameDto>();
            }
            else
            {
                ShownGames = new List<VideoGameDto>();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDelete()
        {
            var token = User.FindFirst("Token")?.Value;
            if (string.IsNullOrEmpty(token))
            {
                return Page();
            }

            var client = _httpClientFactory.CreateClient("UserDataMicroservice");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync($"api/users/videogames/{DeleteId}");

            if (!response.IsSuccessStatusCode)
            {
                
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSearch()
        {
            var token = User.FindFirst("Token")?.Value;
            if (string.IsNullOrEmpty(token))
            {
                return Page();
            }

            var queryString = string.IsNullOrEmpty(SearchKey) ? "" : $"?searchQuery={Uri.EscapeDataString(SearchKey)}";
            var client = _httpClientFactory.CreateClient("UserDataMicroservice");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync($"api/users/videogames{queryString}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                ShownGames = JsonSerializer.Deserialize<List<VideoGameDto>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<VideoGameDto>();
            }
            else
            {
                ShownGames = new List<VideoGameDto>();
                ModelState.AddModelError(string.Empty, "Error retrieving games from the microservice.");
            }
            return Page();
        }
    }
}
