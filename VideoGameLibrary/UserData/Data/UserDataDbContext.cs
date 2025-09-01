using Microsoft.EntityFrameworkCore;
using UserData.Models;

namespace UserData.Data
{
    public class UserDataDbContext : DbContext
    {
        public UserDataDbContext(DbContextOptions<UserDataDbContext> options)
        : base(options)
        {
        }

        public DbSet<VideoGame> VideoGames { get; set; }
        public DbSet<WishListGame> WishList { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<VideoGame>()
                .HasKey(vg => vg.VideoGameId);

            builder.Entity<VideoGame>()
                .Property(vg => vg.VideoGameId)
                .ValueGeneratedOnAdd();

            builder.Entity<WishListGame>()
                .Property(wlg => wlg.WishListGameId);
            builder.Entity<WishListGame>()
                .Property(wlg => wlg.WishListGameId)
                .ValueGeneratedOnAdd();
        }
    }
}
