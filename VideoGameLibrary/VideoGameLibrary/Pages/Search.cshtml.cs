using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using UserData.Models.DTOs;
using System.Net.Http;

namespace VideoGameLibrary.Pages
{
    public class SearchModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        [BindProperty]
        public string SearchQuery { get; set; }

        public List<VideoGameDto> SearchResults { get; set; } = new();

        public SearchModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public void OnGet()
        {
            
        }

        public async Task<IActionResult> OnPostSearch()
        {
            var client = _httpClientFactory.CreateClient("ExternalGamesAPI");

            var token = User.FindFirst("Token")?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await client.GetAsync($"api/games?searchQuery={Uri.EscapeDataString(SearchQuery)}");

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
                ModelState.AddModelError(string.Empty, "Error retrieving games from the API.");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAddToCollection(int gameId)
        {
            var token = User.FindFirst("Token")?.Value;
            if (string.IsNullOrEmpty(token))
            {
                ModelState.AddModelError(string.Empty, "User not authenticated.");
                return Page();
            }

            var externalGamesClient = _httpClientFactory.CreateClient("ExternalGamesAPI");

            externalGamesClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var getResponse = await externalGamesClient.GetAsync($"api/games/{gameId}");

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

            var userDataClient = _httpClientFactory.CreateClient("UserDataMicroservice");
            userDataClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var postResponse = await userDataClient.PostAsJsonAsync("api/users/videogames", gameDetails);

            if (!postResponse.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Error adding game to your collection.");
            }

            return RedirectToPage("/Search", new { searchQuery = SearchQuery });
        }
    }
}
