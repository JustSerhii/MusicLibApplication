using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppLab;
using Microsoft.AspNetCore.Authorization;

namespace WebAppLab.Controllers
{
    public class AlbumReviewsController : Controller
    {
        private readonly DblibraryContext _context;

        public AlbumReviewsController(DblibraryContext context)
        {
            _context = context;
        }

        // GET: AlbumReviews
        public async Task<IActionResult> Index(int? id, string? name)
        {
            if (id == null) return RedirectToAction("Index", "Albums");

            ViewBag.AlbumId = id;
            ViewBag.AlbumTitle = name;
            var albumReviewsBySongs = _context.AlbumReviews.Where(b => b.AlbumId == id).Include(b => b.Album);
            return View(await albumReviewsBySongs.ToListAsync());
        }

        // GET: AlbumReviews/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.AlbumReviews == null)
            {
                return NotFound();
            }

            var albumReview = await _context.AlbumReviews
                .Include(a => a.Album)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (albumReview == null)
            {
                return NotFound();
            }

            return View(albumReview);
        }

        // GET: AlbumReviews/Create
        public IActionResult Create(int albumId)
        {
            ViewBag.AlbumId = albumId;
            ViewBag.AlbumTitle = _context.Albums.Where(c => c.Id == albumId).FirstOrDefault().Title;
            return View();
        }

        // POST: AlbumReviews/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int albumId, [Bind("Id,AlbumId,Title,WritingDate,ReviewContent,AlbumScore")] AlbumReview albumReview)
        {
            albumReview.AlbumId = albumId;
            if (ModelState.IsValid)
            {
                _context.Add(albumReview);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "AlbumReviews", new { id = albumId, name = _context.Albums.Where(c => c.Id == albumId).FirstOrDefault().Title });
            }
            //ViewData["ArtistId"] = new SelectList(_context.Artists, "Id", "Name", song.ArtistId);
            //ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "GenreName", song.GenreId);
            return RedirectToAction("Index", "AlbumReviews", new { id = albumId, name = _context.Albums.Where(c => c.Id == albumId).FirstOrDefault().Title });
        }

        // GET: AlbumReviews/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.AlbumReviews == null)
            {
                return NotFound();
            }

            var albumReview = await _context.AlbumReviews.FindAsync(id);
            if (albumReview == null)
            {
                return NotFound();
            }
            ViewData["AlbumId"] = new SelectList(_context.Albums, "Id", "Id", albumReview.AlbumId);
            return View(albumReview);
        }

        // POST: AlbumReviews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AlbumId,Title,WritingDate,ReviewContent,AlbumScore")] AlbumReview albumReview)
        {
            if (id != albumReview.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(albumReview);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlbumReviewExists(albumReview.Id))
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
            ViewData["AlbumId"] = new SelectList(_context.Albums, "Id", "Id", albumReview.AlbumId);
            return View(albumReview);
        }

        // GET: AlbumReviews/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.AlbumReviews == null)
            {
                return NotFound();
            }

            var albumReview = await _context.AlbumReviews
                .Include(a => a.Album)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (albumReview == null)
            {
                return NotFound();
            }

            return View(albumReview);
        }

        // POST: AlbumReviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.AlbumReviews == null)
            {
                return Problem("Entity set 'DblibraryContext.AlbumReviews'  is null.");
            }
            var albumReview = await _context.AlbumReviews.FindAsync(id);
            if (albumReview != null)
            {
                _context.AlbumReviews.Remove(albumReview);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AlbumReviewExists(int id)
        {
          return _context.AlbumReviews.Any(e => e.Id == id);
        }
    }
}
