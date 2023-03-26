using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebAppLab.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartController : ControllerBase
    {
        private readonly DblibraryContext _context;
        public ChartController(DblibraryContext context)
        {
            _context = context;
        }
        [HttpGet("JsonData")]
        public JsonResult JsonData()
        {
            var artists = _context.Artists.Include(b=>b.Songs).ToList();
            List<object> artistSong = new();
            artistSong.Add(new[] { "Artist", "Number of songs" });
            foreach (var artist in artists)
            {
                artistSong.Add(new object[] { artist.Name, artist.Songs.Count() });
            }
            return new JsonResult(artistSong);
        }
    }
}
