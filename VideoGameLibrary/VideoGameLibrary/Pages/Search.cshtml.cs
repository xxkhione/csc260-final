using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using UserData.Models.DTOs;

namespace VideoGameLibrary.Pages
{
    public class SearchModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        [BindProperty]
        public string SearchQuery { get; set; }

        public List<VideoGameDto> SearchResults { get; set; } = new();

        public SearchModel(HttpClient httpClient, IConfiguration configuration)
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

        public void OnGet()
        {
            
        }

        public async Task<IActionResult> OnPostSearch()
        {
            if (string.IsNullOrEmpty(SearchQuery))
            {
                return Page();
            }

            var response = await _httpClient.GetAsync($"api/games?searchQuery={Uri.EscapeDataString(SearchQuery)}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                SearchResults = JsonSerializer.Deserialize<List<VideoGameDto>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<VideoGameDto>();
            }
            else
            {
                SearchResults = new List<VideoGameDto>();
                ModelState.AddModelError(string.Empty, "Error retrieving games from the external API.");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAddToCollection(int gameId)
        {
            AddAuthorizationHeader();
            var getResponse = await _httpClient.GetAsync($"api/games/{gameId}");

            if (!getResponse.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Game not found in external API.");
                return Page();
            }

            var gameDetails = JsonSerializer.Deserialize<VideoGameDto>(
                await getResponse.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (gameDetails == null)
            {
                ModelState.AddModelError(string.Empty, "Failed to retrieve game details.");
                return Page();
            }

            var userDataClient = new HttpClient();
            userDataClient.BaseAddress = new Uri(_configuration["UserDataMicroservice:BaseUrl"]);
            AddAuthorizationHeader();

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(gameDetails),
                Encoding.UTF8,
                "application/json"
            );

            var postResponse = await userDataClient.PostAsync("api/users/videogames", jsonContent);

            if (!postResponse.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Error adding game to your collection.");
            }

            return RedirectToPage("/Search", new { searchQuery = SearchQuery });
        }
    }
}
