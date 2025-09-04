using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using UserData.Models;
using UserData.Models.DTOs;

namespace VideoGameLibrary.Pages
{
    public class VideoGamesModel : PageModel
    {
        private readonly HttpClient _httpClient;
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

        public VideoGamesModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri(_configuration["UserDataMicroservice:BaseUrl"]);
        }

        private void AddAuthorizationHeader()
        {
            var token = HttpContext.Request.Cookies["AuthToken"];
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task OnGetAsync()
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync("api/users/videogames");
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
            AddAuthorizationHeader();

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

            var response = await _httpClient.GetAsync($"api/users/videogames{query.ToString().TrimEnd('&')}");

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
            AddAuthorizationHeader();

            var response = await _httpClient.DeleteAsync($"api/users/videogames/{DeleteId}");

            if (!response.IsSuccessStatusCode)
            {
                
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSearch()
        {
            AddAuthorizationHeader();

            var queryString = string.IsNullOrEmpty(SearchKey) ? "" : $"?searchQuery={Uri.EscapeDataString(SearchKey)}";
            var response = await _httpClient.GetAsync($"api/users/videogames{queryString}");

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
