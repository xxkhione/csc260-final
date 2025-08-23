using VideoGameLibrary.Models;

namespace VideoGameLibrary.Data
{
    public class DBInitializer
    {
        public static void Initialize(VideoGameDBContext context)
        {
            if (context.VideoGames.Any())
            {
                return;
            }

            var videoGames = new VideoGame[]
            {
                new VideoGame() {Title = "Animal Crossing: New Horizons", Platform = Platform.Nintendo, Genre = "Adventure, Social Simulation", ESRBRating = ESRBRating.Everyone, Year = 2020, Image = "images/AnimalCrossing.jpg"},
                new VideoGame() {Title = "Baldur's Gate 3", Platform = Platform.PC, Genre = "RPG, Strategy, Adventure", ESRBRating = ESRBRating.Mature, Year = 2023, Image = "images/BaldursGate3.jpg"},
                new VideoGame() {Title = "The Elder Scrolls V: Skyrim", Platform = Platform.Xbox, Genre = "Action RPG, Speculative Fiction, Adventure, Fantasy", ESRBRating = ESRBRating.Mature, Year = 2011, Image="images/Skyrim.png"},
                new VideoGame() {Title = "Final Fantasy XIII", Platform = Platform.PC, Genre = "Japanese RPG, Adventure, Action RPG, Strategy", ESRBRating = ESRBRating.Teen, Year = 2009, Image = "images/FinalFantasyXIII.jpg"},
                new VideoGame() {Title = "Rage 2", Platform = Platform.Xbox, Genre = "FPS, Action Adventure, Nonlinear Gameplay", ESRBRating = ESRBRating.Mature, Year = 2019, Image = "images/RageTwo.jpg"},
            };

            context.VideoGames.AddRange(videoGames);
            context.SaveChanges();
        }
    }
}
