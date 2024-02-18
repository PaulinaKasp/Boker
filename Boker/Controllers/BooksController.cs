using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Boker.Data;
using Boker.Models;
using Microsoft.AspNetCore.Authorization;

namespace Boker.Controllers
{
    [Authorize]
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Books
        public async Task<IActionResult> Index(string searchString)
        {
            IQueryable<Book> books = _context.Book.Include(b => b.BookType).Include(b => b.User);

            if (!string.IsNullOrEmpty(searchString))
            {
                books = books.Where(b => b.Title.Contains(searchString));
            }

            var bookList = await books.ToListAsync();
            return View(bookList);
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.BookType)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            var reviews = _context.BookReview
                .Where(r => r.BookId == id)
                .Include(r => r.User);

            book.Reviews = await reviews.ToListAsync();

            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            ViewData["BookTypeId"] = new SelectList(_context.Set<BookType>(), "Id", "Name");
            ViewData["userId"] = new SelectList(_context.Users, "Id", "Email");
            return View();
        }

        // POST: Books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Image,Title,Description,Author,TotalPages,BookTypeId,userId,Reviews")] Book book)
        {
            ModelState.Remove("Reviews");

            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["BookTypeId"] = new SelectList(_context.Set<BookType>(), "Id", "Id", book.BookTypeId);
            ViewData["userId"] = new SelectList(_context.Users, "Id", "Id", book.userId);
            return View(book);
        }

        // GET: Books/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            ViewData["BookTypeId"] = new SelectList(_context.Set<BookType>(), "Id", "Name", book.BookTypeId);
            ViewData["userId"] = new SelectList(_context.Users, "Id", "Email", book.userId);
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Image,Title,Description,Author,TotalPages,BookTypeId,userId,Reviews")] Book book)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            ModelState.Remove("Reviews");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
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
            ViewData["BookTypeId"] = new SelectList(_context.Set<BookType>(), "Id", "Id", book.BookTypeId);
            ViewData["userId"] = new SelectList(_context.Users, "Id", "Id", book.userId);
            return View(book);
        }

        // GET: Books/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.BookType)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id);
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
            if (_context.Book == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Book'  is null.");
            }
            var book = await _context.Book.FindAsync(id);
            if (book != null)
            {
                _context.Book.Remove(book);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
          return (_context.Book?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
