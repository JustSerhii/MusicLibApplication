using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
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

            //return RedirectToAction("Index", "AlbumSongs", new { id = song.Id, name = song.Title });
            return RedirectToAction("Index", "SongReviews", new { id = song.Id, name = song.Title });
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
                                Song newsong;
                                var l = (from song in _context.Songs
                                         where song.Title.Contains(worksheet.Name)
                                         select song).ToList();
                                if (l.Count > 0)
                                {
                                    newsong = l[0];
                                }
                                else
                                {
                                    newsong = new Song();
                                    newsong.Title = worksheet.Name;
                                    //newsong.r = "from EXCEL";
                                    _context.Songs.Add(newsong);
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

                                        SongReview songreview;
                                        var d = (from song in _context.Songs where song.Title.Contains(row.Cell(1).Value.ToString()) select song).ToList();
                                        if (d.Count == 0)
                                        {
                                            songreview = new SongReview();
                                            songreview.Title = row.Cell(1).Value.ToString();
                                            songreview.WritingDate = row.Cell(2).Value;
                                            songreview.ReviewContent = row.Cell(3).Value.ToString();
                                            songreview.SongScore = (int)row.Cell(4).Value;
                                            songreview.Song = newsong;
                                            _context.SongReviews.Add(songreview);
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
                var songs = _context.Songs.Include("SongReviews").ToList();

                foreach (var s in songs)
                {
                    var worksheet = workbook.Worksheets.Add(s.Title);

                    worksheet.Cell("A1").Value = "Title";
                    worksheet.Cell("B1").Value = "Date of Writing";
                    worksheet.Cell("C1").Value = "Review Content";
                    worksheet.Cell("E1").Value = "Song Score";
                    worksheet.Row(1).Style.Font.Bold = true;
                    var songreviews = s.SongReviews.ToList();

                    for (int i = 0; i < songreviews.Count; i++)
                    {
                        worksheet.Cell(i + 2, 1).Value = songreviews[i].Title;
                        worksheet.Cell(i + 2, 2).Value = songreviews[i].WritingDate;
                        //worksheet.Cell(i + 2, 3).Value = l.LocName;
                        worksheet.Cell(i + 2, 3).Value = songreviews[i].ReviewContent;
                        worksheet.Cell(i + 2, 4).Value = songreviews[i].SongScore;
                    }
                }
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Flush();
                    return new FileContentResult(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        FileDownloadName = $"reviews_{DateTime.UtcNow.ToShortDateString()}.xlsx"
                    };

                }
            }
        }
    }
}