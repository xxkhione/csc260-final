using Microsoft.EntityFrameworkCore;
using VideoGameLibrary.Data.DAL;
using VideoGameLibrary.Models;

namespace VideoGameLibrary.Data
{
    public class VideoGameDBDal : IVideoGameDal
    {
        private readonly VideoGameDBContext _context;

        public VideoGameDBDal(VideoGameDBContext context)
        {
            _context = context;
        }

        public async Task AddGame(VideoGame videoGame)
        {
            if (videoGame != null && !_context.VideoGames.Contains(videoGame))
            {
                _context.VideoGames.Add(videoGame);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteGame(int id)
        {
            VideoGame toRemove = await _context.VideoGames.FindAsync(id);
            if (toRemove != null)
            {
                _context.VideoGames.Remove(toRemove);
                await _context.SaveChangesAsync();
            }
        }

        public IEnumerable<VideoGame> FilterCollection(string? genre, Platform? platform, ESRBRating? esrbRating)
        {
            return _context.VideoGames.Where(v => genre == null || v.Genre.ToLower().Contains(genre.ToLower()))
               .Where(v => platform == null || v.Platform == platform)
               .Where(v => esrbRating == null || v.ESRBRating == esrbRating);
        }

        public IEnumerable<VideoGame> GetCollection()
        {
            return _context.VideoGames;
        }

        public IEnumerable<VideoGame> SearchForGames(string key)
        {
            if (key == "" || key == null)
            {
                return GetCollection();
            }
            key = key.ToLower();

            return _context.VideoGames.Where(v => v.Title.ToLower().Contains(key));
        }
    }
}
