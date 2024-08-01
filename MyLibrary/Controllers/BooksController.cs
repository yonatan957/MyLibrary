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
        public async Task<IActionResult> Index(int id)
        {
            var libraryDb = await _context.Book.Include(b => b.Genre).Include(b => b.Shelf).Where(b => b.ShelfId == id).ToListAsync();
            var shelf = _context.Shelf.Include(s => s.Genre).Where(s => s.ShelfId == id).FirstOrDefault();
            TempData["idshelf"] = shelf.GenreId;
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
                if (bookAdd.Book.Count > 1)
                {
                    bookAdd.Book[0].Set = true;
                    if (string.IsNullOrEmpty(bookAdd.set))
                    {
                        TempData["ErrorMessage"] = "הכנס שם סט";
                        return View(bookAdd);
                    }
                    foreach (var item in bookAdd.Book) 
                    {
                        item.bookName = $"{bookAdd.set}-{item.bookName}";
                    }
                }
                int width = 0;
                foreach(Book book in bookAdd.Book)
                {
                    if (book.bookHeight <= 0 ||  book.bookWidth <= 0)
                    {
                        TempData["ErrorMessage"] = "גדלים לא חוקיים";
                        return View(book);
                    }
                    width += book.bookWidth;
                }
                var shelvesDb = await _context.Shelf.Where(s => s.GenreId == bookAdd.id).ToListAsync();
                var Shelf = LogicServer.setShlf(shelvesDb, width, bookAdd.Book[0].bookHeight);
                if (Shelf == null)
                {
                    TempData["ErrorMessage"] = "לא נמצא מדף, אנה הוסף מדף ונסה שנית";
                    return View(bookAdd);
                }
                foreach (Book book in bookAdd.Book)
                {
                    book.ShelfId = Shelf.ShelfId;
                    book.GenreId = bookAdd.id;
                    _context.Add(book);
                    if (Shelf.ShelfHeight - book.bookHeight >= 10)
                    {
                        TempData["ErrorMessage"] = "הספר נמוך יחסית למדף, הוכנס בכל זאת";
                    }
                }
                Shelf.ShelfWidth -= width;
                _context.Update(Shelf);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "המשימה בוצעה בהצלחה";
            }
            return View(bookAdd);
        }

        // GET: Books/Create/1
        public IActionResult AddToShelf(int id)
        {
            BookAdd bookAdd = new BookAdd()
            {
                id = id
            };
            TempData["idshelf"] = _context.Shelf.Where(s => s.ShelfId == id).FirstOrDefault().GenreId;
            return View(bookAdd);
        }
        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToShelf(BookAdd bookAdd)
        {
            if (ModelState.IsValid)
            {
                int width = 0;
                if (bookAdd.Book.Count > 1)
                {
                    bookAdd.Book[0].Set = true;
                    if (string.IsNullOrEmpty(bookAdd.set))
                    {
                        TempData["ErrorMessage"] = "הכנס שם סט";
                        return View(bookAdd);
                    }
                    foreach (var item in bookAdd.Book)
                    {
                        item.bookName = $"{bookAdd.set}-{item.bookName}";
                    }
                }
                foreach (var item in bookAdd.Book)
                {
                    if (item.bookHeight <= 0 || item.bookWidth <= 0)
                    {
                        TempData["ErrorMessage"] = "גדלים לא חוקיים";
                        return View(bookAdd);
                    }
                    width += item.bookWidth;
                }
                var Shelf = await _context.Shelf
                                .Include(s => s.Genre)
                                .FirstOrDefaultAsync(m => m.ShelfId == bookAdd.id);
                if (Shelf.ShelfWidth < width)
                {
                    TempData["ErrorMessage"] = "אין מקום במדף!";
                    return View(bookAdd) ;
                }
                foreach (Book book in bookAdd.Book)
                {
                    book.ShelfId = bookAdd.id;
                    book.GenreId = Shelf.GenreId;
                    _context.Add(book);
                    if (Shelf.ShelfHeight - book.bookHeight >= 10)
                    {
                        TempData["ErrorMessage"] = "הספר נמוך יחסית למדף, הוכנס בכל זאת";
                    }
                }
                Shelf.ShelfWidth -= width;
                _context.Update(Shelf);
                await _context.SaveChangesAsync();
                TempData["idshelf"] = Shelf.GenreId;
            }
            TempData["SuccessMessage"] = "המשימה בוצעה בהצלחה";
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
            var book = await _context.Book.Include(b => b.Shelf).Where(b => b.BookId == id).FirstOrDefaultAsync();
            var Shelf = book.Shelf ;
            Shelf.ShelfWidth += book.bookWidth;
            _context.Shelf.Update(Shelf);
            if (book != null)
            {
                _context.Book.Remove(book);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { id = Shelf.ShelfId });
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
