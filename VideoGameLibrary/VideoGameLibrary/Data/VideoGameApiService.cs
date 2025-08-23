using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using VideoGameLibrary.Models;

namespace VideoGameLibrary.Data
{
    public class VideoGameApiService
    {
        private readonly HttpClient httpClient;
        private readonly string apiKey = "cc2863da04ce49c9aa1742aa46f4fb09";
        private readonly string baseUrl = "https://api.rawg.io/api/";
        private readonly LoginContext context;

        public VideoGameApiService(HttpClient httpClient, LoginContext context)
        {
            this.httpClient = httpClient;
            this.httpClient.BaseAddress = new Uri(baseUrl);
            this.context = context;
        }

        public async Task<List<VideoGame>> GetGamesAsync(string searchQuery = null)
        {
            string url = $"games?key={apiKey}";
            if(!string.IsNullOrEmpty(searchQuery))
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
                var semaphore = new SemaphoreSlim(2); //Help from Gemini to not overwhelm the API while searching and using another API call
                var tasks = new List<Task<VideoGame>>();

                foreach (var gameElement in results.EnumerateArray())
                {
                    await semaphore.WaitAsync();
                    int apiGameId = gameElement.GetProperty("id").GetInt32();
                    var task = GetGameDetailsAsync(apiGameId);
                    tasks.Add(task.ContinueWith(t =>
                    {
                        semaphore.Release();
                        return t.Result;
                    }));
                }

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

                VideoGame game = new VideoGame
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
                return game;
            }
            return null;
        }

        public async Task<VideoGame> GetGameDetailsForAddAsync(string userId, int apiGameId)
        {
            string url = $"games/{apiGameId}?key={apiKey}";
            var response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var videoGameResponse = await response.Content.ReadAsStringAsync();
                using JsonDocument json = JsonDocument.Parse(videoGameResponse);
                JsonElement root = json.RootElement;

                VideoGame game = new VideoGame
                {
                    ApiGameId = root.GetProperty("id").GetInt32(),
                    Title = root.GetProperty("name").GetString(),
                    Image = root.GetProperty("background_image").GetString(),
                    Year = root.TryGetProperty("released", out var releasedElement) && releasedElement.ValueKind != JsonValueKind.Null
                            ? DateTime.Parse(releasedElement.GetString()).Year : 0000,
                    Platform = GetPlatform(root),
                    Genre = GetGenre(root),
                    ESRBRating = GetESRBRating(root),
                    UserId = userId,
                };
                return game;
            }
            return null;
        }

        public async Task<WishListGame> GetWishListGameDetailsForAddAsync(string userId, int apiGameId)
        {
            string url = $"games/{apiGameId}?key={apiKey}";
            var response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var videoGameResponse = await response.Content.ReadAsStringAsync();
                using JsonDocument json = JsonDocument.Parse(videoGameResponse);
                JsonElement root = json.RootElement;

                WishListGame game = new WishListGame
                {
                    ApiGameId = root.GetProperty("id").GetInt32(),
                    Title = root.GetProperty("name").GetString(),
                    Image = root.GetProperty("background_image").GetString(),
                    Year = root.TryGetProperty("released", out var releasedElement) && releasedElement.ValueKind != JsonValueKind.Null
                            ? DateTime.Parse(releasedElement.GetString()).Year : 0000,
                    Platform = GetPlatform(root),
                    Genre = GetGenre(root),
                    ESRBRating = GetESRBRating(root),
                    UserId = userId,
                };
                return game;
            }
            return null;
        }

        public async Task<VideoGame> GetGameFromDatabase(string userId, int apiGameId)
        {
            VideoGame game = context.VideoGames.FirstOrDefault(g => g.ApiGameId == apiGameId);
            if(game == null)
            {
                game = await GetGameDetailsForAddAsync(userId, apiGameId);
                if(game != null)
                {
                    context.VideoGames.Add(game);
                    context.SaveChanges();
                }
            }
            return game;
        }

        public async Task<WishListGame> GetWishGameFromDatabase(string userId, int apiGameId)
        {
            WishListGame game = context.WishList.FirstOrDefault(g => g.ApiGameId == apiGameId);
            if (game == null)
            {
                game = await GetWishListGameDetailsForAddAsync(userId, apiGameId);
                if (game != null)
                {
                    context.WishList.Add(game);
                    context.SaveChanges();
                }
            }
            return game;
        }

        private Platform GetPlatform(JsonElement gameElement)
        {
            if (gameElement.TryGetProperty("parent_platforms", out var platforms))
            {
                if (platforms.ValueKind == JsonValueKind.Array && platforms.GetArrayLength() > 0)
                {
                    var platformName = platforms[0].GetProperty("platform").GetProperty("name").GetString();
                    if (Enum.TryParse(platformName.Replace(" ", ""), true, out Platform platform))
                    {
                        return platform;
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
                if(rating.ValueKind != JsonValueKind.Null)
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
