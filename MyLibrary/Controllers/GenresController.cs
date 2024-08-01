using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyLibrary.DAL;
using MyLibrary.Models;
using MyLibrary.ViewModels;

namespace MyLibrary.Controllers
{
    public class GenresController : Controller
    {
        private readonly LibraryDb _context;

        public GenresController(LibraryDb context)
        {
            _context = context;
        }

        // GET: Genres
        public async Task<IActionResult> Index()
        {
            return View(await _context.Genre.ToListAsync());
        }

        // GET: Genres/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genre = await _context.Genre
                .FirstOrDefaultAsync(m => m.GenreId == id);
            if (genre == null)
            {
                return NotFound();
            }

            return View(genre);
        }

        // GET: Genres/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Genres/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GenreId,Name")] Genre genre)
        {
            if (ModelState.IsValid)
            {
                _context.Add(genre);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "נוסף בהצלחה";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = "ערכים אינם תקינים";
            return View(genre);
        }

        // GET: Genres/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genre = await _context.Genre.FindAsync(id);
            if (genre == null)
            {
                return NotFound();
            }
            return View(genre);
        }

        // POST: Genres/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("GenreId,Name")] Genre genre)
        {
            if (id != genre.GenreId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(genre);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GenreExists(genre.GenreId))
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
            return View(genre);
        }

        // GET: Genres/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genre = await _context.Genre
                .FirstOrDefaultAsync(m => m.GenreId == id);
            if (genre == null)
            {
                return NotFound();
            }

            return View(genre);
        }

        // POST: Genres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var genre = await _context.Genre
                .Include(g => g.shelves)
                .Include(g => g.books)
                .FirstOrDefaultAsync(g => g.GenreId == id);

            if (genre == null)
            {
                TempData["ErrorMessage"] = "לא נמצא ז'אנר עם מזהה זה.";
                return RedirectToAction(nameof(Index));
            }

            if (genre.shelves.Count > 0)
            {
                foreach(Book book in genre.books)
                {
                    _context.Book.Remove(book);
                }
                foreach(Shelf shelf in genre.shelves)
                {
                    _context.Shelf.Remove(shelf);
                }
            }

            _context.Genre.Remove(genre);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "הז'אנר נמחק בהצלחה";
            return RedirectToAction(nameof(Index));
        }

        //GET: Genres/Add/Id
        public async Task<IActionResult> Add(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var genre = await _context.Genre
                 .FirstOrDefaultAsync(m => m.GenreId == id);
            if (genre == null)
            {
                return NotFound();
            }
            var genreAddShelf = new GenreAddShelf() { Genre = genre };
            return View(genreAddShelf);
        }
        //POST: Genres/Add/Id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(GenreAddShelf genreAddShelf)
        {
            
            if (genreAddShelf.Genre.GenreId == null)
            {
                return NotFound();
            }
            var genre = await _context.Genre
                 .FirstOrDefaultAsync(m => m.GenreId == genreAddShelf.Genre.GenreId);
            if (genre == null)
            {
                return NotFound();
            }
            if (genreAddShelf.height == null || genreAddShelf.height <= 0)
            {
                TempData["ErrorMessage"] = "גובה לא תקין";
                return View(genreAddShelf);
            }
            if (genreAddShelf.width == null || genreAddShelf.width <= 0)
            {
                TempData["ErrorMessage"] = "רוחב לא תקין";
                return View(genreAddShelf);
            }
            Shelf shelf = new Shelf()
            {
                ShelfWidth = genreAddShelf.width,
                ShelfHeight = genreAddShelf.height,
                GenreId = genre.GenreId,
                Genre = genre,
            };
            _context.Add(shelf);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "מדף נוסף בהצלחה";
            return RedirectToAction(nameof(Index));
        }

        private bool GenreExists(int id)
        {
            return _context.Genre.Any(e => e.GenreId == id);
        }
    }
}
