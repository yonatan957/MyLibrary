using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyLibrary.DAL;
using MyLibrary.Models;
using MyLibrary.Servers;
using MyLibrary.ViewModels;

namespace MyLibrary.Controllers
{
    public class BooksController : Controller
    {
        private readonly LibraryDb _context;

        public BooksController(LibraryDb context)
        {
            _context = context;
        }

        // GET: Books
        public async Task<IActionResult> Index()
        {
            var libraryDb = await _context.Book.Include(b => b.Genre).Include(b => b.Shelf).ToListAsync();
            return View( libraryDb);
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Genre)
                .Include(b => b.Shelf)
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create/1
        public IActionResult Create(int id)
        {
            BookAdd bookAdd = new BookAdd()
            {
                id = id
            };
            return View(bookAdd);
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookAdd bookAdd)
        {
            if (ModelState.IsValid)
            {
                var shelvesDb = await _context.Shelf.Where(s => s.GenreId == bookAdd.id).ToListAsync();
                var Shelf = LogicServer.setShlf(shelvesDb, bookAdd.Book.bookWidth, bookAdd.Book.bookHeight);
                if (Shelf == null)
                {
                    TempData["ErrorMessage"] = "Shelf not found. Please return and create new shelf.";
                    return View(bookAdd);
                }
                bookAdd.Book.ShelfId = Shelf.ShelfId;
                bookAdd.Book.GenreId = bookAdd.id;
                Shelf.ShelfWidth -= bookAdd.Book.bookWidth;
                _context.Update(Shelf);
                _context.Add(bookAdd.Book);
                await _context.SaveChangesAsync();
                if (Shelf.ShelfHeight - bookAdd.Book.bookHeight >= 10)
                {
                    TempData["ErrorMessage"] = "הספר נמוך יחסית למדף, הוכנס בכל זאת";
                }
            }
            return View(bookAdd);
        }

        public IActionResult CreateSet(int id)
        {

            return View();
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Genre)
                .Include(b => b.Shelf)
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Book.FindAsync(id);
            if (book != null)
            {
                _context.Book.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ////GET: Books/Add/Id
        //public async Task<IActionResult> Add(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }
        //    var genre = await _context.Genre
        //         .FirstOrDefaultAsync(m => m.GenreId == id);
        //    if (genre == null)
        //    {
        //        return NotFound();
        //    }
        //    var genreAddShelf = new GenreAddShelf() { Genre = genre };
        //    return View(genreAddShelf);
        //}
        ////POST: Books/Add/Id
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Add(GenreAddShelf genreAddShelf)
        //{

        //    if (genreAddShelf.Genre.GenreId == null)
        //    {
        //        return NotFound();
        //    }
        //    var genre = await _context.Genre
        //         .FirstOrDefaultAsync(m => m.GenreId == genreAddShelf.Genre.GenreId);
        //    if (genre == null)
        //    {
        //        return NotFound();
        //    }
        //    if (genreAddShelf.height == null)
        //    {
        //        ModelState.AddModelError("height", "height is required.");
        //        return View(genreAddShelf);
        //    }
        //    int width;
        //    if (genreAddShelf.width == null)
        //    {
        //        ModelState.AddModelError("width", "width is required.");
        //        return View(genreAddShelf);
        //    }
        //    Shelf shelf = new Shelf()
        //    {
        //        ShelfWidth = genreAddShelf.width,
        //        ShelfHeight = genreAddShelf.height,
        //        GenreId = genre.GenreId,
        //        Genre = genre,
        //    };
        //    _context.Add(shelf);
        //    await _context.SaveChangesAsync();
        //    return View(genreAddShelf);
        //}

        //// POST: Books/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("BookId,bookName,bookWidth,bookHeight,GenreId,ShelfId")] Book book)
        //{
        //    if (id != book.BookId)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(book);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!BookExists(book.BookId))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["GenreId"] = new SelectList(_context.Set<Genre>(), "GenreId", "GenreId", book.GenreId);
        //    ViewData["ShelfId"] = new SelectList(_context.Set<Shelf>(), "ShelfId", "ShelfId", book.ShelfId);
        //    return View(book);
        //}
        // GET: Books/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var book = await _context.Book.FindAsync(id);
        //    if (book == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["GenreId"] = new SelectList(_context.Set<Genre>(), "GenreId", "GenreId", book.GenreId);
        //    ViewData["ShelfId"] = new SelectList(_context.Set<Shelf>(), "ShelfId", "ShelfId", book.ShelfId);
        //    return View(book);
        //}

        private bool BookExists(int id)
        {
            return _context.Book.Any(e => e.BookId == id);
        }
    }
}
