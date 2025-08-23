using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoGameLibrary.Models;

namespace VideoGameLibrary.Data
{
    public class VideoGameDBContext : DbContext
    {
        public VideoGameDBContext (DbContextOptions<VideoGameDBContext> options)
            : base(options)
        {
        }

        public DbSet<VideoGame> VideoGames { get; set; } = default!;
    }
}
