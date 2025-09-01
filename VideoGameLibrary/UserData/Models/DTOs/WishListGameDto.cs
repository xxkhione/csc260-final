namespace UserData.Models.DTOs
{
    public class WishListGameDto
    {
        public int WishListGameId { get; set; }
        public int ApiGameId { get; set; }
        public int VideoGameId { get; set; }
        public string Title { get; set; }
        public Platform Platform { get; set; }
        public string Genre { get; set; }
        public ESRBRating ESRBRating { get; set; }
        public int Year { get; set; }
        public string? Image { get; set; }
        public string UserId { get; set; }
    }
}
