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
    public class SongReviewsController : Controller
    {
        private readonly DblibraryContext _context;

        public SongReviewsController(DblibraryContext context)
        {
            _context = context;
        }

        // GET: SongReviews
        public async Task<IActionResult> Index(int? id, string? name)
        {
            if (id == null) return RedirectToAction("Index", "Songs");

            ViewBag.songId = id;
            ViewBag.songTitle = name;
            var songReviewsBySongs = _context.SongReviews.Where(b => b.SongId == id).Include(b => b.Song);
            return View(await songReviewsBySongs.ToListAsync());
        }

        // GET: SongReviews/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.SongReviews == null)
            {
                return NotFound();
            }

            var songReview = await _context.SongReviews
                .Include(s => s.Song)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (songReview == null)
            {
                return NotFound();
            }

            return View(songReview);
        }

        // GET: SongReviews/Create
        public IActionResult Create(int songId)
        {
            ViewBag.songId = songId;
            ViewBag.songTitle = _context.Songs.Where(c => c.Id == songId).FirstOrDefault().Title;
            return View();
        }

        // POST: SongReviews/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int songId, [Bind("Id,SongId,Title,WritingDate,ReviewContent,SongScore")] SongReview songReview)
        {   
            songReview.SongId = songId;
            if (ModelState.IsValid)
            {
                _context.Add(songReview);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "SongReviews", new { id = songId, name = _context.Songs.Where(c => c.Id == songId).FirstOrDefault().Title });
            }
            //ViewData["ArtistId"] = new SelectList(_context.Artists, "Id", "Name", song.ArtistId);
            //ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "GenreName", song.GenreId);
            return RedirectToAction("Index", "SongReviews", new { id = songId, name = _context.Songs.Where(c => c.Id == songId).FirstOrDefault().Title });
        }

        // GET: SongReviews/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.SongReviews == null)
            {
                return NotFound();
            }

            var songReview = await _context.SongReviews.FindAsync(id);
            if (songReview == null)
            {
                return NotFound();
            }
            ViewData["SongId"] = new SelectList(_context.Songs, "Id", "Id", songReview.SongId);
            return View(songReview);
        }

        // POST: SongReviews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SongId,Title,WritingDate,ReviewContent,SongScore")] SongReview songReview)
        {
            if (id != songReview.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(songReview);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SongReviewExists(songReview.Id))
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
            ViewData["SongId"] = new SelectList(_context.Songs, "Id", "Id", songReview.SongId);
            return View(songReview);
        }

        // GET: SongReviews/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.SongReviews == null)
            {
                return NotFound();
            }

            var songReview = await _context.SongReviews
                .Include(s => s.Song)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (songReview == null)
            {
                return NotFound();
            }

            return View(songReview);
        }

        // POST: SongReviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.SongReviews == null)
            {
                return Problem("Entity set 'DblibraryContext.SongReviews'  is null.");
            }
            var songReview = await _context.SongReviews.FindAsync(id);
            if (songReview != null)
            {
                _context.SongReviews.Remove(songReview);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SongReviewExists(int id)
        {
          return _context.SongReviews.Any(e => e.Id == id);
        }
    }
}
