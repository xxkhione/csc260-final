using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using VideoGameLibrary.Data;
using VideoGameLibrary.Models;

namespace VideoGameLibrary.Pages
{
    [Authorize]
    public class WishListGamesModel : PageModel
    {
        public readonly LoginContext context;
        private readonly UserManager<IdentityUser> userManager;
        private IdentityUser user;

        [BindProperty]
        public required int DeleteId { get; set; }
        [BindProperty]
        public required int AddId { get; set; }
        [BindProperty]
        public string? SearchKey { get; set; }
        [BindProperty]
        public string? Genre { get; set; }
        [BindProperty]
        public int? PlatformId { get; set; }
        public List<Models.Platform> Platforms => Enum.GetValues<Models.Platform>().ToList();
        [BindProperty]
        public int? ESRBRatingId { get; set; }
        public List<ESRBRating> ESRBRatings => Enum.GetValues<ESRBRating>().ToList();
        public VideoGameApiService videoGameApiService;


        public List<WishListGame> ShownGames { get; set; } = new();

        public WishListGamesModel(LoginContext context, UserManager<IdentityUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }


        public async Task OnGetAsync()
        {
            user = await userManager.GetUserAsync(User);
            if (user != null)
            {
                ShownGames = context.WishList.Where(g => g.UserId == user.Id).ToList();
            }
        }
        public async Task<IActionResult> OnPostFilter()
        {
            user = await userManager.GetUserAsync(User);
            if (user != null)
            {
                Models.Platform? chosenPlatform = PlatformId.HasValue ? (Models.Platform)PlatformId : null;
                ESRBRating? chosenRating = ESRBRatingId.HasValue ? (ESRBRating)ESRBRatingId : null;


                ShownGames = context.WishList.Where(vg => vg.UserId == user.Id).Where(v => Genre == null || v.Genre.ToLower().Contains(Genre.ToLower()))
                    .Where(v => chosenPlatform == null || v.Platform == chosenPlatform)
                    .Where(v => chosenRating == null || v.ESRBRating == chosenRating).ToList();
            }

            return Page();
        }
        public async Task<IActionResult> OnPostDelete()
        {
            user = await userManager.GetUserAsync(User);
            if (user != null)
            {
                WishListGame gameToRemove = context.WishList.FirstOrDefault(vg => vg.VideoGameId == DeleteId && vg.UserId == user.Id);

                if (gameToRemove != null)
                {
                    context.WishList.Remove(gameToRemove);
                    await context.SaveChangesAsync();
                }
            }
            return Page();
        }
        public async Task<IActionResult> OnPostSearch()
        {
            user = await userManager.GetUserAsync(User);
            if (user != null)
            {
                if (SearchKey == "" || SearchKey == null)
                {
                    ShownGames = context.WishList.Where(v => v.UserId == user.Id).ToList();
                    return Page();
                }
                SearchKey = SearchKey.ToLower();

                ShownGames = context.WishList.Where(v => v.UserId == user.Id).Where(v => v.Title.ToLower().Contains(SearchKey)).ToList();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAddToCollection()
        {
            user = await userManager.GetUserAsync(User);
            if (user != null)
            {
                var wishListGame = await context.WishList
                    .FirstOrDefaultAsync(w => w.UserId == user.Id && w.VideoGameId == AddId);

                if (wishListGame != null)
                {
                    context.WishList.Remove(wishListGame);
                    await context.SaveChangesAsync();
                }
            }
            return Page();
        }
    }
}
