using UserData.Models.DTOs;

namespace UserData.Services
{
    public interface IUserDataService
    {
        Task<List<VideoGameDto>> GetVideoGamesForUserAsync(string userId, string? title, string? genre, string? platform, string? esrbRating);
        Task<VideoGameDto> AddVideoGameAsync(string userId, VideoGameDto videoGameDto);
        Task<bool> UpdateVideoGameAsync(string userId, VideoGameDto videoGameDto); //handles the loaning functionality, but it's easier to read this way.
        Task<bool> DeleteVideoGameAsync(string userId, int videoGameId);

        Task<List<WishListGameDto>> GetWishListGamesForUserAsync(string userId);
        Task<WishListGameDto> AddWishListGameAsync(string userId, WishListGameDto wishListGameDto);
        Task<bool> DeleteWishListGameAsync(string userId, int wishListGameId);
    }
}
