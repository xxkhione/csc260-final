using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VideoGameLibrary.Models
{
    public class VideoGame
    {
        public int VideoGameId { get; set; }
        //results.id
        public int ApiGameId { get; set; }
        //results.name
        public string Title { get; set; }
        //results.parent_platforms.platform.name
        public Platform Platform { get; set; }
        //results.genres.name
        public string Genre { get; set; }
        //results.esrb_rating.name
        public ESRBRating ESRBRating { get; set; }
        //results.released (parse to just get the year released)
        public int Year { get; set; }
        //results.background_image
        public string? Image { get; set; }
        public string? LoanedTo { get; set; }
        public DateTime? LoanedDate { get; set; }
        public string UserId { get; set; }
        public IdentityUser User { get; set; }
        public override string ToString()
        {
            return $"{Title} on {Platform} ({Year}) - Rating: {ESRBRating}";
        }
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
