using VideoGameLibrary.Models;

namespace VideoGameLibrary.Data.DAL
{
    //public class MockVideoGameDB : IVideoGameDal
    //{
    //    private static List<VideoGame> VideoGameList { get; set; } = new List<VideoGame>
    //    {
    //        new VideoGame() {VideoGameId = 1, Title = "Animal Crossing: New Horizons", Platform = Platform.Nintendo, Genre = "Adventure, Social Simulation", ESRBRating = ESRBRating.E, Year = 2020, Image = "images/AnimalCrossing.jpg"},
    //        new VideoGame() {VideoGameId = 2, Title = "Baldur's Gate 3", Platform = Platform.PC, Genre = "RPG, Strategy, Adventure", ESRBRating = ESRBRating.M, Year = 2023, Image = "images/BaldursGate3.jpg"},
    //        new VideoGame() {VideoGameId = 3, Title = "The Elder Scrolls V: Skyrim", Platform = Platform.Xbox, Genre = "Action RPG, Speculative Fiction, Adventure, Fantasy", ESRBRating = ESRBRating.M, Year = 2011, Image="images/Skyrim.png"},
    //        new VideoGame() {VideoGameId = 4, Title = "Final Fantasy XIII", Platform = Platform.PC, Genre = "Japanese RPG, Adventure, Action RPG, Strategy", ESRBRating = ESRBRating.T, Year = 2009, Image = "images/FinalFantasyXIII.jpg"},
    //        new VideoGame() {VideoGameId = 5, Title = "Rage 2", Platform = Platform.Xbox, Genre = "FPS, Action Adventure, Nonlinear Gameplay", ESRBRating = ESRBRating.M, Year = 2019, Image = "images/RageTwo.jpg"},
    //    };

    //    public void AddGame(VideoGame videoGame)
    //    {
    //        if(videoGame != null && !VideoGameList.Contains(videoGame))
    //        {
    //            videoGame.VideoGameId = VideoGameList.Count() + 1;
    //            VideoGameList.Add(videoGame);
    //        }
    //    }

    //    public void DeleteGame(int id)
    //    {
    //        if(id >= 1 && id <= VideoGameList.Count)
    //        {
    //            VideoGame toRemove = VideoGameList.FirstOrDefault(v => v.VideoGameId == id);
    //            if(toRemove != null)
    //            {
    //                VideoGameList.Remove(toRemove);
    //            } 
    //        }
    //    }

    //    public IEnumerable<VideoGame> FilterCollection(string? genre = null, Platform? platform = null, ESRBRating? esrbRating = null)
    //    {
    //        return VideoGameList.Where(v => genre == null || v.Genre.Contains(genre, StringComparison.OrdinalIgnoreCase))
    //            .Where(v => platform == null || v.Platform == platform)
    //            .Where(v => esrbRating == null || v.ESRBRating == esrbRating);
    //    }

    //    public IEnumerable<VideoGame> GetCollection()
    //    {
    //        return VideoGameList;
    //    }

    //    public IEnumerable<VideoGame> SearchForGames(string key)
    //    {
    //        if(key == "" || key == null)
    //        {
    //            return GetCollection();
    //        }
    //        return VideoGameList.Where(v => v.Title.Contains(key, StringComparison.OrdinalIgnoreCase));
    //    }
    //}
}
