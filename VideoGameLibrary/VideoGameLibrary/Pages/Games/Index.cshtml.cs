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
    public class IndexModel : PageModel
    {
        private readonly VideoGameDBContext _context;

        public IndexModel(VideoGameDBContext context)
        {
            _context = context;
        }

        public IList<VideoGame> VideoGames { get;set; } = default!;

        public async Task OnGetAsync()
        {
            VideoGames = await _context.VideoGames.ToListAsync();
        }
    }
}
