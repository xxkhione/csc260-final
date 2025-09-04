using Microsoft.EntityFrameworkCore;
using UserData.Data;
using UserData.Models;
using UserData.Models.DTOs;

namespace UserData.Services
{
    public class UserDataService : IUserDataService
    {
        private readonly UserDataDbContext _context;
        public UserDataService(UserDataDbContext context)
        {
            _context = context;
        }

        public async Task<List<VideoGameDto>> GetVideoGamesForUserAsync(string userId, string? title, string? genre, string? platform, string? esrbRating)
        {
            var query = _context.VideoGames
                .Where(vg => vg.UserId == userId);

            if(!string.IsNullOrEmpty(title))
            {
                query = query.Where(vg => vg.Title.ToLower().Contains(title.ToLower()));
            }
            if (!string.IsNullOrEmpty(genre))
            {
                query = query.Where(vg => vg.Genre.ToLower().Contains(genre.ToLower()));
            }
            if (!string.IsNullOrEmpty(platform))
            {
                var chosenPlatform = (Platform)Enum.Parse(typeof(Platform), platform, true);
                query = query.Where(vg => vg.Platform == chosenPlatform);
            }
            if (!string.IsNullOrEmpty(esrbRating))
            {
                var chosenRating = (ESRBRating)Enum.Parse(typeof(ESRBRating), esrbRating, true);
                query = query.Where(vg => vg.ESRBRating == chosenRating);
            }

            var videoGames = await query.ToListAsync();

            return videoGames.Select(vg => new VideoGameDto
            {
                VideoGameId = vg.VideoGameId,
                Title = vg.Title,
                Platform = vg.Platform.ToString(),
                Genre = vg.Genre,
                ESRBRating = vg.ESRBRating.ToString(),
                Year = vg.Year,
                Image = vg.Image,
                LoanedTo = vg.LoanedTo,
                LoanedDate = vg.LoanedDate
            }).ToList();
        }

        public async Task<VideoGameDto> AddVideoGameAsync(string userId, VideoGameDto videoGameDto)
        {
            var newVideoGame = new VideoGame
            {
                ApiGameId = videoGameDto.ApiGameId,
                Title = videoGameDto.Title,
                Platform = (Platform)Enum.Parse(typeof(Platform), videoGameDto.Platform),
                Genre = videoGameDto.Genre,
                ESRBRating = (ESRBRating)Enum.Parse(typeof(ESRBRating), videoGameDto.ESRBRating),
                Year = videoGameDto.Year,
                Image = videoGameDto.Image,
                UserId = userId
            };

            _context.VideoGames.Add(newVideoGame);
            await _context.SaveChangesAsync();

            return new VideoGameDto
            {
                VideoGameId = newVideoGame.VideoGameId,
                Title = newVideoGame.Title,
                Platform = newVideoGame.Platform.ToString(),
                Genre = newVideoGame.Genre,
                ESRBRating = newVideoGame.ESRBRating.ToString(),
                Year = newVideoGame.Year,
                Image = newVideoGame.Image,
                LoanedTo = newVideoGame.LoanedTo,
                LoanedDate = newVideoGame.LoanedDate
            };
        }

        public async Task<bool> UpdateVideoGameAsync(string userId, VideoGameDto videoGameDto)
        {
            var existingGame = await _context.VideoGames
                .FirstOrDefaultAsync(vg => vg.VideoGameId == videoGameDto.VideoGameId && vg.UserId == userId);

            if(existingGame == null)
            {
                return false;
            }

            existingGame.LoanedTo = videoGameDto.LoanedTo;
            existingGame.LoanedDate = videoGameDto.LoanedDate;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteVideoGameAsync(string userId, int videoGameId)
        {
            var gameToDelete = await _context.VideoGames
                .FirstOrDefaultAsync(vg => vg.VideoGameId == videoGameId && vg.UserId == userId);

            if(gameToDelete == null)
            {
                return false;
            }

            _context.VideoGames.Remove(gameToDelete);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<WishListGameDto>> GetWishListGamesForUserAsync(string userId)
        {
            var wishListGames = await _context.WishList
                .Where(wlg => wlg.UserId == userId)
                .ToListAsync();

            return wishListGames.Select(wlg => new WishListGameDto
            {
                WishListGameId = wlg.WishListGameId,
                Title = wlg.Title,
                Platform = wlg.Platform.ToString(),
                Genre = wlg.Genre,
                ESRBRating = wlg.ESRBRating.ToString(),
                Year = wlg.Year,
                Image = wlg.Image
            }).ToList();
        }

        public async Task<WishListGameDto> AddWishListGameAsync(string userId, WishListGameDto wishListGameDto)
        {
            var newWishListGame = new WishListGame
            {
                ApiGameId = wishListGameDto.ApiGameId,
                Title = wishListGameDto.Title,
                Platform = (Platform)Enum.Parse(typeof(Platform), wishListGameDto.Platform),
                Genre = wishListGameDto.Genre,
                ESRBRating = (ESRBRating)Enum.Parse(typeof(ESRBRating), wishListGameDto.ESRBRating),
                Year = wishListGameDto.Year,
                Image = wishListGameDto.Image,
                UserId = userId
            };

            _context.WishList.Add(newWishListGame);
            await _context.SaveChangesAsync();

            return new WishListGameDto
            {
                VideoGameId = newWishListGame.VideoGameId,
                Title = newWishListGame.Title,
                Platform = newWishListGame.Platform.ToString(),
                Genre = newWishListGame.Genre,
                ESRBRating = newWishListGame.ESRBRating.ToString(),
                Year = newWishListGame.Year,
                Image = newWishListGame.Image,
            };
        }

        public async Task<bool> DeleteWishListGameAsync(string userId, int wishListGameId)
        {
            var gameToDelete = await _context.WishList
                .FirstOrDefaultAsync(wlg => wlg.WishListGameId == wishListGameId && wlg.UserId == userId);

            if (gameToDelete == null)
            {
                return false;
            }

            _context.WishList.Remove(gameToDelete);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
