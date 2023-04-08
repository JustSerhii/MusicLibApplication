using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppLab;
using Microsoft.AspNetCore.Authorization;


namespace WebAppLab.Controllers
{
    public class AlbumsController : Controller
    {
        private readonly DblibraryContext _context;

        public AlbumsController(DblibraryContext context)
        {
            _context = context;
        }

        // GET: Albums
        public async Task<IActionResult> Index()
        {
              return _context.Albums != null ? 
                          View(await _context.Albums.ToListAsync()) :
                          Problem("Entity set 'DblibraryContext.Albums'  is null.");
        }

        // GET: Albums/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Albums == null)
            {
                return NotFound();
            }

            var album = await _context.Albums
                .FirstOrDefaultAsync(m => m.Id == id);
            if (album == null)
            {
                return NotFound();
            }

            return RedirectToAction("Index", "AlbumSongs", new { id = album.Id, name = album.Title });
        }


        // GET: Albums/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Albums/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,ReleaseDate")] Album album)
        {
            if (ModelState.IsValid)
            {
                _context.Add(album);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(album);
        }

        // GET: Albums/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Albums == null)
            {
                return NotFound();
            }

            var album = await _context.Albums.FindAsync(id);
            if (album == null)
            {
                return NotFound();
            }
            return View(album);
        }

        // POST: Albums/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,ReleaseDate")] Album album)
        {
            if (id != album.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(album);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlbumExists(album.Id))
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
            return View(album);
        }

        // GET: Albums/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Albums == null)
            {
                return NotFound();
            }

            var album = await _context.Albums
                .FirstOrDefaultAsync(m => m.Id == id);
            if (album == null)
            {
                return NotFound();
            }

            return View(album);
        }


        // POST: Albums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Albums == null)
            {
                return Problem("Entity set 'DblibraryContext.Albums'  is null.");
            }
            var album = await _context.Albums.FindAsync(id);
            if (album != null)
            {
                _context.Albums.Remove(album);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AlbumExists(int id)
        {
          return (_context.Albums?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        public async Task<IActionResult> ReviewAdd(int? id)
        {
            if (id == null || _context.Albums == null)
            {
                return NotFound();
            }

            var album = await _context.Albums
                .FirstOrDefaultAsync(m => m.Id == id);
            if (album == null)
            {
                return NotFound();
            }

            return RedirectToAction("Index", "AlbumReviews", new { id = album.Id, name = album.Title });
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
                                Album newalb;
                                var a = (from album in _context.Albums
                                         where album.Title.Contains(worksheet.Name)
                                         select album).ToList();
                                if (a.Count > 0)
                                {
                                    newalb = a[0];
                                }
                                else
                                {
                                    newalb = new Album();
                                    newalb.Title = worksheet.Name;
                                    newalb.ReleaseDate = new DateTime(2015, 12, 31);
                                    _context.Albums.Add(newalb);
                                }
                                foreach (IXLRow row in worksheet.RowsUsed().Skip(1))
                                {
                                    try
                                    {
                                        /*Models.Department albumreview = new Models.Department();
                                        albumreview.DepName = row.Cell(1).Value.ToString();
                                        albumreview.DateOfFoundation = row.Cell(2).Value;
                                        albumreview.Loc = newloc;
                                        _context.AlbumReviews.Add(albumreview);*/

                                        AlbumReview albumreview;
                                        var ar = (from areview in _context.AlbumReviews where areview.Title.Contains(row.Cell(1).Value.ToString()) select areview).ToList();
                                        if (ar.Count == 0)
                                        {
                                            albumreview = new AlbumReview();
                                            albumreview.Title = row.Cell(1).Value.ToString();
                                            albumreview.WritingDate = row.Cell(2).Value;
                                            albumreview.ReviewContent = row.Cell(3).Value.ToString();
                                            albumreview.AlbumScore = (int)row.Cell(4).Value;
                                            albumreview.Album = newalb;
                                            _context.AlbumReviews.Add(albumreview);
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
                var albums = _context.Albums.Include("AlbumReviews").ToList();

                foreach (var a in albums)
                {
                    var worksheet = workbook.Worksheets.Add(a.Title);

                    worksheet.Cell("A1").Value = "Title";
                    worksheet.Cell("B1").Value = "Date of writing";
                    worksheet.Cell("C1").Value = "Review Content";
                    worksheet.Cell("D1").Value = "Album Score";
                    worksheet.Cell("E1").Value = "Album Title";
                    worksheet.Row(1).Style.Font.Bold = true;
                    var albumreviews = a.AlbumReviews.ToList();

                    for (int i = 0; i < albumreviews.Count; i++)
                    {
                        worksheet.Cell(i + 2, 1).Value = albumreviews[i].Title;
                        worksheet.Cell(i + 2, 2).Value = albumreviews[i].WritingDate;
                        worksheet.Cell(i + 2, 3).Value = albumreviews[i].ReviewContent;
                        worksheet.Cell(i + 2, 4).Value = albumreviews[i].AlbumScore;
                        worksheet.Cell(i + 2, 5).Value = a.Title;
                    }
                }
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Flush();
                    return new FileContentResult(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        FileDownloadName = $"albumreviews_{DateTime.UtcNow.ToShortDateString()}.xlsx"
                    };

                }
            }
        }
    }
}
