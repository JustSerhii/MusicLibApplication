using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppLab;

namespace WebAppLab.Controllers
{
    public class SongsController : Controller
    {
        private readonly DblibraryContext _context;

        public SongsController(DblibraryContext context)
        {
            _context = context;
        }

        // GET: Songs
        public async Task<IActionResult> Index(int? id, string? name)
        {
            if (id == null) return RedirectToAction("Index", "Artists");

            ViewBag.ArtistId = id;
            ViewBag.ArtistName = name;
            var songsByArtist = _context.Songs.Where(b => b.ArtistId == id).Include(b => b.Artist).Include(b => b.Genre);
            return View(await songsByArtist.ToListAsync());
        }

        // GET: Songs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Songs == null)
            {
                return NotFound();
            }

            var song = await _context.Songs
                .Include(s => s.Artist)
                .Include(s => s.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (song == null)
            {
                return NotFound();
            }

            return RedirectToAction("Index", "AlbumSongs", new { id = song.Id, name = song.Title });
            //return RedirectToAction("Index", "SongReviews", new { id = song.Id, name = song.Title });
        }

        // GET: Songs/Create
        public IActionResult Create(int artistId)
        {
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "GenreName");
            //ViewData["ArtistId"] = new SelectList(_context.Artists, "Id", "Name");
            ViewBag.ArtistId = artistId;
            ViewBag.ArtistName = _context.Artists.Where(c => c.Id == artistId).FirstOrDefault().Name;
            return View();
        }

        // POST: Songs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int artistId, [Bind("Id,GenreId,ArtistId,Title,ReleaseDate,SongLength")] Song song)
        {
            song.ArtistId = artistId;
            if (ModelState.IsValid)
            {
                _context.Add(song);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Songs", new { id = artistId, name = _context.Artists.Where(c => c.Id == artistId).FirstOrDefault().Name });
            }
            //ViewData["ArtistId"] = new SelectList(_context.Artists, "Id", "Name", song.ArtistId);
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "GenreName", song.GenreId);
            return RedirectToAction("Index", "Songs", new { id = artistId, name = _context.Artists.Where(c => c.Id == artistId).FirstOrDefault().Name });
        }

        // GET: Songs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Songs == null)
            {
                return NotFound();
            }

            var song = await _context.Songs.FindAsync(id);
            if (song == null)
            {
                return NotFound();
            }
            ViewData["ArtistId"] = new SelectList(_context.Artists, "Id", "Name", song.ArtistId);
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "GenreName", song.GenreId);
            return View(song);
        }

        // POST: Songs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,GenreId,ArtistId,Title,ReleaseDate,SongLength")] Song song)
        {
            if (id != song.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(song);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SongExists(song.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ArtistId"] = new SelectList(_context.Artists, "Id", "Name", song.ArtistId);
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "GenreName", song.GenreId);
            return View(song);
        }

        // GET: Songs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Songs == null)
            {
                return NotFound();
            }

            var song = await _context.Songs
                .Include(s => s.Artist)
                .Include(s => s.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (song == null)
            {
                return NotFound();
            }

            return View(song);
        }

        // POST: Songs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Songs == null)
            {
                return Problem("Entity set 'DblibraryContext.Songs'  is null.");
            }
            var song = await _context.Songs.FindAsync(id);
            if (song != null)
            {
                _context.Songs.Remove(song);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SongExists(int id)
        {
            return (_context.Songs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}