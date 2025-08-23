using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VideoGameLibrary.Data;
using VideoGameLibrary.Models;

namespace VideoGameLibrary.Pages
{
    [Authorize]
    public class SearchModel(LoginContext context, UserManager<IdentityUser> userManager, VideoGameApiService videoGameApiService) : PageModel
    {
        public readonly LoginContext context = context;
        private readonly UserManager<IdentityUser> userManager = userManager;
        private IdentityUser user;
        public VideoGameApiService videoGameApiService = videoGameApiService;

        [BindProperty]
        public string SearchQuery { get; set; }

        public List<VideoGame> SearchResults { get; set; } = new();

        public void OnGet()
        {
            
        }

        public async Task<IActionResult> OnPostAddToCollection(int gameId)
        {
            user = await userManager.GetUserAsync(User);
            if (user != null)
            {
                VideoGame game = await videoGameApiService.GetGameFromDatabase(user.Id, gameId);
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAddToWishList(int gameId)
        {
            user = await userManager.GetUserAsync(User);
            if (user != null)
            {
                WishListGame game = await videoGameApiService.GetWishGameFromDatabase(user.Id, gameId);
            }
            return Page();
        }
        public async Task<IActionResult> OnPostSearch()
        {
            user = await userManager.GetUserAsync(User);
            if (user != null)
            {
                if (!string.IsNullOrEmpty(SearchQuery))
                {
                    SearchResults = await videoGameApiService.GetGamesAsync(SearchQuery);
                    if (SearchResults != null)
                    {
                        user = await userManager.GetUserAsync(User);
                        if (user != null)
                        {
                            List<VideoGame> ownedGames = context.VideoGames.Where(vg => vg.UserId == user.Id).ToList();
                            SearchResults = SearchResults.Where(vg => !ownedGames.Any(y => y.VideoGameId == vg.VideoGameId)).ToList();
                        }
                    }
                }
            }
            return Page();
        }
    }
}
