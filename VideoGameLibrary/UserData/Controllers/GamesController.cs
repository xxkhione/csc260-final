using Microsoft.AspNetCore.Mvc;
using UserData.Services;

[ApiController]
[Route("api/games")]
public class GamesController : ControllerBase
{
    private readonly VideoGameApiService _videoGameApiService;

    public GamesController(VideoGameApiService videoGameApiService)
    {
        _videoGameApiService = videoGameApiService;
    }

    [HttpGet]
    public async Task<IActionResult> GetVideoGames([FromQuery] string? searchQuery)
    {
        var games = await _videoGameApiService.GetGamesAsync(searchQuery);

        if (games == null || games.Count == 0)
        {
            return NotFound("No games found.");
        }

        return Ok(games);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetVideoGameDetails(int id)
    {
        var game = await _videoGameApiService.GetGameDetailsAsync(id);

        if (game == null)
        {
            return NotFound("Game not found.");
        }

        return Ok(game);
    }
}
