using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyLibrary.DAL;
using MyLibrary.Models;

namespace MyLibrary.Controllers
{
    public class ShelvesController : Controller
    {
        private readonly LibraryDb _context;

        public ShelvesController(LibraryDb context)
        {
            _context = context;
        }

        // GET: Shelves
        public async Task<IActionResult> Index(int id)
        {
            var libraryDb = _context.Shelf.Include(s => s.Genre).Where(s => s.GenreId == id);
            return View(await libraryDb.ToListAsync());
        }

        // GET: Shelves/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shelf = await _context.Shelf
                .Include(s => s.Genre)
                .FirstOrDefaultAsync(m => m.ShelfId == id);
            if (shelf == null)
            {
                return NotFound();
            }

            return View(shelf);
        }

        // GET: Shelves/Create
        public IActionResult Create()
        {
            ViewData["GenreId"] = new SelectList(_context.Genre, "GenreId", "GenreId");
            return View();
        }

        // POST: Shelves/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ShelfId,ShelfWidth,ShelfHeight,GenreId")] Shelf shelf)
        {
            if (ModelState.IsValid)
            {
                _context.Add(shelf);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GenreId"] = new SelectList(_context.Genre, "GenreId", "GenreId", shelf.GenreId);
            return View(shelf);
        }

        // GET: Shelves/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shelf = await _context.Shelf.FindAsync(id);
            if (shelf == null)
            {
                return NotFound();
            }
            ViewData["GenreId"] = new SelectList(_context.Genre, "GenreId", "GenreId", shelf.GenreId);
            return View(shelf);
        }

        // POST: Shelves/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ShelfId,ShelfWidth,ShelfHeight,GenreId")] Shelf shelf)
        {
            if (id != shelf.ShelfId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shelf);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShelfExists(shelf.ShelfId))
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
            ViewData["GenreId"] = new SelectList(_context.Genre, "GenreId", "GenreId", shelf.GenreId);
            return View(shelf);
        }

        // GET: Shelves/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shelf = await _context.Shelf
                .Include(s => s.Genre)
                .FirstOrDefaultAsync(m => m.ShelfId == id);
            if (shelf == null)
            {
                return NotFound();
            }

            return View(shelf);
        }

        // POST: Shelves/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var Shelf = await _context.Shelf
                .Include(s => s.Books)
                .FirstOrDefaultAsync(s => s.ShelfId == id);
            if (Shelf == null)
            {
                TempData["ErrorMessage"] = "לא נמצא ז'אנר עם מזהה זה.";
                return RedirectToAction(nameof(Index));
            }
            if (Shelf.Books.Count > 0)
            {
                foreach (var book in Shelf.Books) 
                {
                    _context.Book.Remove(book);
                }
            }
            id = Shelf.GenreId;
            _context.Shelf.Remove(Shelf);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "הז'אנר נמחק בהצלחה";
            return RedirectToAction(nameof(Index),new {id});
        }

        private bool ShelfExists(int id)
        {
            return _context.Shelf.Any(e => e.ShelfId == id);
        }
    }
}
