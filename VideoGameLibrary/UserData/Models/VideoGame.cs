using System.ComponentModel.DataAnnotations;

namespace UserData.Models
{
    public class VideoGame
    {
        [Key]
        public int VideoGameId { get; set; }
        public int ApiGameId { get; set; }
        public string Title { get; set; }
        public Platform Platform { get; set; }
        public string Genre { get; set; }
        public ESRBRating ESRBRating { get; set; }
        public int Year { get; set; }
        public string? Image { get; set; }
        public string? LoanedTo { get; set; }
        public DateTime? LoanedDate { get; set; }
        public string UserId { get; set; }
    }

    public enum Platform
    {
        Nintendo,
        Xbox,
        Playstation,
        PC,
        Unknown
    }

    public enum ESRBRating
    {
        Everyone,
        Teen,
        Mature,
        Everyone_10,
        Rating_Pending,
        Adults_Only,
        Early_Childhood,
        Unknown
    }
}
