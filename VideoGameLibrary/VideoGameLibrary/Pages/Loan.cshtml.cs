using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using UserData.Models.DTOs;

namespace VideoGameLibrary.Pages
{
    public class LoanModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        [BindProperty]
        public required int LoanGameId { get; set; }
        [BindProperty]
        public required string PersonLoaningData { get; set; }
        public List<VideoGameDto> videoGames { get; set; } = new();

        public LoanModel(HttpClient httpClient, IConfiguration configuration)
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

            if(response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                videoGames = JsonSerializer.Deserialize<List<VideoGameDto>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<VideoGameDto>();
            }
        }

        public async Task<IActionResult> OnPost()
        {
            AddAuthorizationHeader();

            var getResponse = await _httpClient.GetAsync($"api/users/videogames/{LoanGameId}");
            if (!getResponse.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Game not found in your collection.");
                return Page();
            }

            var existingGame = JsonSerializer.Deserialize<VideoGameDto>(
                await getResponse.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { 
                    PropertyNameCaseInsensitive = true 
                });

            if (existingGame == null)
            {
                ModelState.AddModelError(string.Empty, "Failed to retrieve existing game details.");
                return Page();
            }

            existingGame.LoanedTo = PersonLoaningData;
            existingGame.LoanedDate = DateTime.Now;

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(existingGame),
                Encoding.UTF8,
                "application/json"
            );

            var putResponse = await _httpClient.PutAsync($"api/users/videogames/{LoanGameId}", jsonContent);

            if (!putResponse.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Error loaning the game.");
                return Page();
            }

            return RedirectToPage("/VideoGames");
        }

        public async Task<IActionResult> OnPostReturn()
        {
            AddAuthorizationHeader();

            var getResponse = await _httpClient.GetAsync($"api/users/videogames/{LoanGameId}");
            if (!getResponse.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Game not found in your collection.");
                return Page();
            }

            var existingGame = JsonSerializer.Deserialize<VideoGameDto>(
                await getResponse.Content.ReadAsStringAsync(),
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (existingGame == null)
            {
                ModelState.AddModelError(string.Empty, "Failed to retrieve existing game details.");
                return Page();
            }

            existingGame.LoanedTo = null;
            existingGame.LoanedDate = null;

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(existingGame),
                Encoding.UTF8,
                "application/json"
            );

            var putResponse = await _httpClient.PutAsync($"api/users/videogames/{LoanGameId}", jsonContent);

            if (!putResponse.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Error loaning the game.");
                return Page();
            }

            return RedirectToPage("/VideoGames");
        }
    }
}
