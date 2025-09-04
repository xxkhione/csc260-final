using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using UserData.Models;

namespace UserData.Services
{
    public class VideoGameApiService
    {
        private readonly HttpClient httpClient;
        private readonly string apiKey = "cc2863da04ce49c9aa1742aa46f4fb09";
        private readonly string baseUrl = "https://api.rawg.io/api/";

        public VideoGameApiService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
            this.httpClient.BaseAddress = new Uri(baseUrl);
        }

        public async Task<List<VideoGame>> GetGamesAsync(string searchQuery = null)
        {
            string url = $"games?key={apiKey}";
            if (!string.IsNullOrEmpty(searchQuery))
            {
                url += $"&search={searchQuery}";
            }

            try
            {
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                using JsonDocument json = JsonDocument.Parse(content);
                JsonElement root = json.RootElement;
                JsonElement results = root.GetProperty("results");

                List<VideoGame> games = new List<VideoGame>();
                var tasks = results.EnumerateArray().Select(async gameElement =>
                {
                    int apiGameId = gameElement.GetProperty("id").GetInt32();
                    return await GetGameDetailsAsync(apiGameId);
                });

                games = (await Task.WhenAll(tasks)).ToList();

                return games;
            }
            catch (HttpRequestException ex)
            {
                return new List<VideoGame>();
            }
            catch (JsonException ex)
            {
                return new List<VideoGame>();
            }
        }

        public async Task<VideoGame> GetGameDetailsAsync(int apiGameId)
        {
            string url = $"games/{apiGameId}?key={apiKey}";
            var response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var videoGameResponse = await response.Content.ReadAsStringAsync();
                using JsonDocument json = JsonDocument.Parse(videoGameResponse);
                JsonElement root = json.RootElement;

                return new VideoGame
                {
                    ApiGameId = root.GetProperty("id").GetInt32(),
                    Title = root.GetProperty("name").GetString(),
                    Image = root.GetProperty("background_image").GetString(),
                    Year = root.TryGetProperty("released", out var releasedElement) && releasedElement.ValueKind != JsonValueKind.Null
                            ? DateTime.Parse(releasedElement.GetString()).Year : 0000,
                    Platform = GetPlatform(root),
                    Genre = GetGenre(root),
                    ESRBRating = GetESRBRating(root)
                };
            }
            return null;
        }

        private Platform GetPlatform(JsonElement gameElement)
        {
            if (gameElement.TryGetProperty("platforms", out var platforms))
            {
                if (platforms.ValueKind == JsonValueKind.Array && platforms.GetArrayLength() > 0)
                {
                    if (platforms[0].TryGetProperty("platform", out var platformElement))
                    {
                        if (platformElement.TryGetProperty("name", out var nameElement) && nameElement.ValueKind == JsonValueKind.String)
                        {
                            var platformName = nameElement.GetString();
                            if (Enum.TryParse(platformName.Replace(" ", ""), true, out Platform platform))
                            {
                                return platform;
                            }
                        }
                    }
                }
            }
            return Platform.Unknown;
        }

        private string GetGenre(JsonElement gameElement)
        {
            if (gameElement.TryGetProperty("genres", out var genres))
            {
                if (genres.ValueKind == JsonValueKind.Array && genres.GetArrayLength() > 0)
                {
                    return genres[0].GetProperty("name").GetString();
                }
            }
            return "Unknown";
        }

        private ESRBRating GetESRBRating(JsonElement gameElement)
        {
            if (gameElement.TryGetProperty("esrb_rating", out var rating))
            {
                if (rating.ValueKind != JsonValueKind.Null)
                {
                    var ratingName = rating.GetProperty("name").GetString();
                    if (Enum.TryParse(ratingName.Replace(" ", "_"), true, out ESRBRating esrbRating))
                    {
                        return esrbRating;
                    }
                }
            }
            return ESRBRating.Unknown;
        }
    }
}
