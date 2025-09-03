using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis;

namespace VideoGameLibrary.Pages
{
    [Authorize]
    public class VideoGamesModel : PageModel
    {
        private readonly UserManager<IdentityUser> userManager;
        private IdentityUser user;

        [BindProperty]
        public required int DeleteId { get; set; }
        [BindProperty]
        public string? SearchKey { get; set; }
        [BindProperty]
        public string? Genre { get; set; }
        [BindProperty]
        public int? PlatformId { get; set; }
        public List<dynamic> Platforms { get; set; }
        [BindProperty]
        public int? ESRBRatingId { get; set; }
        public List<dynamic> ESRBRatings { get; set; }

        
        public List<dynamic> ShownGames { get; set; } = new();

        public VideoGamesModel(UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
        }
            

        public async Task OnGetAsync()
        {
            user = await userManager.GetUserAsync(User);
            if(user != null)
            {
                ShownGames = context.VideoGames.Where(g => g.UserId == user.Id).ToList();
            }
        }
        public async Task<IActionResult> OnPostFilter()
        {
            user = await userManager.GetUserAsync(User);
            if (user != null)
            {
                var chosenPlatform = PlatformId.HasValue ? PlatformId : null;
                var chosenRating = ESRBRatingId.HasValue ? ESRBRatingId : null;


                ShownGames = context.VideoGames.Where(vg => vg.UserId == user.Id).Where(v => Genre == null || v.Genre.ToLower().Contains(Genre.ToLower()))
                    .Where(v => chosenPlatform == null || v.Platform == chosenPlatform)
                    .Where(v => chosenRating == null || v.ESRBRating == chosenRating).ToList();
            }
            
            return Page();
        }
        public async Task<IActionResult> OnPostDelete()
        {
            user = await userManager.GetUserAsync(User);
            if(user != null)
            {
                var gameToRemove = context.VideoGames.FirstOrDefault(vg => vg.VideoGameId == DeleteId && vg.UserId == user.Id);

                if(gameToRemove != null)
                {
                    context.VideoGames.Remove(gameToRemove);
                    await context.SaveChangesAsync();
                }
            }
            return Page();
        }
        public async Task<IActionResult> OnPostSearch()
        {
            user = await userManager.GetUserAsync(User);
            if(user != null)
            {
                if (SearchKey == "" || SearchKey == null)
                {
                    ShownGames = context.VideoGames.Where(v => v.UserId == user.Id).ToList();
                    return Page();
                }
                SearchKey = SearchKey.ToLower();

                ShownGames = context.VideoGames.Where(v => v.UserId == user.Id).Where(v => v.Title.ToLower().Contains(SearchKey)).ToList();
            }
            return Page();
        }
    }
}
