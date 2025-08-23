using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VideoGameLibrary.Data;
using VideoGameLibrary.Models;

namespace VideoGameLibrary.Pages.Games
{
    public class DeleteModel : PageModel
    {
        private readonly VideoGameDBContext _context;

        public DeleteModel(VideoGameDBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public VideoGame VideoGame { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var videogame = await _context.VideoGames.FirstOrDefaultAsync(m => m.VideoGameId == id);

            if (videogame == null)
            {
                return NotFound();
            }
            else
            {
                VideoGame = videogame;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var videogame = await _context.VideoGames.FindAsync(id);
            if (videogame != null)
            {
                VideoGame = videogame;
                _context.VideoGames.Remove(VideoGame);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
