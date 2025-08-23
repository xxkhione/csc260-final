using VideoGameLibrary.Models;

namespace VideoGameLibrary.Data.DAL
{
    public interface IVideoGameDal
    {
        IEnumerable<VideoGame> GetCollection();
        IEnumerable<VideoGame> SearchForGames(string key);
        IEnumerable<VideoGame> FilterCollection(string? genre, Platform? platform, ESRBRating? esrbRating);
        Task AddGame(VideoGame videoGame);
        Task DeleteGame(int id);
    }
}
