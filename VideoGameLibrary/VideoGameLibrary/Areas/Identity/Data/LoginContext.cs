using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VideoGameLibrary.Areas.Identity;
using VideoGameLibrary.Models;

namespace VideoGameLibrary.Data;

public class LoginContext : IdentityDbContext<IdentityUser>
{
    public LoginContext(DbContextOptions<LoginContext> options)
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

        builder.Entity<VideoGame>()
            .HasOne(vg => vg.User)
            .WithMany()
            .HasForeignKey(vg => vg.UserId);

        builder.Entity<WishListGame>()
            .HasOne(wlg => wlg.User)
            .WithMany()
            .HasForeignKey(wlg => wlg.UserId);
    }
}
