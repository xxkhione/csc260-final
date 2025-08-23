using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using VideoGameLibrary.Data.DAL;
using VideoGameLibrary.Data;
using VideoGameLibrary.Pages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using VideoGameLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace VideoGameLibrary.Pages
{
    [Authorize]
    public class LoanModel(LoginContext context, UserManager<IdentityUser> userManager, VideoGameApiService videoGameApiService) : PageModel
    {
        public readonly LoginContext context = context;
        private readonly UserManager<IdentityUser> userManager = userManager;
        private IdentityUser user;
        public VideoGameApiService videoGameApiService = videoGameApiService;

        [BindProperty]
        public required int LoanGameId { get; set; }
        [BindProperty]
        public required string PersonLoaningData { get; set; }
        public List<VideoGame> videoGames { get; set; } = new();

        public async Task OnGetAsync()
        {
            user = await userManager.GetUserAsync(User);
            if (user != null)
            {
                videoGames = context.VideoGames.Where(vg => vg.UserId == user.Id).ToList();
            }
        }

        public async Task<IActionResult> OnPost()
        {
            var game = await context.VideoGames.FirstOrDefaultAsync(vg => vg.VideoGameId == LoanGameId && vg.UserId == user.Id);

            if (game != null)
            {
                game.LoanedTo = PersonLoaningData;
                game.LoanedDate = DateTime.Now;
                context.SaveChanges();
            }
            return RedirectToPage("/VideoGames");
        }

        public async Task<IActionResult> OnPostReturn()
        {
            var game = await context.VideoGames.FirstOrDefaultAsync(vg => vg.VideoGameId == LoanGameId && vg.UserId == user.Id);

            if (game != null)
            {
                game.LoanedTo = null;
                game.LoanedDate = null;
                context.SaveChanges();
            }
            return RedirectToPage("/VideoGames");
        }
    }
}
