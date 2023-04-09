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
    public class AlbumSongsController : Controller
    {
        private readonly DblibraryContext _context;

        public AlbumSongsController(DblibraryContext context)
        {
            _context = context;
        }

        // GET: AlbumSongs
        public async Task<IActionResult> Index(int? id, string? name)

        {
            if (id == null) return RedirectToAction("Index", "Albums");

            ViewBag.AlbumId = id;
            ViewBag.AlbumTitle = name;
            var albumSongsByAlbum = _context.AlbumSongs.Where(b => b.AlbumId == id).Include(b => b.Album).Include(b => b.Song);
            return View(await albumSongsByAlbum.ToListAsync());
        }

        // GET: AlbumSongs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.AlbumSongs == null)
            {
                return NotFound();
            }

            var albumSong = await _context.AlbumSongs
                .Include(a => a.Album)
                .Include(a => a.Song)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (albumSong == null)
            {
                return NotFound();
            }

            return View(albumSong);
           
        }

        // GET: AlbumSongs/Create
        public IActionResult Create(int albumId)
        {

            //ViewData["AlbumId"] = new SelectList(_context.Albums, "Id", "Title");
            ViewData["SongId"] = new SelectList(_context.Songs, "Id", "Title");

            ViewBag.AlbumId = albumId;
            ViewBag.AlbumTitle = _context.Albums.Where(c => c.Id == albumId).FirstOrDefault().Title;

            return View();
        }

        // POST: AlbumSongs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int albumId, [Bind("Id,SongId,AlbumId")] AlbumSong albumSong)
        {   
            albumSong.AlbumId = albumId;
            if (ModelState.IsValid)
            {
                _context.Add(albumSong);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "AlbumSongs", new { id = albumId, name = _context.Albums.Where(c => c.Id == albumId).FirstOrDefault().Title});;
            }
            //ViewData["AlbumId"] = new SelectList(_context.Albums, "Id", "Id", albumSong.AlbumId);
            ViewData["SongId"] = new SelectList(_context.Songs, "Id", "Id", albumSong.SongId);
            return RedirectToAction("Index", "AlbumSongs", new { id = albumId, name = _context.Albums.Where(c => c.Id == albumId).FirstOrDefault().Title }); ;
        }

        // GET: AlbumSongs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.AlbumSongs == null)
            {
                return NotFound();
            }

            var albumSong = await _context.AlbumSongs.FindAsync(id);
            if (albumSong == null)
            {
                return NotFound();
            }
            ViewData["AlbumId"] = new SelectList(_context.Albums, "Id", "Id", albumSong.AlbumId);
            ViewData["SongId"] = new SelectList(_context.Songs, "Id", "Id", albumSong.SongId);
            return View(albumSong);
        }

        // POST: AlbumSongs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SongId,AlbumId")] AlbumSong albumSong)
        {
            if (id != albumSong.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(albumSong);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlbumSongExists(albumSong.Id))
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
            ViewData["AlbumId"] = new SelectList(_context.Albums, "Id", "Id", albumSong.AlbumId);
            ViewData["SongId"] = new SelectList(_context.Songs, "Id", "Id", albumSong.SongId);
            return View(albumSong);
        }

        // GET: AlbumSongs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.AlbumSongs == null)
            {
                return NotFound();
            }

            var albumSong = await _context.AlbumSongs
                .Include(a => a.Album)
                .Include(a => a.Song)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (albumSong == null)
            {
                return NotFound();
            }

            return View(albumSong);
        }

        // POST: AlbumSongs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.AlbumSongs == null)
            {
                return Problem("Entity set 'DblibraryContext.AlbumSongs'  is null.");
            }
            var albumSong = await _context.AlbumSongs.FindAsync(id);
            if (albumSong != null)
            {
                _context.AlbumSongs.Remove(albumSong);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AlbumSongExists(int id)
        {
          return _context.AlbumSongs.Any(e => e.Id == id);
        }
    }
}
