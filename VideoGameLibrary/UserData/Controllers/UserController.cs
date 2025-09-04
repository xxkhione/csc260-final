using Microsoft.AspNetCore.Mvc;
using UserData.Models.DTOs;
using UserData.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace UserData.Controllers;

[Route("api/users/videogames")]
[ApiController]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserDataService _userDataService;

    public UserController(IUserDataService userDataService)
    {
        _userDataService = userDataService;
    }

    private string GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    [HttpGet]
    public async Task<IActionResult> GetVideoGames(
        [FromQuery] string? title,
        [FromQuery] string? genre,
        [FromQuery] string? platform,
        [FromQuery] string? esrbRating)
    {
        var userId = GetUserId();
        var games = await _userDataService.GetVideoGamesForUserAsync(userId, title, genre, platform, esrbRating);
        return Ok(games);
    }

    [HttpPost]
    public async Task<IActionResult> AddVideoGame([FromBody] VideoGameDto videoGameDto)
    {
        var userId = GetUserId();
        var newGame = await _userDataService.AddVideoGameAsync(userId, videoGameDto);
        return CreatedAtAction(nameof(GetVideoGames), new { id = newGame.VideoGameId }, newGame);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateVideoGame(int id, [FromBody] VideoGameDto videoGameDto)
    {
        if (id != videoGameDto.VideoGameId)
        {
            return BadRequest("Game ID in the URL does not match the ID in the request body.");
        }

        var userId = GetUserId();
        var success = await _userDataService.UpdateVideoGameAsync(userId, videoGameDto);

        if (!success)
        {
            return NotFound("Game not found or user not authorized to update this game.");
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVideoGame(int id)
    {
        var userId = GetUserId();
        var success = await _userDataService.DeleteVideoGameAsync(userId, id);

        if (!success)
        {
            return NotFound("Game not found or user not authorized to delete this game.");
        }

        return NoContent();
    }

    [HttpGet("wishlist")]
    public async Task<IActionResult> GetWishList()
    {
        var userId = GetUserId();
        var wishlistGames = await _userDataService.GetWishListGamesForUserAsync(userId);
        return Ok(wishlistGames);
    }

    [HttpPost("wishlist")]
    public async Task<IActionResult> AddWishListGame([FromBody] WishListGameDto wishListGameDto)
    {
        var userId = GetUserId();
        var newGame = await _userDataService.AddWishListGameAsync(userId, wishListGameDto);
        return CreatedAtAction(nameof(GetWishList), new { id = newGame.WishListGameId }, newGame);
    }

    [HttpDelete("wishlist/{id}")]
    public async Task<IActionResult> DeleteWishListGame(int id)
    {
        var userId = GetUserId();
        var success = await _userDataService.DeleteWishListGameAsync(userId, id);

        if (!success)
        {
            return NotFound("Game not found in wishlist or user not authorized to delete this game.");
        }

        return NoContent();
    }
}