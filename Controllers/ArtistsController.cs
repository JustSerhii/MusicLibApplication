using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppLab;
using ClosedXML.Excel;

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile fileExcel)
        {
            if (ModelState.IsValid)
            {
                if (fileExcel != null)
                {
                    using (var stream = new FileStream(fileExcel.FileName, FileMode.Create))
                    {
                        await fileExcel.CopyToAsync(stream);
                        using (XLWorkbook workbook = new XLWorkbook(stream))
                        {
                            foreach (IXLWorksheet worksheet in workbook.Worksheets)
                            {
                                Artist newArtist;
                                var a = (from artist in _context.Artists
                                         where artist.Name.Contains(worksheet.Name)
                                         select artist).ToList();
                                if (a.Count > 0)
                                {
                                    newArtist = a[0];
                                }
                                else
                                {
                                    newArtist = new Artist();
                                    newArtist.Name = worksheet.Name;
                                    newArtist.ActiveSince = new DateTime();
                                    newArtist.ActivityStop = new DateTime();
                                    _context.Artists.Add(newArtist);
                                }
                                foreach (IXLRow row in worksheet.RowsUsed().Skip(1))
                                {
                                    try
                                    {
                                        /*Models.Department department = new Models.Department();
                                        department.DepName = row.Cell(1).Value.ToString();
                                        department.DateOfFoundation = row.Cell(2).Value;
                                        department.Loc = newloc;
                                        _context.Departments.Add(department);*/

                                        Song song;
                                        var d = (from songs in _context.Songs where songs.Title.Contains(row.Cell(1).Value.ToString()) select songs).ToList();
                                        if (d.Count == 0)
                                        {
                                            song = new Song();
                                            song.Title = row.Cell(1).Value.ToString();
                                            song.SongLength = (double)row.Cell(2).Value;
                                            song.ReleaseDate = row.Cell(3).GetDateTime();
                                            song.GenreId = (int)row.Cell(5).Value;
                                            song.Artist = newArtist;
                                            _context.Songs.Add(song);
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.ToString());
                                    }
                                }
                            }
                        }
                    }
                }
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        public ActionResult Export()
        {
            using (XLWorkbook workbook = new XLWorkbook())
            {
                var artists = _context.Artists.Include("Songs").ToList();

                foreach (var a in artists)
                {
                    var worksheet = workbook.Worksheets.Add(a.Name);

                    worksheet.Cell("A1").Value = "Song Title";
                    worksheet.Cell("B1").Value = "Song Length";
                    worksheet.Cell("C1").Value = "Release Date";
                    worksheet.Cell("D1").Value = "Artist Name";
                    worksheet.Cell("E1").Value = "Genre";
                    //worksheet.Cell("D1").Value = "Artist Name";
                    worksheet.Row(1).Style.Font.Bold = true;
                    var songs = a.Songs.ToList();

                    for (int i = 0; i < songs.Count; i++)
                    {
                        worksheet.Cell(i + 2, 1).Value = songs[i].Title;
                        worksheet.Cell(i + 2, 2).Value = songs[i].SongLength;
                        worksheet.Cell(i + 2, 3).Value = songs[i].ReleaseDate;
                        worksheet.Cell(i + 2, 4).Value = a.Name;
                        worksheet.Cell(i + 2, 5).Value = songs[i].GenreId;
                        //worksheet.Cell(i + 2, 4).Value = a.Name;
                    }
                }
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Flush();
                    return new FileContentResult(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        FileDownloadName = $"artists_{DateTime.UtcNow.ToShortDateString()}.xlsx"
                    };

                }
            }
        }
    }
}
