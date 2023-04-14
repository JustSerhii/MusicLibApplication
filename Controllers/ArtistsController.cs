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
    public class ArtistsController : Controller
    {
        private readonly DblibraryContext _context;

        public ArtistsController(DblibraryContext context)
        {
            _context = context;
        }

        // GET: Artists
        public async Task<IActionResult> Index()
        {
              return _context.Artists != null ? 
                          View(await _context.Artists.ToListAsync()) :
                          Problem("Entity set 'DblibraryContext.Artists'  is null.");
        }

        // GET: Artists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Artists == null)
            {
                return NotFound();
            }

            var artist = await _context.Artists
                .FirstOrDefaultAsync(m => m.Id == id);
            if (artist == null)
            {
                return NotFound();
            }

            return RedirectToAction("Index", "Songs", new { id = artist.Id, name = artist.Name });
        }

        // GET: Artists/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Artists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ActiveSince,ActivityStop")] Artist artist)
        {   

            if (ModelState.IsValid)
            {
                DateTime enteredDate = artist.ActiveSince;
                DateTime? enteredDate1 = artist.ActivityStop;

                DateTime startDate = new DateTime(1000, 1, 1);
                DateTime endDate = DateTime.Now;

                if (enteredDate <= startDate || enteredDate >= endDate)
                {
                    ModelState.AddModelError("ActiveSince", "Not available value. Set in range: 01.01.1000 - today");
                    return View(artist);
                }

                if ((enteredDate1 <= enteredDate || enteredDate1 >= endDate) && enteredDate1 != null)
                {
                    ModelState.AddModelError("ActivityStop", "Not available value. Date should be after date of the start of activity until today");
                    return View(artist);
                }
                _context.Add(artist);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(artist);
        }

        // GET: Artists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Artists == null)
            {
                return NotFound();
            }

            var artist = await _context.Artists.FindAsync(id);
            if (artist == null)
            {
                return NotFound();
            }
            return View(artist);
        }

        // POST: Artists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ActiveSince,ActivityStop")] Artist artist)
        {
            if (id != artist.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    DateTime enteredDate = artist.ActiveSince;
                    DateTime? enteredDate1 = artist.ActivityStop;

                    DateTime startDate = new DateTime(1000, 1, 1);
                    DateTime endDate = DateTime.Now;

                    if (enteredDate <= startDate || enteredDate >= endDate)
                    {
                        ModelState.AddModelError("ActiveSince", "Not available value. Set in range: 01.01.1000 - today");
                        return View(artist);
                    }

                    if ((enteredDate1 <= enteredDate || enteredDate1 >= endDate) && enteredDate1 != null)
                    {
                        ModelState.AddModelError("ActivityStop", "Not available value. Date should be after date of the start of activity until today");
                        return View(artist);
                    }
                    _context.Update(artist);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArtistExists(artist.Id))
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
            return View(artist);
        }

        // GET: Artists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Artists == null)
            {
                return NotFound();
            }

            var artist = await _context.Artists
                .FirstOrDefaultAsync(m => m.Id == id);
            if (artist == null)
            {
                return NotFound();
            }

            return View(artist);
        }

        // POST: Artists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Artists == null)
            {
                return Problem("Entity set 'DblibraryContext.Artists'  is null.");
            }
            var artist = await _context.Artists.FindAsync(id);
            if (artist != null)
            {
                _context.Artists.Remove(artist);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArtistExists(int id)
        {
          return (_context.Artists?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        

    }
}
